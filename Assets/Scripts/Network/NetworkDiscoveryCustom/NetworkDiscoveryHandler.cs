using System;
using System.Collections.Generic;
using Mirror;
using Mirror.Discovery;
using Network.NetworkDiscoveryCustom;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NetworkDiscoveryHandler : MonoBehaviour
{
    [SerializeField] private NetworkDiscovery _networkDiscovery;
    [SerializeField] private NetworkDiscoveryHUDCustom _networkDiscoveryHUD;
    [SerializeField] private TMP_InputField _ipInputField;
    [SerializeField] private NetworkDiscoveryList _discoveryList;

    private List<ServerResponse> _serversFounded = new List<ServerResponse>();
    
    private void Start()
    {
        _networkDiscovery.OnServerFound.AddListener(OnServerFound);
    }

    public void CheckServers()
    {
        if (_ipInputField.text == "") 
            return;
        
        _discoveryList.ClearList();
        _serversFounded.Clear();
        _networkDiscovery.BroadcastAddress = _ipInputField.text;
        _networkDiscoveryHUD.ClearDiscoveryServers();
        _networkDiscovery.StartDiscovery();
            
    }
    
    private void OnServerFound(ServerResponse info)
    {
        if (!_serversFounded.Contains(info))
        {
            _serversFounded.Add(info);
            Debug.Log($"Найден сервер: {info.EndPoint.Address}");

            Button button = _discoveryList.AddIPButton(info);
            button.onClick.AddListener(() => StartClient(info));
        }
    }

    public void StartClient(ServerResponse _serverResponse)
    {
        _networkDiscovery.StopDiscovery();
        NetworkManager.singleton.StartClient(_serverResponse.uri);
    }
}
