using System;
using Mirror;
using TMPro;
using UnityEngine;

namespace Logger
{
    public class LoggerMessage : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _messageText;
        
        public void SetMessageText(string text)
        {
            _messageText.text = text;
        }

        private void Start()
        {
            Destroy(gameObject, 3.5f);
        }
    }
}