using UnityEngine;
using UnityEngine.UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class NetButtons : MonoBehaviour
{
    public Button hostBtn;
    public Button clientBtn;
    public TMPro.TMP_InputField ipField; 

    void Awake()
    {
        if (hostBtn) hostBtn.onClick.AddListener(StartHost);
        if (clientBtn) clientBtn.onClick.AddListener(StartClient);
    }

    void StartHost()
    {
        var nm = NetworkManager.Singleton;
        var utp = nm.GetComponent<UnityTransport>();
        utp.SetConnectionData("0.0.0.0", 7777, "0.0.0.0");
         nm.StartHost();
         Debug.Log("[NetButtons] Host listening on 0.0.0.0:7777");
    }

    void StartClient()
    {
        var nm = NetworkManager.Singleton;
        var utp = nm.GetComponent<UnityTransport>();
        string ip = ipField && !string.IsNullOrWhiteSpace(ipField.text) ? ipField.text : "192.168.1.8";
        utp.SetConnectionData(ip, 7777);
        nm.StartClient();
        Debug.Log($"[NetButtons] Client started → {ip}:7777");
    }
}
