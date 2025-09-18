using UnityEngine;

public class KillZone : MonoBehaviour
{
    public float ingredientReturnDelay = 2.5f;

    public float playerRespawnYOffeset = 0.5f; //to avoid clipping and insanity

    private void OnTriggerEnter(Collider other)
    {
        //what ingredient
        var pooled = other.GetComponentInParent<PooledIngredients>();
        if (pooled)
        {
            pooled.ReturnToPool(ingredientReturnDelay);
            return; //sanity guard
        }

        //what player
        var pId = other.GetComponentInParent<PlayerIdentity>();
        var rb = other.GetComponentInParent<Rigidbody>();
        var pr = other.GetComponentInParent<PlayerRespawn>();
        if (pId && pr)
        {
            pr.RespawnNow(playerRespawnYOffeset);
            if (rb)
            {
                rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero;
            }
        }
    }
}


