using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInteract : MonoBehaviour
{
   

        public enum AimReference
        {
            PlayerForward,      // use transform.forward flattened to XZ
            CameraFlattened     // use lookOrigin.forward flattened to XZ (good if cam is static)
        }  


        public Transform lookOrigin;     // player
        public Transform holdSocket;     // where items attach in-hand
        public LayerMask pickupMask = ~0;


        [Header("Interaction")]
         public AimReference aim = AimReference.PlayerForward;
        public float originHeightOffset = 1.0f;   // start cast from chest-ish height
        public float interactRange = 2.5f;
        public float sphereRadius   = 0.35f;
         public float interactCooldown = 0.15f;

        public bool showGizmosAlways = false;
         public bool debugLogs = false;

         private float lastInteractTime;
         private ItemPickup held;
        private Rigidbody playerRb;

        private void Awake()
        {
            playerRb = GetComponent<Rigidbody>();
            if (!lookOrigin) lookOrigin = transform; // fallback
            if (!holdSocket)
            {
                Debug.LogWarning("[PlayerInteract] No holdSocket assigned.");
            }
        }


    public void OnInteract(InputValue input)
        {
            if (!input.isPressed) return;
            if (Time.time < lastInteractTime + interactCooldown) return;
            lastInteractTime = Time.time;

            if (held == null)
            {
                if (TryFindPickup(out ItemPickup pickup))
                {
                    held = pickup;
                    held.PickUp(holdSocket);
                    if (debugLogs) Debug.Log($"[PlayerInteract] Picked up: {held.data?.displayName ?? held.name}");
                }
                else if (debugLogs)
                {
                    Debug.Log($"[PlayerInteract] No pickup in reach (range {interactRange}, radius {sphereRadius}, mask {pickupMask.value}).");
                }
            }
            else
            {
                Vector3 dir = GetAimDirection();
                held.Drop(playerRb ? playerRb.linearVelocity : Vector3.zero, dir, Vector3.up);
                if (debugLogs) Debug.Log($"[PlayerInteract] Dropped.");
                held = null;
            }
        }

     private bool TryFindPickup(out ItemPickup pickup)
        {
            pickup = null;

            Vector3 origin = transform.position + Vector3.up * originHeightOffset;
            Vector3 dir = GetAimDirection();
             Ray ray = new Ray(origin, dir);

             var hits = Physics.SphereCastAll(ray, sphereRadius, interactRange, pickupMask, QueryTriggerInteraction.Ignore);
            if (hits.Length == 0) return false;

            float bestDist = float.MaxValue;
            ItemPickup best = null;
            foreach (var h in hits)
            {
                var cand = h.collider.GetComponentInParent<ItemPickup>();
                if (!cand) continue;
                if (h.distance < bestDist) { bestDist = h.distance; best = cand; }
            }
            if (best != null) { pickup = best; return true; }
            return false;
        }


           private Vector3 GetAimDirection()
        {
            Vector3 fwd = (aim == AimReference.CameraFlattened && lookOrigin ? lookOrigin.forward : transform.forward);
            fwd.y = 0f;
            if (fwd.sqrMagnitude < 0.0001f) fwd = transform.forward;
            return fwd.normalized;
        }
        private void OnDrawGizmosSelected()
        {
            if (!lookOrigin) return;
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(lookOrigin.position + lookOrigin.forward * interactRange, sphereRadius);
            Gizmos.DrawLine(lookOrigin.position, lookOrigin.position + lookOrigin.forward * interactRange);
        }

}
