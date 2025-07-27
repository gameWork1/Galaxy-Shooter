using System;
using Mirror;
using Mirror.BouncyCastle.Math.Field;
using TMPro;
using UnityEngine;

namespace Player.Gun
{
    public class Gun : NetworkBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GunSO _gunConfig;
        [SerializeField] private Transform _startPosition;
        
        private UIManager _uiManager;
        
        private int ammoCount;
        private float shootDelay;
        private float rechargeDelay;

        private void Start()
        {
            _uiManager = FindObjectOfType<UIManager>();
            ChangeAmmoCount(_gunConfig.maxAmmoCount);
        }

        private void ChangeAmmoCount(int count)
        {
            ammoCount = count;
            _uiManager.AmmoText.text = $"{ammoCount}/{_gunConfig.maxAmmoCount}";
        }

        [Client]
        public void Shoot()
        {
            if (shootDelay <= 0 && ammoCount > 0)
            {
                CmdShoot(NetworkClient.connection.identity.netId);
                shootDelay = _gunConfig.shootDelay;
                ChangeAmmoCount(ammoCount - 1);
                if (ammoCount == 0)
                {
                    rechargeDelay = _gunConfig.rechargeTime;
                }
            }
                
        }

        [Command(requiresAuthority = false)]
        private void CmdShoot(uint id)
        {
            GameObject bullet = Instantiate(_bulletPrefab, _startPosition.position, transform.rotation);
            
            NetworkServer.Spawn(bullet);
            
            bullet.GetComponent<Bullet>().SetData(id, _gunConfig.damage, _gunConfig.speed);
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            
            if (shootDelay > 0) shootDelay -= Time.deltaTime;
            if (rechargeDelay > 0)
            {
                rechargeDelay -= Time.deltaTime;
                if (rechargeDelay <= 0)
                {
                    ChangeAmmoCount(_gunConfig.maxAmmoCount);
                }
            }
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            if (angle > 90 || angle < -90) transform.localScale = new Vector3(1, -1, 1);
            else transform.localScale = new Vector3(1, 1, 1);
        }
    }
}