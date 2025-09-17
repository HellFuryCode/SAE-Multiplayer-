using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Data", fileName = "NewItemData")]


public class ItemData : ScriptableObject
{
    public enum ItemKind { None, Berry, Sprite }
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

