using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ksantee
{
    public class InputHandler : MonoBehaviour
    {
        float horizontal;
        float vertical;

        bool aimInput;
        bool sprinntInput;
        bool shootInput;
        bool crouchInput;
        bool reloadInput;
        bool switchInput;
        bool pivotInput;

        bool isInit;

        float delta;

        public StatesManager states;

        public Transform camHolder;

        private void Start()
        {
            InitInGame();
        }

        public void InitInGame()
        {
            states.Init();
            isInit = true;
        }

        private void FixedUpdate()
        {
            if(!isInit) return;

            delta = Time.fixedDeltaTime;
            GetInput_FixedUpdate();
            InGame_UpdateStates_FixedUpdate();
            states.FixedTick(delta);
        }

        void GetInput_FixedUpdate()
        {
            vertical = Input.GetAxis("Vertical");
            horizontal = Input.GetAxis("Horizontal");
        }

        void InGame_UpdateStates_FixedUpdate()
        {
            states.inp.horizontal = horizontal;
            states.inp.vertical = vertical;
            states.inp.moveAmount = Mathf.Clamp01(Mathf.Abs(horizontal) + Mathf.Abs(vertical));

            Vector3 moveDir = camHolder.forward * vertical;
            moveDir += camHolder.right * horizontal;
            moveDir.Normalize();
            states.inp.moveDirection = moveDir;
        }

        private void Update()
        {
            if (!isInit) return;

            delta = Time.deltaTime;

            states.Tick(delta);
        }
    }

    public enum GamePhase
    {
        inGame, inMenu
    }
}
