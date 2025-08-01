using System.Collections.Generic;
using UnityEngine;

namespace Ksantee
{
    public class StatesManager : MonoBehaviour
    {
        public ControllerStats stats;
        public ControllerStates states;
        public InputVariables inp;

        [System.Serializable]
        public class InputVariables
        {
            public float horizontal;
            public float vertical;
            public float moveAmount;
            public Vector3 moveDirection;
            public Vector3 aimPositon;
            public Vector3 rotateDirection;
        }

        [System.Serializable]
        public class ControllerStates
        {
            public bool onGround;
            public bool isAiming;
            public bool isCrouching;
            public bool isRunning;
            public bool isInteracting;
        }

        #region References
        public Animator anim;
        public GameObject activeModel;
        [HideInInspector]
        public AnimatorHook a_hook;

        [HideInInspector]
        public Rigidbody rb;

        [HideInInspector]
        public Collider controllerColliderl;

        List<Collider> ragdollColliders = new List<Collider>();
        List<Rigidbody> ragdollRigids = new List<Rigidbody>();

        [HideInInspector]
        public LayerMask ignoreLayers;
        [HideInInspector]
        public LayerMask ignoreForGround;

        //[HideInInspector]
        //public Transform referencesParent;
        [HideInInspector]
        public Transform mTransform;
        public CharState currentState;

        public float delta;
        #endregion

        #region Init
        public void Init()
        {
            mTransform = transform;
            SetupAnimator();
            rb = GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.drag = 4;
            rb.angularDrag = 999;
            rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            controllerColliderl = GetComponent<Collider>();

            SetupRagdoll();

            ignoreLayers = ~(1 << 9);
            ignoreForGround = ~(1 << 9 | 1 << 10);

            a_hook = activeModel.AddComponent<AnimatorHook>();
            a_hook.Init(this);
        }

        void SetupAnimator()
        {
            if (activeModel == null)
            {
                anim = GetComponentInChildren<Animator>();
                activeModel = anim.gameObject;
            }

            if (anim == null)
                anim = activeModel.GetComponent<Animator>();

            anim.applyRootMotion = false;
        }

        void SetupRagdoll()
        {
            Rigidbody[] rigids = activeModel.GetComponentsInChildren<Rigidbody>();
            foreach (var rigidbody in rigids)
            {
                Debug.Log(rigidbody.gameObject.name);
                if (rigidbody == rb)
                {
                    continue;
                }

                Collider collider = rigidbody.gameObject.GetComponent<Collider>();
                collider.isTrigger = true;

                ragdollRigids.Add(rigidbody);
                ragdollColliders.Add(collider);

                rigidbody.isKinematic = true;
                rigidbody.gameObject.layer = 10;
            }
        }
        #endregion

        #region FixedUpdate
        public void FixedTick(float d)
        {
            delta = d;
            switch (currentState)
            {
                case CharState.normal:
                    states.onGround = OnGround();
                    if (states.isAiming)
                        MovementAiming();
                    else
                        MovementNormal();
                    RotationNormal();
                    break;
                case CharState.onAir:
                    rb.drag = 0;
                    states.onGround = OnGround();
                    break;
                case CharState.cover:
                    break;
                case CharState.vaulting:
                    break;
                default:
                    break;
            }

        }

        void MovementNormal()
        {
            float speed = stats.walkSpeed;
            if (states.isRunning)
                speed = stats.runSpeed;
            if (states.isCrouching)
                speed = stats.croundSpeed;

            Vector3 dir = Vector3.zero;
            dir = mTransform.forward * (speed * inp.moveAmount);
            rb.velocity = dir;
        }

        void MovementAiming()
        {
            float speed = stats.aimSpeed;
            Vector3 dir = inp.moveDirection * speed;
            rb.velocity = dir;
        }

        void RotationNormal()
        {
            if(!states.isAiming)
                inp.rotateDirection = inp.moveDirection;

            Vector3 targetDir = inp.rotateDirection;
            targetDir.y = 0;

            if (targetDir == Vector3.zero)
                targetDir = mTransform.forward;

            Quaternion lookDir = Quaternion.LookRotation(targetDir);
            Quaternion targetRot = Quaternion.Slerp(mTransform.rotation, lookDir, stats.rotateSpeed * delta);
            mTransform.rotation = targetRot;
        }
        #endregion

        #region Update
        public void Tick(float d)
        {
            delta = d;
            switch (currentState)
            {
                case CharState.normal:
                    states.onGround = OnGround();
                    HandleAnimationsAll();
                    break;
                case CharState.onAir:
                    states.onGround = OnGround();
                    break;
                case CharState.cover:
                    break;
                case CharState.vaulting:
                    break;
                default:
                    break;
            }
        }

        void HandleAnimationsAll()
        {
            anim.SetBool(StaticStrings.sprint, states.isRunning);
            anim.SetBool(StaticStrings.aiming, states.isAiming);
            anim.SetBool(StaticStrings.crouch, states.isCrouching);

            if (states.isAiming)
            {
                HandleAnimationsAiming();
            }
            else
            {
                HandleAnimatonsNormal();
            }
        }

        void HandleAnimatonsNormal()
        {
            if (inp.moveAmount > 0.05f)
                rb.drag = 0;
            else
                rb.drag = 4;

            float anim_v = inp.moveAmount;
            anim.SetFloat(StaticStrings.vertical, anim_v, 0.15f, delta);
        }

        void HandleAnimationsAiming()
        {
            //float v = inp.vertical;
            //float h = inp.horizontal;
            float anim_v = inp.moveAmount;
            anim.SetFloat(StaticStrings.vertical, anim_v, 0.2f, delta);
        }
        #endregion

        bool OnGround()
        {
            Vector3 origin = mTransform.position;
            origin.y += 0.6f;
            Vector3 dir = -Vector3.up;
            float dis = .7f;
            RaycastHit hit;
            if (Physics.Raycast(origin, dir, out hit, dis, ignoreForGround))
            {
                Vector3 tp = hit.point;
                mTransform.position = tp;
                return true;
            }

            return false;
        }
    }

    public enum CharState
    {
        normal,
        onAir,
        cover,
        vaulting,
    }
}

