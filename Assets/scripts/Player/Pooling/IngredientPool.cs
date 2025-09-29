using UnityEngine;
using System.Collections.Generic;

public class IngredientPool : MonoBehaviour
{
    public ItemPickup prefab;
    public int initialSize = 16;
    public bool allowExpand = true;

    private readonly Queue<ItemPickup> _pool = new();
    public int ActiveCount { get; private set; }

    public delegate void ItemEvent(ItemPickup item);
    public event ItemEvent OnSpawned;
    public event ItemEvent OnReturned;

    private void Awake()
    {
        if (!prefab)
        {
            Debug.LogError("[IngredientPool] Missing Prefab");
            enabled = false; return;
        }

        for (int i = 0; i < initialSize; i++)
        {
            _pool.Enqueue(CreateNew());
        }
    }

    ItemPickup CreateNew()
    {
        var it = Instantiate(prefab, transform);
        it.gameObject.SetActive(false);

        var pooled = it.GetComponent<PooledIngredients>() ?? it.gameObject.AddComponent<PooledIngredients>();
        pooled.originPool = this;

        return it;
    }

    public ItemPickup SpawnAt(Vector3 pos, Quaternion rot)
    {
        ItemPickup it = _pool.Count > 0 ? _pool.Dequeue() :
            allowExpand ? CreateNew() : null;

        if (!it) return null;

        var rb = it.rb ? it.rb : GetComponent<Rigidbody>();
        if (rb)
        {
            rb.linearVelocity = Vector3.zero; rb.angularVelocity = Vector3.zero;
        }

        it.transform.SetParent(null);
        it.transform.SetPositionAndRotation(pos, rot);
        it.gameObject.SetActive(true);
        ActiveCount++;
        OnSpawned?.Invoke(it);
        return it;
    }


    public void Return(ItemPickup it)
    {
        if (!it) return;
        it.gameObject.SetActive(false);
        it.transform.SetParent(transform);
        _pool.Enqueue(it);
        ActiveCount = Mathf.Max(0, ActiveCount - 1);
        OnReturned?.Invoke(it);
    }


}

// How To Make A HORROR Game In Unity | Basic Health System | Horror Series Part 024
//date accessed 2025/9/17
//created by: User1 Productions
//created on: 2022
//url: https://www.youtube.com/watch?v=qRgKB8l9GIg&list=PLlcgaDpDEvw05IgKGZo9FYA8Fo38RtAqH&index=24
//Online Video
//youtube

// Awesome UNIQUE Crafting System! (Max Immersion, No Inventory, Hydroneer, Unity Tutorial
//date accessed 2025/9/17
//created by: Code monkey
//created on: 2022
//url: https://www.youtube.com/watch?v=_aC3NVIQ-ok 
//Online Video
//youtube
