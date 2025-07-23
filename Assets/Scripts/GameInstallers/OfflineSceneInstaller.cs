using Mirror.Discovery;
using Network.NetworkDiscoveryCustom;
using UnityEngine;
using Zenject;

public class OfflineSceneInstaller : MonoInstaller
{
    [SerializeField] private NetworkDiscovery _networkDiscovery;
    [SerializeField] private NetworkDiscoveryHUDCustom _networkDiscoveryHUD;
    
    public override void InstallBindings()
    {
        Container.Bind<NetworkDiscovery>().FromInstance(_networkDiscovery).AsSingle();
        Container.Bind<NetworkDiscoveryHUDCustom>().FromInstance(_networkDiscoveryHUD).AsSingle();
    }
}
