using System;
using Mirror;

namespace Network
{
    public class NetworkPlayer : NetworkBehaviour
    {
        public static NetworkPlayer localPlayer;

        public string Nickname
        {
            get { return nickName; }
        }

        private string nickName;

        public void SetNickname(string nickname)
        {
            nickName = nickName;
        }
        
        private void Start()
        {
            if (isLocalPlayer) localPlayer = this;
        }
    }
}