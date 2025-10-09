using Unity.Netcode;
using UnityEngine;

public class OnlinePlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefabA;
    public GameObject playerPrefabB;
    public Transform[] spawnPoints;
    int nextSpawn;

    public void ApproveAndSpawn(NetworkManager.ConnectionApprovalRequest req,
                                NetworkManager.ConnectionApprovalResponse res)
    {
        res.Approved = true;
        res.CreatePlayerObject = false;
        res.Pending = false;

        byte idx = 0;
        if (req.Payload != null && req.Payload.Length > 0) idx = req.Payload[0];

        var prefab = (idx == 0) ? playerPrefabA : playerPrefabB;
        if (!prefab) { Debug.LogWarning("Missing prefab, default A"); prefab = playerPrefabA; }

        var spawn = GetNext();
        var go = Instantiate(prefab, spawn.position, spawn.rotation);

        var no = go.GetComponent<NetworkObject>();
        if (!no) { Debug.LogError("Prefab missing NetworkObject"); Destroy(go); return; }

        no.SpawnAsPlayerObject(req.ClientNetworkId, true);
        Debug.Log($"[Spawner] Spawned {(idx==0?"A":"B")} for client {req.ClientNetworkId}");
    }

    Transform GetNext()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
            return spawnPoints[(nextSpawn++) % spawnPoints.Length];
        return transform;
    }
}
