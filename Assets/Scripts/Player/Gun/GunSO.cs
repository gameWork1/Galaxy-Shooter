using UnityEngine;

namespace Player.Gun
{
    [CreateAssetMenu(menuName = "Configs/Gun", fileName = "GunSO")]
    public class GunSO : ScriptableObject
    {
        public float speed;
        public int damage;
        public float shootDelay;
        public float rechargeTime;
        public int maxAmmoCount;
    }
}