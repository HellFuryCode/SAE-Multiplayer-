
using UnityEngine;

public class PresentationTable : MonoBehaviour  //megamind
{
    public LayerMask itemMask = ~0;
    public bool RequireNotHeld = true; //must be on the ground not parented to the hands

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & itemMask.value) == 0) return;  //. not ,

        var pickup = other.GetComponentInParent<ItemPickup>();
        if (!pickup) return;

        if (RequireNotHeld && pickup.transform.parent != null) return;

        var drink = pickup.GetComponent<DrinkItem>();
        if (!drink || drink.crafter == null) return;

      GameDirector.Instance?.AddScore(drink.crafter.playerIndex, drink.points);

        Destroy(pickup.gameObject);
    }

    private void OnTriggerStay(Collider other)
    {
        OnTriggerEnter(other);
    }
}


// Awesome UNIQUE Crafting System! (Max Immersion, No Inventory, Hydroneer, Unity Tutorial
//date accessed 2025/9/17
//created by: Code monkey
//created on: 2022
//url: https://www.youtube.com/watch?v=_aC3NVIQ-ok 
//Online Video
//youtube