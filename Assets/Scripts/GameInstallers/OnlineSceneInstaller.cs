using Logger;
using Player;
using Player.Gun;
using UnityEngine;
using Zenject;

namespace GameInstallers
{
    public class OnlineSceneInstaller : MonoInstaller
    {
        [SerializeField] private LoggerManager _loggerManager;
        [SerializeField] private GameManager _gameManager;
        [SerializeField] private UIManager _uiManager;
        
        public override void InstallBindings()
        {
            Container.Bind<LoggerManager>().FromInstance(_loggerManager).AsSingle();
            Container.Bind<GameManager>().FromInstance(_gameManager).AsSingle();
            Container.Bind<UIManager>().FromInstance(_uiManager).AsSingle(); 
        }
    }
}