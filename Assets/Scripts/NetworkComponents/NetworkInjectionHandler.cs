namespace Network
{
    using UnityEngine;
    using Zenject;

    public class NetworkInjectionHandler : MonoBehaviour
    {
        [Inject] private DiContainer _container;

        public void Inject(GameObject obj)
        {
            _container.InjectGameObject(obj);
        }
    }

}