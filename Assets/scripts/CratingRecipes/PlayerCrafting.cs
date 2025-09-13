using System.Runtime.CompilerServices;
using UnityEngine;
using System.Collections;

public class PlayerCrafting : MonoBehaviour
{
    [SerializeField] private Transform playerCamTransform;
    [SerializeField] private LayerMask interactLayermask;


    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButton(0))
        {
        
            if (Physics.Raycast(playerCamTransform.position, playerCamTransform.forward, out RaycastHit raycastHit, interactLayermask))
                if (raycastHit.transform.TryGetComponent(out CraftingBowl craftingBowl))
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        craftingBowl.NextRecipe();
                    }

                    if (Input.GetMouseButtonDown(0))
                    {
                        craftingBowl.Craft();
                    }
            }
        }
    }



}


