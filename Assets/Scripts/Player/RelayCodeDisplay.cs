using System;
using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utp;

namespace Player
{
    public class RelayCodeDisplay : MonoBehaviour
    {
        [SerializeField] private TMP_Text _relayCodeText;
        [SerializeField] private Button _copyBuffer;
        
        private void Start()
        {
            if (NetworkManager.singleton is RelayNetworkManager _manager)
            {
                if (_manager.relayJoinCode != "")
                {
                    _relayCodeText.text = $"Код: {_manager.relayJoinCode}";
                    _copyBuffer.onClick.AddListener(() => GUIUtility.systemCopyBuffer = _manager.relayJoinCode);
                }else gameObject.SetActive(false);
                
            }
        }
    }
}