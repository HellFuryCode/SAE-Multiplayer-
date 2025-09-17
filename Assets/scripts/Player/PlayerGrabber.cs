using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
public class PlayerGrabber : MonoBehaviour
{
    public Transform holdSocket;
    public LayerMask player;

    public float grabRange = 1.5f;
    public float grabRadius = 0.4f;

    public float maxHoldSeconds = 2.5f;
    public float tosdForward = 9f;
    public float tossUp = 5f;

    public bool requireNoItemHeld = true;
    private ItemPickup myHeldItem;
    private PlayerCarry carry;
    private float grabTimer;

    private Rigidbody myRb;
    private Collider colliderAgain;
    private PlayerScript_Multi moves;

    private void Awake()
    {
        myRb = GetComponent<Rigidbody>();
        colliderAgain = GetComponent<Collider>();
        moves = GetComponent<PlayerScript_Multi>();
    }

    private void Update()
    {
        if (carry)
        {
            grabTimer += Time.deltaTime;
            if (grabTimer >= maxHoldSeconds)
            {
                DoToss();
            }
        }
    }

    public void OnGrab(InputValue v)
    {
        if (!v.isPressed)
        {
            return;
        }

        if (carry == null)
        {
            TryGrab();
        }

        else
        {
            DoToss();
        }
    }
    private void TryGrab()
    {
        if (requireNoItemHeld)
        {
            myHeldItem = GetComponentInChildren<ItemPickup>();
            if (myHeldItem != null)
            {
                return;
            }
        }

        Vector3 origin = transform.position + Vector3.up * 0.09f;
        Vector3 dir = transform.forward;
        Ray ray = new Ray(origin, dir);

        var hits = Physics.SphereCastAll(ray, grabRadius, grabRange, player, QueryTriggerInteraction.Ignore);
        if (hits.Length == 0)
        {
            return; ;
        }

        PlayerCarry bestVictium = null;
        float bestDist = float.MaxValue;

        foreach (var h in hits)
        {
            var candidate = h.collider.GetComponentInParent<PlayerCarry>();
            if (!candidate || candidate.gameObject == this.gameObject) continue;
            if (candidate.beenGrabbed)
            {
                continue;
            }

            if (h.distance < bestDist)
            {
                bestDist = h.distance;
                bestVictium = candidate;
            }
        }

        if (!bestVictium)
        {
            return;
        }

        carry = bestVictium;
        grabTimer = 0f;

        Transform parent = holdSocket ? holdSocket : this.transform;
        carry.EnterGrab(parent, transform, colliderAgain);
    }

    private void DoToss()
    {
        if (carry == null)
        {
            return;
        }

        Vector3 vel = Vector3.zero;
        if (myRb) vel += myRb.linearVelocity * 0.25f; //carry a bit of our momentum
        vel += transform.forward * tosdForward + Vector3.up * tossUp;

        carry.ExitGrab(vel);
        carry = null;
    }
 


}
