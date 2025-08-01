using UnityEngine;

namespace Ksantee
{
    public class CameraHandler : MonoBehaviour
    {
        public Transform camTrans;
        public Transform target;
        public Transform pivot;
        public Transform mTransform;
        public bool leftPivot;
        float delta;

        float mouseX;
        float mouseY;
        float smoothX;
        float smoothY;
        float smoothXVelocity;
        float smoothYVelocity;
        float lookAngle;
        float tiltAngle;

        public CameraValue values;

        StatesManager states;

        public void Init(InputHandler inp)
        {
            mTransform = this.transform;
            states = inp.states;
            target = states.mTransform;
        }

        public void FixedTick(float d)
        {
            delta = d;

            if (target == null) return;

            HandlePosition();
            HandleRotation();

            float speed = values.moveSpeed;
            if (states.states.isAiming)
                speed = values.aimSpeed;

            Vector3 targetPosiont = Vector3.Lerp(mTransform.position, target.position, delta * speed);
            mTransform.position = targetPosiont;
        }

        public void HandlePosition()
        {
            float targetX = values.normalX;
            float targetY = values.normalY;
            float targetZ = values.normalZ;

            if (states.states.isCrouching)
                targetY = values.crouchY;

            if (states.states.isAiming)
            {
                targetX = values.aimX;
                targetZ = values.aimZ;
            }

            if (leftPivot)
                targetX = -targetX;

            Vector3 newPivotPositon = pivot.localPosition;
            newPivotPositon.x = targetX;
            newPivotPositon.y = targetY;

            Vector3 newCamPositon = camTrans.localPosition;
            newCamPositon.z = targetZ;

            float t = delta * values.adaptSpeed;
            pivot.localPosition = Vector3.Lerp(pivot.localPosition, newPivotPositon, t);
            camTrans.localPosition = Vector3.Lerp(camTrans.localPosition, newCamPositon, t);
        }

        public void HandleRotation()
        {
            mouseX = Input.GetAxis("Mouse X");
            mouseY = Input.GetAxis("Mouse Y");

            if (values.turnSmooth > 0)
            {
                smoothX = Mathf.SmoothDamp(smoothX, mouseX, ref smoothXVelocity, values.turnSmooth);
                smoothY = Mathf.SmoothDamp(smoothY, mouseY, ref smoothYVelocity, values.turnSmooth);
            }
            else
            {
                smoothX = mouseX;
                smoothY = mouseY;
            }

            lookAngle += smoothX * values.y_rotate_speed;
            Quaternion targetRot = Quaternion.Euler(0, lookAngle, 0);
            mTransform.rotation = targetRot;

            tiltAngle -= smoothY * values.x_rotate_speed;
            tiltAngle = Mathf.Clamp(tiltAngle, values.minAngle, values.maxAngle);
            pivot.localRotation = Quaternion.Euler(tiltAngle, 0, 0);
        }
    }
}