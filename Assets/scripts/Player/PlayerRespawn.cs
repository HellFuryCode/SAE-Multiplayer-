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

// How To Make A HORROR Game In Unity 
//date accessed 2025/9/17
//created by: User1 Productions
//created on: 2022
//url: https://www.youtube.com/watch?v=qRgKB8l9GIg&list=PLlcgaDpDEvw05IgKGZo9FYA8Fo38RtAqH&index=24
//Online Video
//youtube