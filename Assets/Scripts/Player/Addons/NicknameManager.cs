using Logger;
using Mirror;
using TMPro;
using UnityEngine;
using Zenject;

namespace Player
{
    public class NicknameManager : NetworkBehaviour
    {
        [SerializeField] private TMP_Text nickNameText;

        [SyncVar(hook = nameof(ChangeNameText))]
        private string _playerName;

        public string PlayerName
        {
            get { return _playerName; }
        }
        
        private LoggerManager _logger;
        [SyncVar] private bool isJoined = false;

        [Inject]
        private void Construct(LoggerManager logger)
        {
            _logger = logger;
        }
        
        [Client]
        public void LogJoin(string name)
        {
            if (!isJoined)
            {
                isJoined = true;
                CmdLogInJoin(name, _logger);
            }
           
        }
    
        [Command(requiresAuthority = false)]
        private void CmdLogInJoin(string name, LoggerManager logger)
        {
            logger.AddPlayerConnectedMessage(name);
        }

        public void SetNickname(string text)
        {
            CmdSetNickname(text);
        }

        [Command(requiresAuthority = false)]
        private void CmdSetNickname(string text)
        {
            _playerName = text;
        }
    
        private void ChangeNameText(string oldText, string newText)
        {
            nickNameText.text = newText;
            CmdChangeNameText(newText);
        }
    
        [Command(requiresAuthority = false)]
        private void CmdChangeNameText(string text)
        {
            nickNameText.text = text;
        }
    }
}