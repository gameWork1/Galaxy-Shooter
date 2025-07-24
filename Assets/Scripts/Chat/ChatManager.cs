using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chat.Structs;
using Mirror;
using Player;
using TMPro;
using UnityEngine;

namespace Chat
{
    public class ChatManager : NetworkBehaviour
    {
        [SerializeField] private TMP_Text _chatText;
        [SerializeField] private TMP_InputField _messageField;
        [SerializeField] private GameObject _inputUI;
        
        private PlayerController _player;
        private InputActions _inputActions;
        private bool isEditing = false;

        private void Start()
        {
            _inputActions = new InputActions();
            _inputActions.Player.Enable();
            _inputUI.SetActive(false);
        }

        public override void OnStartClient()
        {
            StartCoroutine(WaitForIdentity());
        }
        
        private IEnumerator WaitForIdentity()
        {
            while (!NetworkClient.ready || NetworkClient.connection == null || NetworkClient.connection.identity == null)
                yield return null;

            _player = NetworkClient.connection.identity.GetComponent<PlayerController>();
        }

        public void AddMessage()
        {
            if (_player != null && _messageField.text != "")
            {
                CmdAddMessage(new ChatMessage()
                {
                    author = _player.PlayerName,
                    message = _messageField.text
                });
                _messageField.text = "";
            }
        }

        public void SetIsEditing(bool isEdit)
        {
            isEditing = isEdit;
        }

        private void Update()
        {
            if (_inputActions.Player.Chat.WasPressedThisFrame() && !isEditing)
            {
                _player.SetUsing(!_inputUI.active);
                _inputUI.SetActive(!_inputUI.active);
            }
        }

        [Command(requiresAuthority = false)]
        private void CmdAddMessage(ChatMessage message)
        {
            RpcAddMessage(message);
        }
        
        [ClientRpc]
        private void RpcAddMessage(ChatMessage message)
        {
            _chatText.text += (_chatText.text != "" ? "\n" : "") + $"<color=yellow>{message.author}</color>: {message.message}";
        }
    }
}