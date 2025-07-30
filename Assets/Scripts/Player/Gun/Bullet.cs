using System;
using Logger;
using Mirror;
using UnityEngine;
using Zenject;

namespace Player.Gun
{
    public class Bullet : NetworkBehaviour
    {
        [SerializeField] private GameObject _bulletDestroyEffect;
        [SyncVar] private uint _senderId;
        [SyncVar] private string _senderName;
        [SyncVar] private int _damage;
        [SyncVar] private float _speed;
        [SyncVar] private bool _isDamaged = false;
        private LoggerManager _logger;
        private GameManager _gameManager;

        [Inject]
        private void Construct(LoggerManager logger, GameManager gameManager)
        {
            _logger = logger;
            _gameManager = gameManager;
        }
        
        public void SetData(uint id, string senderName, int damage, float speed)
        {
            _senderId = id;
            _damage = damage;
            _speed = speed;
            _senderName = senderName;
            CmdSetGlobalData(id, senderName, damage, speed);   
        }

        [Command(requiresAuthority = false)]
        private void CmdSetGlobalData(uint id, string senderName, int damage, float speed)
        {
            _senderId = id;
            _damage = damage;
            _speed = speed;
            _senderName = senderName;
        }

        private void Update()
        {
            if (!isServer) return;
            
            transform.position += transform.right * _speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other.isTrigger) return;
            
            if (other.CompareTag("Wall"))
            {
                Deactivate();
                return;
            }
            
            if (!isServer) return;
            
            if (!_isDamaged && other.gameObject.TryGetComponent<PlayerController>(out PlayerController _player))
            {
                if (_player.GetComponent<NetworkIdentity>().netId != _senderId)
                {
                    if (_player.Health - _damage <= 0)
                    {
                        if (NetworkServer.spawned.TryGetValue(_senderId, out var senderPlayerObj))
                        {
                            senderPlayerObj.GetComponent<PlayerController>().TargetKill(senderPlayerObj.connectionToClient, _player.PlayerName);
                        }
                        
                    }
                    _player.TakeDamage(_damage);
                    _isDamaged = true;
                    Deactivate();
                }
            }
        }

        [Server]
        private void Deactivate()
        {
            GameObject effect = Instantiate(_bulletDestroyEffect, transform.position, Quaternion.identity);
            NetworkServer.Spawn(effect);
            
            NetworkServer.Destroy(gameObject);
        }

        
    }
}