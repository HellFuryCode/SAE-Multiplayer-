using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Data", fileName = "NewItemData")]


public class ItemData : ScriptableObject
{
    public enum ItemKind { None, OrangeSlice, Soda, Apple, WaterMelon, ice }
    public string displayName;
    public Sprite icon;
    public ItemKind kind = ItemKind.None; //for the recipes

    public Vector3 localPosition;     // where the item sits in the hand
    public Vector3 localEulerAngles;  // how it’s rotated in the hand
    public Vector3 localScale = Vector3.one;

    public float dropForwardForce = 4f;
    public float dropUpwardForce = 2f;
    public float randomTorque = 1f;  // spin when tossed
}

// Awesome UNIQUE Crafting System! (Max Immersion, No Inventory, Hydroneer, Unity Tutorial
//date accessed 2025/9/17
//created by: Code monkey
//created on: 2022
//url: https://www.youtube.com/watch?v=_aC3NVIQ-ok 
//Online Video
//youtube
