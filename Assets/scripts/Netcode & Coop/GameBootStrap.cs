using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class GameBootstrap : MonoBehaviour
{
    [Header("Local-only systems")]
    public GameObject joinSystemRoot;         // any local-couch objects to enable/disable
    public GameObject playerInputManagerRoot; // e.g., your PlayerInputManager holder

    [Header("Optional default online player prefab")]
    public GameObject defaultOnlinePlayerPrefab;

    void Start()
    {
        var mode = GameSessions.Instance?.Mode ?? GameSessions.GameMode.Local;

        // Local couch: enable JoinSystem (keyboard/controller mix), no networking
        if (mode == GameSessions.GameMode.Local)
        {
            ToggleLocalSystems(true);
            return;
        }

        // Online: disable local JoinSystem (spawns are network-driven)
        ToggleLocalSystems(false);

        var nm = NetworkManager.Singleton;
        if (!nm)
        {
            Debug.LogError("No NetworkManager found in GameScene.");
            return;
        }

        // Choose player prefab for NGO before StartHost/Client (optional)
        var chosen = GameSessions.Instance.OnlinePlayerPrefab ?? defaultOnlinePlayerPrefab;
        // if (chosen) nm.OnlinePlayerPrefab = chosen;

        // Configure transport (LAN/IP for now)
        var utp = nm.GetComponent<UnityTransport>();
        if (utp == null) { Debug.LogError("No UnityTransport on NetworkManager."); return; }

        utp.ConnectionData.Port = GameSessions.Instance.ServerPort;

        if (mode == GameSessions.GameMode.OnlineHost)
        {
            // Host listens on local machine; clients connect to host IP
            nm.StartHost();
            Debug.Log("Started Host");
        }
        else if (mode == GameSessions.GameMode.OnlineClient)
        {
            var ip = GameSessions.Instance.ServerIp;
            utp.SetConnectionData(ip, GameSessions.Instance.ServerPort);
            nm.StartClient();
            Debug.Log($"Started Client to {ip}:{GameSessions.Instance.ServerPort}");
        }
    }

    void ToggleLocalSystems(bool on)
    {
        if (joinSystemRoot) joinSystemRoot.SetActive(on);
        if (playerInputManagerRoot) playerInputManagerRoot.SetActive(on);
    }
}
