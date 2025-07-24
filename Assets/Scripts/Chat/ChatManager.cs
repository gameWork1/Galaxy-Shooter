using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Chat.Structs;
using Mirror;
using TMPro;
using UnityEngine;

namespace Chat
{
    public class ChatManager : NetworkBehaviour
    {
        public readonly SyncList<ChatMessage> _history = new SyncList<ChatMessage>();
        [SerializeField] private TMP_Text _chatText;
        [SerializeField] private TMP_InputField _messageField;

        private PlayerController _player;

        public override void OnStartClient()
        {
            StartCoroutine(WaitForIdentity());
            _history.Callback += ChangeChatMessagesHandler;
        }
        
        private IEnumerator WaitForIdentity()
        {
            while (!NetworkClient.ready || NetworkClient.connection == null || NetworkClient.connection.identity == null)
                yield return null;

            _player = NetworkClient.connection.identity.GetComponent<PlayerController>();
        }

        private void ChangeChatMessagesHandler(SyncList<ChatMessage>.Operation op, int index, ChatMessage oldItem, ChatMessage newItem)
        {
            string chat = "";

            for (int i = 0; i < _history.Count; i++)
            {
                ChatMessage message = _history[i];
                chat += $"<color=yellow>{message.author}</color>: {message.message}" + (i == _history.Count - 1 ? "" : "\n");
            }

            _chatText.text = chat;
            // CmdChangedChatMessages();
        }

        // [Command(requiresAuthority = false)]
        // private void CmdChangedChatMessages()
        // {
        //     string chat = "";
        //
        //     for (int i = 0; i < _history.Count; i++)
        //     {
        //         ChatMessage message = _history[i];
        //         chat += $"<color=yellow>{message.author}</color>: {message.message}" + (i == _history.Count - 1 ? "" : "\n");
        //     }
        //
        //     _chatText.text = chat;
        //     RpcChangeChatMessages(chat);
        // }
        //
        // [ClientRpc]
        // private void RpcChangeChatMessages(string text)
        // {
        //     _chatText.text = text;
        // }

        public void AddMessage()
        {
            if (_player != null && _messageField.text != "")
            {
                CmdAddMessage(_player.PlayerName, _messageField.text);
                _messageField.text = "";
            }
                
        }

        [Command(requiresAuthority = false)]
        private void CmdAddMessage(string author, string message)
        {
            _history.Add(new ChatMessage()
            {
                author = author,
                message = message
            });
        }
    }
}