using UnityEngine;
using Unity.Netcode;

public class OnlinePlayerMini : NetworkBehaviour
{
    public float moveSpeed = 5f;

    public override void OnNetworkSpawn()
    {
        // only the local owner should run input/movement
        if (!IsOwner) enabled = false;
    }

    void Update()
    {
        // extra guard in case Unity re-enables component
        if (!IsOwner) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        var dir = new Vector3(h, 0f, v).normalized;
        transform.Translate(dir * moveSpeed * Time.deltaTime, Space.World);
    }
}
