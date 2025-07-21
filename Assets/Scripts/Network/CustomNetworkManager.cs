using Mirror;
using Mirror.Discovery;
using UnityEngine;

namespace Network
{
    public class CustomNetworkManager : NetworkManager
    {
        private string _playerName;
        [SerializeField] private NetworkDiscovery _networkDiscovery;

        public void ChangePlayerName(string newText)
        {
            Debug.Log(newText);
            _playerName = newText;
        }

        public void StartHostDiscovery()
        {
            StartHost();
            _networkDiscovery.AdvertiseServer();
        }
        
        
    }
}