using UnityEngine;
using Unity.Netcode;

public class PlayerRespawn : NetworkBehaviour
{
    public Transform spawnPoint;
    public float defaultYOffset = 0.05f;

    private PlayerRespawnManager manager;
    private PlayerIdentity pid;
    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        pid = GetComponent<PlayerIdentity>();
        manager = FindFirstObjectByType<PlayerRespawnManager>();   
    }

    public void RespawnNow(float yOffest = float.NaN)
    {
        float y = float.IsNaN(yOffest) ? defaultYOffset : yOffest;

        if (!IsNetActive())
        {
            DoRespawn(y);
            return;
        }

        if (IsServer) DoRespawn(y);
        else RequestRespawnServerRpc(y);
    }
    
      bool IsNetActive() =>
        NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;

    Transform ResolveSpawn()
    {
        if (spawnPoint) return spawnPoint;
        if (manager && pid != null) return manager.GetSpawnFOrIndex(pid.playerIndex);
        return null; // fallback = current pos
    }

    void DoRespawn(float yOff)
    {
        var sp = ResolveSpawn();
        Vector3 pos = (sp ? sp.position : transform.position) + Vector3.up * yOff;
        Quaternion rot = sp ? sp.rotation : transform.rotation;

        transform.SetPositionAndRotation(pos, rot);

        if (rb)
        {
            rb.linearVelocity  = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    [ServerRpc(RequireOwnership = false)]
    void RequestRespawnServerRpc(float yOff) => DoRespawn(yOff);
}


// How To Make A HORROR Game In Unity 
//date accessed 2025/9/17
//created by: User1 Productions
//created on: 2022
//url: https://www.youtube.com/watch?v=qRgKB8l9GIg&list=PLlcgaDpDEvw05IgKGZo9FYA8Fo38RtAqH&index=24
//Online Video
//youtube