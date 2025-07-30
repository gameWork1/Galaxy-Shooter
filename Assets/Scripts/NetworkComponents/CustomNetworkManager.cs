using Logger;
using Mirror;
using Mirror.Discovery;
using ModestTree;
using UnityEngine;
using Utp;
using Zenject;

namespace Network
{
    public class CustomNetworkManager : RelayNetworkManager 
    {
        private string _playerName;
        [Inject] private NetworkDiscovery _networkDiscovery;

        public void ChangePlayerName(string newText) => _playerName = newText;

        public bool isNicknameEmpty() => string.IsNullOrEmpty(_playerName) || _playerName == "";

        public string GetNickName() => _playerName;

        public void StartHostDiscovery()
        {
            if (!isNicknameEmpty())
            {
                StartHost();
                _networkDiscovery.AdvertiseServer();
            }
        }

        public void StartRelayHost()
        {
            if (!isNicknameEmpty())
            {
                StartRelayHost(6, null);
            }
        }

        public void StartRelayClient()
        {
            if (!isNicknameEmpty())
            {
                JoinRelayServer();
            }
        }
        
        

        public override void OnServerAddPlayer(NetworkConnectionToClient conn)
        {
            Transform startPos = GetStartPosition();

            GameObject player = Instantiate(playerPrefab, startPos.position, startPos.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);
            
            // if (_player != null)
            // {
            //     _player.TargetSetUpLocalNickname(conn);
            // }else Debug.Log(1);
        }
        
    }
}