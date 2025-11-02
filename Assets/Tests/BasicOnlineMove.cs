using UnityEngine;
using Unity.Netcode;

public class BasicOnlineMove : NetworkBehaviour
{
    public float moveSpeed = 5f;
    public float turnSpeed = 360f;

    void Update()
    {
        if (!IsOwner) return; // only the owner reads input

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0f, v).normalized;
        if (dir.sqrMagnitude > 0.01f)
        {
            // face move direction
            Quaternion target = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, target, turnSpeed * Time.deltaTime);
            transform.position += dir * moveSpeed * Time.deltaTime;
        }
    }
}
