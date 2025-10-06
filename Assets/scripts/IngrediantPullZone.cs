using UnityEngine;
using UnityEngine.Events;


public class IngrediantPullZone : MonoBehaviour
{
    //what to pull depending on the tags
    public string[] allowedTags = new string[] { "Ingredient", "Collectible" };
    public LayerMask affectedLayers = ~0;

    public bool requireNotHeld = true;

    public Transform attractionPoint;

    public float pullStrength = 30f;

    public float maxAcceleration = 60f;
    public AnimationCurve distanceFalloff = AnimationCurve.EaseInOut(0, 1, 1, 0);

    public float snapDistance = 0.25f;

    public float snapSpeed = 10f;
    public float consumeDistance = 0.1f;
    public bool destroyOnConsume = true;

    //events
    public UnityEvent<GameObject> OnObjectEnter;
    public UnityEvent<GameObject> OnObjectExit;
    public UnityEvent<GameObject> OnObjectConsumed;

    // cache the trigger collider and a representative radius for falloff
    private Collider triggerCol;
    private float effectiveRadius = 1f;

    private void Reset()
    {
        // Ensure the collider is set as trigger on add
        var col = GetComponent<Collider>();
        if (col) col.isTrigger = true;
    }

    private void Awake()
    {
        triggerCol = GetComponent<Collider>();
        if (!triggerCol) Debug.LogError(" Requires a Collider set as Trigger.");
        else if (!triggerCol.isTrigger)
        {
            Debug.LogWarning(" Collider was not trigger. Setting isTrigger = true.");
            triggerCol.isTrigger = true;
        }

        if (attractionPoint == null) attractionPoint = transform;

        if (triggerCol is SphereCollider sc)
        {
            effectiveRadius = Mathf.Abs(sc.radius) * Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z));
        }
        else if (triggerCol is CapsuleCollider cc)
        {
            effectiveRadius = Mathf.Max(cc.radius, cc.height * 0.5f) * Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z));
        }
        else if (triggerCol is BoxCollider bc)
        {
            effectiveRadius = (bc.size.magnitude * 0.5f) * Mathf.Max(transform.lossyScale.x, Mathf.Max(transform.lossyScale.y, transform.lossyScale.z));
        }
        else
        {
            effectiveRadius = 1f;
        }

        if (effectiveRadius <= 0.0001f)
        { effectiveRadius = 1f; }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!IsEligible(other)) return;
        OnObjectEnter?.Invoke(other.gameObject);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!IsEligibleLoose(other)) return; // loose check so we still raise exit for anything that entered
        OnObjectExit?.Invoke(other.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!IsEligible(other)) return;

        var rb = other.attachedRigidbody;
        if (!rb) return;

        // Optionally ignore objects still "held" (commonly parented to a hand/socket)
        if (requireNotHeld)
        {
            // If you have a specific held flag, check it here.
            // Generic heuristic: if parented under something (and not this zone), treat as held.
            var pickup = other.GetComponentInParent<Transform>();
            if (pickup && rb.transform.parent != null && rb.transform.parent != transform)
                return;

            // If you have an ItemPickup component with a known "IsHeld" flag, you could do:
            // var ip = other.GetComponentInParent<ItemPickup>();
            // if (ip && ip.transform.parent != null) return;
        }

        Vector3 target = attractionPoint.position;
        Vector3 toCenter = target - rb.worldCenterOfMass;
        float dist = toCenter.magnitude;
        if (dist < Mathf.Epsilon) return;

        Vector3 dir = toCenter / Mathf.Max(dist, 0.0001f);

        // Distance falloff (0 near center => 1.0; 1 at edge => 0.0 by default curve)
        float normalized = Mathf.Clamp01(dist / effectiveRadius);
        float falloff = distanceFalloff != null ? distanceFalloff.Evaluate(1f - normalized) : 1f;

        float accel = pullStrength * falloff;
        if (maxAcceleration > 0f) accel = Mathf.Min(accel, maxAcceleration);

        // Apply as acceleration for steady behavior regardless of mass
        rb.AddForce(dir * accel, ForceMode.Acceleration);

        // Snap gently when very close
        if (dist <= snapDistance)
        {
            Vector3 snapTarget = Vector3.Lerp(rb.position, target, Time.deltaTime * snapSpeed);
            rb.MovePosition(snapTarget);
            // Optionally reduce residual spin
            rb.angularVelocity *= 0.8f;
            rb.linearVelocity *= 0.8f;
        }

        // Consume
        if (consumeDistance > 0f && dist <= consumeDistance)
        {
            OnObjectConsumed?.Invoke(other.gameObject);
            if (destroyOnConsume)
            {
                Destroy(rb.gameObject);
            }
            // Otherwise: you can disable it, parent it, etc. in OnObjectConsumed
        }
    }

    private bool IsEligible(Collider other)
    {
        if (!other) return false;
        if (((1 << other.gameObject.layer) & affectedLayers.value) == 0) return false;

        // Must match one of the allowed tags
        if (allowedTags != null && allowedTags.Length > 0)
        {
            bool match = false;
            for (int i = 0; i < allowedTags.Length; i++)
            {
                if (!string.IsNullOrEmpty(allowedTags[i]) && other.CompareTag(allowedTags[i]))
                {
                    match = true;
                    break;
                }
            }
            if (!match) return false;
        }

        // Must have a rigidbody (on self or parent)
        if (!other.attachedRigidbody) return false;

        return true;
    }

    // A looser version used for exit (so we still fire exit for stuff that entered even if tags changed)
    private bool IsEligibleLoose(Collider other)
    {
        if (!other) return false;
        if (!other.attachedRigidbody) return false;
        return true;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw helpful rings
        Transform center = attractionPoint ? attractionPoint : transform;
        Gizmos.matrix = Matrix4x4.identity;
        Gizmos.color = new Color(0.4f, 0.7f, 1f, 0.4f);
        Gizmos.DrawWireSphere(center.position, Mathf.Max(0.05f, effectiveRadius));
        Gizmos.color = new Color(1f, 0.9f, 0.2f, 0.6f);
        Gizmos.DrawWireSphere(center.position, Mathf.Max(0.01f, snapDistance));
        if (consumeDistance > 0f)
        {
            Gizmos.color = new Color(1f, 0.3f, 0.3f, 0.7f);
            Gizmos.DrawWireSphere(center.position, Mathf.Max(0.005f, consumeDistance));
        }
    }
}

