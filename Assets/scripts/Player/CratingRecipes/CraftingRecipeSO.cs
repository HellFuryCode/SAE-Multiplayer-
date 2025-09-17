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
