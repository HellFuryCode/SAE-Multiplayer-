using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(menuName = "Crafting/Recipe", fileName = "Recipe_")]
public class CraftingRecipeSO : ScriptableObject
{
    public Sprite sprite;
    public List<ItemSOHolder> inputItemSOlist;
    public ItemSOHolder outputItemSO;





}
