using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CraftingBowl : MonoBehaviour
{

    public PlayerIdentity owner;

    [Header("Recipes")]
    public CraftingRecipeSO recipe;
    [SerializeField] private Image recipeImage;

    //spawn points
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private Transform VFXSpawnItem;

    [Tooltip("recipes")]
    public BoxCollider bowlTrigger; //zones for whos whpses
    [SerializeField] private LayerMask itemMask = ~0; //the layers my items are on for the items specificallly

    private readonly HashSet<ItemPickup> contents = new();

    private void Reset()
    {
        bowlTrigger = GetComponent<BoxCollider>();
        if (bowlTrigger) bowlTrigger.isTrigger = true;
    }

    private void Awake()
    {
        if (!bowlTrigger) bowlTrigger = GetComponent<BoxCollider>();

        if (bowlTrigger) bowlTrigger.isTrigger = true;

        if (recipe && recipeImage) recipeImage.sprite = recipe.sprite;
    }

    private void OnTriggerEnter(Collider other)
    {
        TryAdd(other);
        TryAutoCraft();
    }

    void OnTriggerExit(Collider other)
    {
        TryRemove(other);
    }

    public void TryAdd(Collider c)
    {
        if (((1 << c.gameObject.layer) & itemMask.value) == 0) return;
        var pickup = c.GetComponentInParent<ItemPickup>();
        if (!pickup || pickup.data == null) return;
        {
            contents.Add(pickup);
        }
    }

    public void TryRemove(Collider c)
    {
        var pickup = c.GetComponentInParent<ItemPickup>();
        if (pickup) contents.Remove(pickup);
    }

    public void TryAutoCraft()
    {
        if (!recipe || contents.Count < recipe.inputKinds.Count) return;

        var havekinds = new Dictionary<ItemData.ItemKind, int>();

        foreach (var p in contents)
        {
            var k = p.data.kind;
            if (!havekinds.ContainsKey(k)) havekinds[k] = 0;
            {
                havekinds[k]++;
            }
        }

        var need = new Dictionary<ItemData.ItemKind, int>(); //why arent you working.... oh needed a >
        foreach (var k in recipe.inputKinds)
        {
            if (!need.ContainsKey(k)) need[k] = 0;
            {
                need[k]++;
            }
        }

        //check for exact match (no extra stuff or missing things)
        foreach (var kv in need)
        {
            if (!havekinds.TryGetValue(kv.Key, out var count) || count < kv.Value)
            {
                return;
            }
        }

        //sabtoage
        foreach (var kv in havekinds)
        {
            if (!need.TryGetValue(kv.Key, out var needCount) || kv.Value > needCount)
            {
                return;  //extra or wrong items present
            }
        }

    }

    private void ConsumeRequired(Dictionary<ItemData.ItemKind, int> need)
    {
        var toRemove = new List<ItemPickup>();
        foreach (var p in contents)
        {
            var k = p.data.kind;
            if (need.TryGetValue(k, out var remaining) && remaining > 0)
            {
                need[k] = remaining - 1;
                toRemove.Add(p);
            }
        }

        foreach (var p in toRemove)
        {
            contents.Remove(p);
            Destroy(p.gameObject);
        }
    }

    private void SpawnDrink()
    {
        if (!recipe || !recipe.outputPrefab || !itemSpawnPoint) return;

        var t = Instantiate(recipe.outputPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);

        var drink = t.gameObject.GetComponent<DrinkItem>();
        if (!drink) drink = t.gameObject.AddComponent<DrinkItem > ();  //remember <>
        drink.crafter = owner;
        drink.points = Mathf.Max(1, recipe.points);

        if (VFXSpawnItem)
        {
            Instantiate(VFXSpawnItem, itemSpawnPoint.position, itemSpawnPoint.rotation);
        }
    }
    

}