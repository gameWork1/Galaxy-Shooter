using System;
using UnityEngine;

namespace Sounds
{
    public class MusicPlayer : MonoBehaviour
    {
        public static MusicPlayer _musicPlayer;

        private void Start()
        {
            if (_musicPlayer == null)
            {
                _musicPlayer = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}