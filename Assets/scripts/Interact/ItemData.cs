using UnityEngine;

[CreateAssetMenu(menuName = "Items/Item Data", fileName = "NewItemData")]
public class ItemData : ScriptableObject
{
    [Header("Identity")]
    public string displayName;
    public Sprite icon;

    [Header("Hold Placement")]
    public Vector3 localPosition;     // where the item sits in the hand
    public Vector3 localEulerAngles;  // how it’s rotated in the hand
    public Vector3 localScale = Vector3.one;

    [Header("Drop Behaviour")]
    public float dropForwardForce = 4f;
    public float dropUpwardForce  = 2f;
    public float randomTorque = 10f;  // spin when tossed
}

