using UnityEngine;
using System.Collections;
using System;

public class PooledIngredients : MonoBehaviour
{
    [HideInInspector] public IngredientPool originPool;
    public float returnDelay = 0f;

    private ItemPickup _pickup;

    private void Awake()
    {
        _pickup = GetComponent<ItemPickup>();
    }

    public void ReturnToPool(float delay = 0f)
    {
        if (!originPool)
        {
            gameObject.SetActive(false); return;
        }

        if (delay <= 0f)
        {
            originPool.Return(_pickup);
        }
        else
        {
            StartCoroutine(ReturnRoutine(delay));
        }

    }

    private IEnumerator ReturnRoutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        originPool.Return(_pickup);
    }
}
