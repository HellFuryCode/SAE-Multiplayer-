using UnityEngine;
using Unity.Netcode;

public class NetButtions : MonoBehaviour
{
    public void Host()  => NetworkManager.Singleton.StartHost();
    public void Join()  => NetworkManager.Singleton.StartClient();
    public void StopAll()
    {
        if (NetworkManager.Singleton.IsHost || NetworkManager.Singleton.IsClient)
            NetworkManager.Singleton.Shutdown();
    }
}