using Mirror.Discovery;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Network.NetworkDiscoveryCustom
{
    public class NetworkDiscoveryList : MonoBehaviour
    {
        [SerializeField] private GameObject _ipButton;
        [SerializeField] private Transform _parent;
        
        public Button AddIPButton(ServerResponse _serverResponse)
        {
            GameObject obj = Instantiate(_ipButton, _parent);

            Button _button = obj.GetComponent<Button>();

            _button.GetComponentInChildren<TMP_Text>().text = _serverResponse.EndPoint.Address.ToString();

            return _button;
        }

        public void ClearList()
        {
            while (_parent.childCount > 0)
            {
                Debug.Log(1);
                DestroyImmediate(_parent.GetChild(0).gameObject);
            }
        }
    }
}