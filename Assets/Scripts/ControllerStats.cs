using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ksantee
{
    [CreateAssetMenu(menuName = "Controller/Stats")]
    public class ControllerStats : ScriptableObject
    {
        public float walkSpeed = 4;
        public float runSpeed = 6;
        public float croundSpeed = 2;
        public float aimSpeed = 2;
        public float rotateSpeed = 8;
    }
}