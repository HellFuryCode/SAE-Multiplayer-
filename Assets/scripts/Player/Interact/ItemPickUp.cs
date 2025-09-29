using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData data;

    [HideInInspector] public PlayerIdentity lastHolder;

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

    //who he;d it last??
        lastHolder = holdSocket.GetComponentInParent<PlayerIdentity>();

        if (data)
        {
            transform.localPosition = data.localPosition;
            transform.localEulerAngles = data.localEulerAngles;
        }

        //youre the reason its weird
            // transform.localPosition = data ? data.localPosition : Vector3.zero;
            // transform.localEulerAngles = data ? data.localEulerAngles : Vector3.zero;
            // transform.localScale = data ? data.localScale : Vector3.one;

            SetWorldPhysics(false);
    }

    public void Drop(Vector3 playerLinearVelocity, Vector3 forward, Vector3 up)
    {
        transform.SetParent(null, worldPositionStays: true); //clarity is important
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

// How To Make A HORROR Game In Unity | Basic Health System | Horror Series Part 024
//date accessed 2025/9/17
//created by: User1 Productions
//created on: 2022
//url: https://www.youtube.com/watch?v=qRgKB8l9GIg&list=PLlcgaDpDEvw05IgKGZo9FYA8Fo38RtAqH&index=24
//Online Video
//youtube

// How to Make a Flexible Interaction System in 2 Minutes [C#] [Unity3D] 
//date accessed 2025/9/17
//created by: Rytech
//created on: 2023
//url:  https://www.youtube.com/watch?v=K06lVKiY-sY 
//Online Video
//youtube
