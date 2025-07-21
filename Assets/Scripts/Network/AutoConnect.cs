using System;
using System.Threading.Tasks;
using Mirror;
using UnityEngine;

public class AutoConnect : MonoBehaviour
{
    [SerializeField] private float startDelayTime = 1.5f;
    
    private void Start()
    {
        StartConnection();
        
        
        
    }

    private async Task StartConnection()
    {
        await Task.Delay(Convert.ToInt32(startDelayTime * 1000));

        if (NetworkServer.active) await ConnectClientAsync();
        else await StartHostAsync();
    }


    private async Task<bool> ConnectClientAsync(float timeoutSeconds = 5f)
    {
        NetworkManager.singleton.StartClient();

        float timer = 0;

        while (!NetworkClient.isConnected && timer < timeoutSeconds)
        {
            await Task.Yield();
            timer += Time.deltaTime;
        }

        return NetworkClient.isConnected;
    }
    
    private async Task<bool> StartHostAsync(float timeoutSeconds = 5f)
    {
        NetworkManager.singleton.StartHost();

        float timer = 0;

        while (!NetworkClient.isConnected && !NetworkServer.active && timer < timeoutSeconds)
        {
            await Task.Yield();
            timer += Time.deltaTime;
        }

        return NetworkClient.isConnected && NetworkServer.active;
    }
}
