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

// Unity Object Pooling Made Easy: Learn to Manage Spawns Like a Pro 
//date accessed 2025/9/17
//created by: Sasquatch B studios
//created on: 2023
//url: https://www.youtube.com/watch?v=9O7uqbEe-xc&list=PLM_ItQtRF47HNo9Ddkviol9mlIMRcMNTQ&index=2 
//Online Video
//youtube