using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;

public class GameBootstrap : MonoBehaviour
{
    public GameObject joinSystemRoot;         // any local-couch objects to enable/disable
    public GameObject playerInputManagerRoot; // e.g., your PlayerInputManager holder

 
    public GameObject defaultOnlinePlayerPrefab;

     private OnlinePlayerSpawner spawner;

    void Start()
    {
         var mode = GameSessions.Instance?.Mode ?? GameSessions.GameMode.Local;
        var nm = NetworkManager.Singleton;
        if (!nm) { Debug.LogError("No NetworkManager"); return; }

        var utp = nm.GetComponent<UnityTransport>();
        if (!utp) { Debug.LogError("No UnityTransport"); return; }
//        utp.ConnectionData.Port = GameSessions.Instance.ServerPort;

        spawner = FindFirstObjectByType<OnlinePlayerSpawner>();
        if (!spawner) { Debug.LogError("No OnlinePlayerSpawner in scene"); return; }

        nm.NetworkConfig.ConnectionApproval = true;
        nm.ConnectionApprovalCallback += spawner.ApproveAndSpawn;

        if (mode == GameSessions.GameMode.OnlineHost)
        {
            // host can also set payload to reflect selection
            nm.NetworkConfig.ConnectionData = new byte[] { GameSessions.Instance.SelectedCharIndex };
            nm.StartHost();
            Debug.Log("[Bootstrap] Host started");
        }
        else if (mode == GameSessions.GameMode.OnlineClient)
        {
            utp.SetConnectionData(GameSessions.Instance.ServerIp, GameSessions.Instance.ServerPort);
            nm.NetworkConfig.ConnectionData = new byte[] { GameSessions.Instance.SelectedCharIndex };
            nm.StartClient();
            // Debug.Log($"[Bootstrap] Client started {GameSessions.Instance.ServerIp}:{GameSessions.Instance.ServerPort} with char={GameSessions.Instance.SelectedCharIndex}");
        }
        else
        {
            Debug.LogWarning("Loaded Online scene with Mode=Local (no network start).");
        }
    }

    void ToggleLocalSystems(bool on)
    {
        if (joinSystemRoot) joinSystemRoot.SetActive(on);
        if (playerInputManagerRoot) playerInputManagerRoot.SetActive(on);
    }
}
