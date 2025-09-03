using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData data;

    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public Collider col;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        SetWorldPhysics(true); // starts on ground by default
    }

    public void PickUp(Transform holdSocket)
    {
        // Parent to hand/hold socket
        transform.SetParent(holdSocket, worldPositionStays: false);
        transform.localPosition = data ? data.localPosition : Vector3.zero;
        transform.localEulerAngles = data ? data.localEulerAngles : Vector3.zero;
        transform.localScale = data ? data.localScale : Vector3.one;

        SetWorldPhysics(false);
    }

    public void Drop(Vector3 playerLinearVelocity, Vector3 forward, Vector3 up)
    {
        transform.SetParent(null);
        SetWorldPhysics(true);

        // Carry momentum
        rb.linearVelocity = playerLinearVelocity;

        // Toss
        float fwd = data ? data.dropForwardForce : 4f;
        float upF  = data ? data.dropUpwardForce  : 2f;
        rb.AddForce(forward * fwd, ForceMode.Impulse);
        rb.AddForce(up * upF,      ForceMode.Impulse);

        // Spin
        float torque = data ? data.randomTorque : 10f;
        float r = Random.Range(-1f, 1f);
        rb.AddTorque(new Vector3(r, r, r) * torque, ForceMode.Impulse);
    }

    private void SetWorldPhysics(bool world)
    {
        rb.isKinematic = !world;
        col.isTrigger  = !world;
    }
}
