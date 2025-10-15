using UnityEngine;
using Unity.Netcode;

public class OnlinePlayerMini : NetworkBehaviour
{
    public float moveSpeed = 5f;

    public override void OnNetworkSpawn()
    {
      
        if (!IsOwner) enabled = false;
    }

    void Update()
    {
      
        if (!IsOwner) return;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        Vector3 dir = new Vector3(h, 0f, v).normalized;
        transform.Translate(moveSpeed * Time.deltaTime * dir, Space.World);
      
     //   Debug.Log($"[Input] owner {OwnerClientId} h={h} v={v}");
}
    }

