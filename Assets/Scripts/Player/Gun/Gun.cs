using System;
using System.Collections;
using Logger;
using Mirror;
using UnityEngine;
using Zenject;

namespace Player.Gun
{
    public class Gun : NetworkBehaviour
    {
        [SerializeField] private GameObject _bulletPrefab;
        [SerializeField] private GunSO _gunConfig;
        [SerializeField] private Transform _startPosition;
        [SerializeField] private PlayerController _player;
        [SerializeField] private GameObject _spriteGun;
        [SerializeField] private GameObject _shootSound;
        [SyncVar(hook = nameof(SpriteStateHook))] private bool _spriteState = false;
        
        private UIManager _uiManager;
        private SceneContext _sceneContext;
        
        private int ammoCount;
        private float shootDelay;

        private Coroutine _rechargeCoroutine;

        [Inject]
        private void Construct( UIManager uiManager)
        {
            _uiManager = uiManager;
            ChangeAmmoCount(_gunConfig.maxAmmoCount);
        }
        
        private void Start()
        {
            _sceneContext = FindFirstObjectByType<SceneContext>();
        }

        private void SpriteStateHook(bool oldState, bool newState)
        {
            _spriteState = newState;
            _spriteGun.SetActive(newState);
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

        public void SetSpriteState(bool state)
        {
            _spriteGun.SetActive(state);  
            CmdSetSpriteState(state);
        }

        [Command(requiresAuthority = false)]
        private void CmdSetSpriteState(bool state)
        {
            _spriteState = state;  
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
            GameObject bulletObj = Instantiate(_bulletPrefab, position, rotation);
            GameObject shootSound = Instantiate(_shootSound, position, Quaternion.identity);
            
            Bullet _bullet = bulletObj.GetComponent<Bullet>();
            
            _sceneContext.Container.Inject(_bullet);
            
            _bullet.SetData(id, _player.PlayerName, _gunConfig.damage, _gunConfig.speed);
            
            NetworkServer.Spawn(bulletObj);
            NetworkServer.Spawn(shootSound);
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