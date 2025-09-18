using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    public Transform spawnPoint;

    public void RespawnNow(float yOffest = 0f)
    {
        if (!spawnPoint) return;
        var pos = spawnPoint.position + Vector3.up * yOffest; //space to spawn without clipping or going mad\
        transform.SetPositionAndRotation(pos, spawnPoint.rotation);

        var rb = GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero;
        }
    }
}
