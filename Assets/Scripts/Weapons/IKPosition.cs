using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Ksantee
{
    [CreateAssetMenu(menuName = "Weapons/IKPosition")]
    public class IKPosition : ScriptableObject
    {
        public Vector3 pos;
        public Vector3 rot;
    }
}