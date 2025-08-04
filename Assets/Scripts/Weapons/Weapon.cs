using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Ksantee
{
    [CreateAssetMenu(menuName = "Weapons/Weapon", order = 0)]
    public class Weapon : ScriptableObject
    {
        public string id;

        public IKPosition m_h_ik;
        public GameObject modelPrefab;

        public float fireRate = 0.1f;
        public int magazineAmmo = 30;
        public int maxAmmo = 160;
        public AnimationCurve recoilY;
        public AnimationCurve recoilZ;
    }
}