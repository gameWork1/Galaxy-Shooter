using System;
using Mirror;
using UnityEngine;
using Utp;

namespace Player
{
    public class RelayCodeDisplay : MonoBehaviour
    {
        private void Start()
        {
            if (NetworkManager.singleton is RelayNetworkManager _manager)
            {
                FindFirstObjectByType<UIManager>().RelayCodeText.text = $"Код: {_manager.relayJoinCode}";
            }
        }
    }
}