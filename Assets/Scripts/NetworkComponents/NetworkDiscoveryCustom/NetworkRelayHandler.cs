using Mirror;
using TMPro;
using UnityEngine;
using Utp;

namespace Network.NetworkDiscoveryCustom
{
    public class NetworkRelayHandler : MonoBehaviour
    {
        [SerializeField] private TMP_InputField _joinCodeField;

        public void Join()
        {
            if (NetworkManager.singleton is RelayNetworkManager manager)
            {
                manager.relayJoinCode = _joinCodeField.text;
                manager.JoinRelayServer();
            }
        }
    }
}