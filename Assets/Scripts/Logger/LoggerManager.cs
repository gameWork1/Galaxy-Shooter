using Mirror;
using UnityEngine;

namespace Logger
{
    public class LoggerManager : NetworkBehaviour
    {
        [SerializeField] private GameObject _messagePrefab;
        [SerializeField] private Transform _parent;

        [ClientRpc]
        private void AddMessage(string text)
        {
            GameObject _messageObj = Instantiate(_messagePrefab, _parent);
            
            NetworkServer.Spawn(_messageObj.gameObject);
            
            _messageObj.GetComponent<LoggerMessage>().SetMessageText(text);
        }
        
        [Server]
        public void AddPlayerConnectedMessage(string playerName)
        {
            AddMessage($"К игре присоединился игрок <color=red>{playerName}</color>!");
        }
    }
}