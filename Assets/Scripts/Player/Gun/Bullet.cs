using System;
using Mirror;
using UnityEngine;

namespace Player.Gun
{
    public class Bullet : NetworkBehaviour
    {
        [SyncVar] private uint _senderId;
        [SyncVar] private int _damage;
        [SyncVar] private float _speed;
        [SyncVar] private bool _isDamaged = false;

        public void SetData(uint id, int damage, float speed)
        {
            _senderId = id;
            _damage = damage;
            _speed = speed;
            CmdSetGlobalData(id, damage, speed);   
        }

        [Command(requiresAuthority = false)]
        private void CmdSetGlobalData(uint id, int damage, float speed)
        {
            _senderId = id;
            _damage = damage;
            _speed = speed;
        }

        private void Update()
        {
            transform.position += transform.right * _speed * Time.deltaTime;
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!isServer) return;
            if (!_isDamaged && other.gameObject.TryGetComponent<PlayerController>(out PlayerController _player))
            {
                if (_player.GetComponent<NetworkIdentity>().netId != _senderId)
                {
                    _player.TakeDamage(_damage);
                    _isDamaged = true;
                    Deactivate();
                }
            }
        }

        [Command(requiresAuthority = false)]
        private void Deactivate()
        {
            Destroy(gameObject);
            NetworkServer.Destroy(gameObject);
        }
    }
}