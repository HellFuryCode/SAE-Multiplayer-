using Unity.Netcode;
using UnityEngine;
using System.Collections.Generic;

public class OnlinePlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefabA;
    public GameObject playerPrefabB;
    public Transform[] spawnPoints;
    int nextSpawn;

    private readonly HashSet<ulong> _spawned = new();

    public void ApproveAndSpawn(NetworkManager.ConnectionApprovalRequest req, NetworkManager.ConnectionApprovalResponse res)
    {
        res.Approved = true;
        res.CreatePlayerObject = false;  //sapnws maunally
        res.Pending = false;

        ulong clientId = req.ClientNetworkId;

        if (_spawned.Contains(clientId))
        {
            return;  //this client is already spawned
        }
        
        byte idx = 0;
        if (req.Payload != null && req.Payload.Length > 0) idx = req.Payload[0];

        var prefab = (idx == 0) ? playerPrefabA : playerPrefabB;
        if (!prefab) { Debug.LogWarning("Missing prefab, default A"); prefab = playerPrefabA; }

        var spawn = GetNext();
        var go = Instantiate(prefab, spawn.position, spawn.rotation);

        var no = go.GetComponent<NetworkObject>();
        if (!no) { Debug.LogError("Prefab missing NetworkObject"); Destroy(go); return; }

        no.SpawnAsPlayerObject(clientId, true);

        _spawned.Add(clientId);


        Debug.Log($"[Spawner] Spawned {(idx==0?"A":"B")} for client {req.ClientNetworkId}");
    }

    Transform GetNext()
    {
        if (spawnPoints != null && spawnPoints.Length > 0)
            return spawnPoints[(nextSpawn++) % spawnPoints.Length];
        return transform;
    }
}
