using UnityEngine;
using Unity.Netcode;

public class KillZone : MonoBehaviour
{
    [Header("Ingredients")]
    public float ingredientReturnDelay = 2.5f;

    [Header("Players")]
    public float playerRespawnYOffset = 2.0f;  // avoid clipping on respawn

    // Is networking active at all? bitch
    private static bool IsNetActive =>
        NetworkManager.Singleton != null && NetworkManager.Singleton.IsListening;

   
    private static bool ShouldProcess =>
        !IsNetActive || NetworkManager.Singleton.IsServer;

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("[KillZone] Triggered by: " + other.name);

        if (!ShouldProcess) return;

        // 1) INGREDIENTS (pooled)
        var pooled = other.GetComponentInParent<PooledIngredients>();
        if (pooled)
        {
            pooled.ReturnToPool(ingredientReturnDelay);
            return; // done
        }

        // 2) PLAYERS — preferred via health component (lets you do death VFX, etc.)
        var hp = other.GetComponentInParent<HealthRespawnNet>();
        if (hp)
        {
            // Big damage to trigger death -> HealthRespawnNet will call respawn
            hp.Damage(9999);
            return;
        }

        // 3) PLAYERS — fallback: direct respawn script
        var pr = other.GetComponentInParent<PlayerRespawn>();
        if (pr)
        {
            pr.RespawnNow(playerRespawnYOffset);
            var rb = pr.GetComponent<Rigidbody>();
            if (rb) { rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero; }
        }
    }

    
}
