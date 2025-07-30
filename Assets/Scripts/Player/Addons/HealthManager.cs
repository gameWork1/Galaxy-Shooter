using System;
using System.Collections;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Player
{
    public class HealthManager : NetworkBehaviour
    {
        [SerializeField] private int maxHealth = 100;
        [SerializeField] private Image _healthBar;
        [SerializeField] private PlayerController _player;
        [SerializeField] private int healthRegenerateAmount, healthRegenerateDelay;
        [SyncVar(hook = nameof(OnChangeHealth))] private int health;
        [SyncVar(hook = nameof(OnChangeStateDead))] private bool isDead;
        
        private Coroutine _deadTimerCoroutine;

        public int Health
        {
            get { return health; }
        }
        private UIManager _uiManager;
        
        private void Start()
        {
            health = maxHealth;
            _uiManager = FindObjectOfType<UIManager>();
            StartCoroutine(HealthRegenerator());
        }

        private void OnChangeStateDead(bool oldState, bool newState)
        {
            isDead = newState;
            
            if(isDead) _player.Dead();
            else _player.Respawn();

            if (_player.isLocalPlayer)
            {
                _uiManager.DeadPanel.SetActive(isDead);

                if(isDead) StartCoroutine(DeadTimeTimer(_uiManager.DeadTextCount));
            }
        }

        private void OnChangeHealth(int oldHealth, int newHealth)
        {
            health = newHealth;
            float amount = ((float)health) / ((float)maxHealth);
            SetHealthBar(amount);
        }
        
        private void SetHealthBar(float amount)
        {
            _healthBar.fillAmount = amount;
        }

        public void TakeDamage(int damage)
        {
            CmdTakeDamage(damage);
        }

        [Command(requiresAuthority = false)]
        private void CmdTakeDamage(int damage, NetworkConnectionToClient sender = null)
        {
            health -= damage;
            if (health <= 0)
            {
                isDead = true;
            }
        }
        private IEnumerator DeadTimeTimer(TMP_Text _countText)
        {
            int count = _player.DeadTime;
            while (count >= 0)
            {
                _countText.text = count.ToString();
                yield return new WaitForSeconds(1);
                count--;
            }
            CmdRespawn();
        }

        private IEnumerator HealthRegenerator()
        {
            while (true)
            {
                CmdRegenerateHealth();

                yield return new WaitForSeconds(healthRegenerateDelay);
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdRegenerateHealth()
        {
            if (health < maxHealth)
            {
                if (health + healthRegenerateAmount >= maxHealth)
                {
                    health = maxHealth;
                }
                else
                {
                    health += healthRegenerateAmount;
                }
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdRespawn(NetworkConnectionToClient sender = null)
        {
            health = maxHealth;
            isDead = false;
        }
        
    }
}