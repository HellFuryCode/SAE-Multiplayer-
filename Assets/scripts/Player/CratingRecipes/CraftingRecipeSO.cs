using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Crafting/Recipe", fileName = "Recipe_")]
public class CraftingRecipeSO : ScriptableObject
{
    public Sprite sprite;
    public List<ItemData.ItemKind> inputKinds = new();
    public Transform outputPrefab;  // prefab with ItemPickup + ItemData

    public int points = 1; //points per recipe



}

// Awesome UNIQUE Crafting System! (Max Immersion, No Inventory, Hydroneer, Unity Tutorial
//date accessed 2025/9/17
//created by: Code monkey
//created on: 2022
//url: https://www.youtube.com/watch?v=_aC3NVIQ-ok 
//Online Video
//youtube