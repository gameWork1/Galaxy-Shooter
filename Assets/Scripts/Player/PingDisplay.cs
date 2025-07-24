using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace Player
{
    public class PingDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _pingText;

        private int lastPing = 0;

        private void Start()
        {
            ChangePing();
        }

        private void Update()
        {
            ChangePing();
        }

        private void ChangePing()
        {
            int ping = (int) Math.Round(NetworkTime.rtt * 1000);
            if (lastPing != ping)
            {
                lastPing = ping;
                UpdateText();
            }
        }

        private void UpdateText()
        {
            _pingText.text = $"Ping: {lastPing.ToString()}";
        }
    }
}