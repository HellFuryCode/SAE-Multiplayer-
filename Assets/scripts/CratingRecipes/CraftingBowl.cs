using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CraftingBowl : MonoBehaviour
{

    [SerializeField] private Image recipeImage;
    [SerializeField] private List<CraftingRecipeSO> craftingRecipeList;
    [SerializeField] private BoxCollider placeItemsAreaBoxCollider;
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private Transform VFXSpawnItem;

    private CraftingRecipeSO craftingRecipeSo;

    private void Awake()
    {
        NextRecipe();
    }

    public void NextRecipe()
    {
        if (craftingRecipeSo == null)
        {
            craftingRecipeSo = craftingRecipeList[0];
        }

        else
        {
            int index = craftingRecipeList.IndexOf(craftingRecipeSo);
            index = (index + 1) % craftingRecipeList.Count;
            craftingRecipeSo = craftingRecipeList[index];
        }

        recipeImage.sprite = craftingRecipeSo.sprite;
    }

    public void Craft()
    {
        Debug.Log("Crafts");
        Collider[] colliderArray = Physics.OverlapBox(
            transform.position + placeItemsAreaBoxCollider.center,
            placeItemsAreaBoxCollider.size,
            placeItemsAreaBoxCollider.transform.rotation);


        List<ItemSOHolder> inputItemList = new List<ItemSOHolder>(craftingRecipeSo.inputItemSOlist);
        List<GameObject> consumeItemGameObjectList = new List<GameObject>();

        foreach (Collider collider in colliderArray)
        {
            Debug.Log(collider);
            if (collider.TryGetComponent(out ItemSOHolder itemSOHolder))
            {
                if (inputItemList.Contains(itemSOHolder.itemSO))
                {
                    inputItemList.Remove(itemSOHolder.itemSO);
                    consumeItemGameObjectList.Add(collider.gameObject);
                }
            }
        }

        if (inputItemList.Count == 0)
        {
            Transform spawnedItemTransform =
             Instantiate(craftingRecipeSo.outputItemSO.prefab, itemSpawnPoint.position, itemSpawnPoint.rotation);

            Instantiate(VFXSpawnItem, itemSpawnPoint.position, itemSpawnPoint.rotation);


            foreach (GameObject consumeItemGameObject in consumeItemGameObjectList)
            {
                Destroy(consumeItemGameObject);
            }

        }
     
      
    }
}
