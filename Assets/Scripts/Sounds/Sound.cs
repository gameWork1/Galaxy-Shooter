using System;
using System.Collections;
using Mirror;
using UnityEngine;

namespace Sounds
{
    public class Sound : NetworkBehaviour
    {
        [SerializeField] private AudioSource _audioSource;

        private void Start()
        {
            StartCoroutine(DestroyAfterPlay());
        }

        private IEnumerator DestroyAfterPlay()
        {
            yield return new WaitWhile(() => _audioSource.isPlaying);
            
            NetworkServer.Destroy(gameObject);
            
        }
    }
}