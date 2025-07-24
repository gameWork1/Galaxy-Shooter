using Logger;
using UnityEngine;
using Zenject;

namespace GameInstallers
{
    public class OnlineSceneInstaller : MonoInstaller
    {
        [SerializeField] private LoggerManager _loggerManager;
        
        public override void InstallBindings()
        {
            Container.Bind<LoggerManager>().FromInstance(_loggerManager).AsSingle().NonLazy();
        }
    }
}