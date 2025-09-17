using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

public class CraftingBowl : MonoBehaviour
{
        //ui duh
    [SerializeField] private Image recipeImage;

      [Tooltip("recipes")]
    [SerializeField] private List<CraftingRecipeSO> recipes = new();


    //where to find the items
    [SerializeField] private BoxCollider placeItemsAreaBoxCollider;  //bowl zone not trigger
    [SerializeField] private LayerMask itemMask = ~0; //the layers my items are on for the items specificallly

    //spawn points
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private Transform VFXSpawnItem;
 
    int _recipeIndex = 0;


/// <summary>
/// /////////////////////////////////////////////////////////////////////////////////////////////
/// </summary>

    private void Awake()
    {
        if (recipes.Count > 0 && recipeImage)
        {
            recipeImage.sprite = recipes[_recipeIndex].sprite;
        }
            if (!placeItemsAreaBoxCollider)
                {
                    Debug.LogWarning("[CraftingBowl: placeItemsAreaboxcollider isnt set");
                }
    }

    public void NextRecipe()
    {
        if (recipes.Count == 0) return;
        _recipeIndex = (_recipeIndex + 1) % recipes.Count;
        if (recipeImage) recipeImage.sprite = recipes[_recipeIndex].sprite;
    }

    public void Craft()
    {
        if (recipes.Count == 0 || !placeItemsAreaBoxCollider) return;


        Vector3 centerWS = placeItemsAreaBoxCollider.transform.TransformPoint(placeItemsAreaBoxCollider.center);
        Vector3 halfExt = Vector3.Scale(placeItemsAreaBoxCollider.size, placeItemsAreaBoxCollider.transform.lossyScale) * 0.5f;
     
        Quaternion rot = placeItemsAreaBoxCollider.transform.rotation;

        Collider[] hits = Physics.OverlapBox(centerWS, halfExt, rot, itemMask, QueryTriggerInteraction.Ignore);

        // Collect kinds in bowl (unique per item)
        List<ItemData.ItemKind> haveKinds = new();
        List<GameObject> toConsume = new();


        foreach (var h in hits)
        {
            var pickup = h.GetComponentInParent<ItemPickup>();
            if (!pickup || pickup.data == null) continue;
            if (toConsume.Contains(pickup.gameObject)) continue;


            haveKinds.Add(pickup.data.kind);
             toConsume.Add(pickup.gameObject); //yum yum
        }


        var recipe = recipes[_recipeIndex];

        if (!MatchKinds(haveKinds, recipe.inputKinds))
        {
            return;
        }

            foreach (var go in toConsume) Destroy(go);  //consume yum yum

            if (recipe.outputPrefab && itemSpawnPoint)
            {
                Instantiate(recipe.outputPrefab, itemSpawnPoint.position, itemSpawnPoint.rotation);

            }

            if (VFXSpawnItem)
            {
                Instantiate(VFXSpawnItem, itemSpawnPoint.position, itemSpawnPoint.rotation);
            }
        

    }


    bool MatchKinds(List<ItemData.ItemKind> have, List<ItemData.ItemKind> need)
    {

        if (have.Count < need.Count) return false;


        var counts = new Dictionary<ItemData.ItemKind, int>();

        foreach (var k in have)
        {
            counts[k] = counts.TryGetValue(k, out var c) ? c + 1 : 1;
        }

        foreach (var k in need)
        {
            if (!counts.TryGetValue(k, out var c) || c == 0) return false;
            counts[k] = c - 1;
        }
        return true;
    }

#if UNITY_EDITOR
    void OnDrawGizmosSelected()
    {
        if (!placeItemsAreaBoxCollider) return;
        Gizmos.color = new Color(0, 1, 1, 0.25f); //colour

        Vector3 centerWS = placeItemsAreaBoxCollider.transform.TransformPoint(placeItemsAreaBoxCollider.center);
        Vector3 sizeWS = Vector3.Scale(placeItemsAreaBoxCollider.size, placeItemsAreaBoxCollider.transform.lossyScale);
        Gizmos.matrix = Matrix4x4.TRS(centerWS, placeItemsAreaBoxCollider.transform.rotation, sizeWS);
        Gizmos.DrawCube(Vector3.zero, Vector3.one); //shape
        Gizmos.matrix = Matrix4x4.identity;
    }
#endif
}

