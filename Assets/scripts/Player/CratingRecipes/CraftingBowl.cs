
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingBowl : MonoBehaviour
{
    public PlayerIdentity owner;
    [SerializeField] private Image recipeImage;


    [Header("Recipes")]
    public CraftingRecipeSO recipe;
    public BoxCollider bowlTrigger; //zones for whos whpses
    [SerializeField] private LayerMask itemMask = ~0, PickUp; //the layers my items are on for the items specificallly

    //spawn points
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private Transform VFXSpawnItem;

    private readonly HashSet<ItemPickup> contents = new();

    private void Reset()
    {
        bowlTrigger = GetComponent<BoxCollider>();
        if (bowlTrigger) bowlTrigger.isTrigger = true;
    }

    private void Awake()
    {
        if (!bowlTrigger)
        {
            bowlTrigger = GetComponent<BoxCollider>();
        }

        if (bowlTrigger)
        {
            bowlTrigger.isTrigger = true;
        }


        if (recipe && recipeImage)
        {
            recipeImage.sprite = recipe.sprite;
        }
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
        //layer filter
        if (((1 << c.gameObject.layer) & itemMask.value) == 0) return;

        //only count items
        var pickup = c.GetComponentInParent<ItemPickup>();
        if (!pickup || pickup.data == null) return;

        contents.Add(pickup);
        Debug.Log($"[Bowl] + {pickup.data.kind}  (total={contents.Count})");

    }

    public void TryRemove(Collider c)
    {
        var pickup = c.GetComponentInParent<ItemPickup>();
        if (pickup) contents.Remove(pickup);
        if (pickup) Debug.Log($"[Bowl] - {pickup.data.kind}  (total={contents.Count})");
    }

    public void TryAutoCraft() //crafting logic //aka the motherfucker who messes everythibng uuuuuup
    {
        if (!recipe || contents.Count == 0) return;


        var have = new Dictionary<ItemData.ItemKind, int>(); //count what we have
        foreach (var p in contents)
        {
            var k = p.data.kind;

            have[k] = have.TryGetValue(k, out var c) ? c + 1 : 1;

        }


        var need = new Dictionary<ItemData.ItemKind, int>(); //why arent you working.... oh needed a >
        foreach (var k in recipe.inputKinds)
        {
            need[k] = need.TryGetValue(k, out var c) ? c + 1 : 1;
        }


        //check for exact match (no extra stuff or missing things)
        foreach (var kv in need)
        {
            if (!have.TryGetValue(kv.Key, out var c) || c < kv.Value)
            {
                return; //missing something
            }
        }

        //sabtoage
        foreach (var kv in have)
        {
            if (!need.TryGetValue(kv.Key, out var needCount) || kv.Value > needCount) return;  //extra or wrong items present

        }

        ConsumeRequired(new Dictionary<ItemData.ItemKind, int>(need));
        // ConsumeRequired(need);
        SpawnDrink();
        contents.Clear();
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
            if (p)
            {
                Destroy(p.gameObject);
            }
        }
    }

    private void SpawnDrink()
    {
        if (!recipe || !recipe.outputPrefab || !itemSpawnPoint) return;

        var t = Instantiate(recipe.outputPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);

        // Ensures the spawned drink can be picked up and carries points/owner
        var pickup = t.GetComponent<ItemPickup>();
        if (!pickup) pickup = t.gameObject.AddComponent<ItemPickup>();

        var drink = t.GetComponent<DrinkItem>();
        if (!drink) drink = t.gameObject.AddComponent<DrinkItem>();  //remember <>
        drink.crafter = owner;
        drink.points = Mathf.Max(1, recipe.points);

        if (!t.GetComponent<ItemPickup>())
        {
            t.gameObject.AddComponent<ItemPickup>();
        }

        if (VFXSpawnItem) Instantiate(VFXSpawnItem, itemSpawnPoint.position, itemSpawnPoint.rotation);
        Debug.Log("[Bowl] Crafted drink!"); //sanity because im lossing it
    }

    public void SetRecipe(CraftingRecipeSO newRecipe)
    {
        recipe = newRecipe;
        if (recipeImage && recipe) recipeImage.sprite = recipe.sprite;
        contents.Clear();

    }

}

// Awesome UNIQUE Crafting System! (Max Immersion, No Inventory, Hydroneer, Unity Tutorial
//date accessed 2025/9/17
//created by: Code monkey
//created on: 2022
//url: https://www.youtube.com/watch?v=_aC3NVIQ-ok 
//Online Video
//youtube