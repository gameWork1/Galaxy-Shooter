using System;
using System.Collections;
using Logger;
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
        [SerializeField] private PlayerController _player;
        
        private UIManager _uiManager;
        private LoggerManager _logger;
        
        private int ammoCount;
        private float shootDelay;

        private Coroutine _rechargeCoroutine;

        private void Start()
        {
            _uiManager = FindObjectOfType<UIManager>();
            _logger = FindObjectOfType<LoggerManager>();
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
            if (shootDelay <= 0 && ammoCount > 0 && _rechargeCoroutine == null)
            {
                CmdShoot(NetworkClient.connection.identity.netId, _startPosition.position, transform.rotation);
                shootDelay = _gunConfig.shootDelay;
                ChangeAmmoCount(ammoCount - 1);
                if (ammoCount == 0)
                {
                    Recharge();
                }
            }
                
        }

        [Client]
        public void Recharge()
        {
            if (_rechargeCoroutine != null || ammoCount == _gunConfig.maxAmmoCount) return;
            
            _rechargeCoroutine = StartCoroutine(RechargeCoroutine());
        }

        private IEnumerator RechargeCoroutine()
        {
            ChangeAmmoCount(0);
            
            yield return new WaitForSeconds(_gunConfig.rechargeTime);
            
            ChangeAmmoCount(_gunConfig.maxAmmoCount);

            _rechargeCoroutine = null;
        }

        [Command(requiresAuthority = false)]
        private void CmdShoot(uint id, Vector3 position, Quaternion rotation)
        {
            GameObject bullet = Instantiate(_bulletPrefab, position, rotation);
            
            NetworkServer.Spawn(bullet);
            
            bullet.GetComponent<Bullet>().SetData(id, _player.PlayerName, _gunConfig.damage, _gunConfig.speed, _logger);
        }

        private void FixedUpdate()
        {
            if (!isLocalPlayer) return;
            
            if (shootDelay > 0) shootDelay -= Time.deltaTime;
            
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector3 direction = mousePosition - transform.position;
            
            float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, angle);
            if (angle > 90 || angle < -90) transform.localScale = new Vector3(1, -1, 1);
            else transform.localScale = new Vector3(1, 1, 1);
        }
    }
}