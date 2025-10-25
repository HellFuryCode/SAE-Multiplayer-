using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
#if PARRELSYNC
using ParrelSync;   
#endif


public class NetwrokManagerUI : MonoBehaviour
{
    [SerializeField] private Button hostButn;
    [SerializeField] private Button clientButn;

    void Awake()
    {
        hostButn.onClick.AddListener(OnHost);
        clientButn.onClick.AddListener(OnClient);
    }

    void OnHost()
    {
        var nm = NetworkManager.Singleton;

        // Choose A/Blue by default. If  host *from the clone*, pick B/Red.
        byte idx = 0;
#if PARRELSYNC
        if (ClonesManager.IsClone()) idx = 1;
#endif
        nm.NetworkConfig.ConnectionData = new byte[] { idx };
        nm.StartHost();
        Debug.Log($"[UI] StartHost payload idx={idx}");
    }

    void OnClient()
    {
        var nm = NetworkManager.Singleton;

        // Join as B/Red by default. If  joined from the main editor, keep B so we get A+B.
        byte idx = 1;
#if PARRELSYNC
        if (ClonesManager.IsClone()) idx = 1; else idx = 0;
#endif
        nm.NetworkConfig.ConnectionData = new byte[] { idx };
        nm.StartClient();
        Debug.Log($"[UI] StartClient payload idx={idx}");
    }
}
