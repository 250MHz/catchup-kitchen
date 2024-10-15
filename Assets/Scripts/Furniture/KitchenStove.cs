using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenStove : BaseFurniture, IInteractable
{
    [SerializeField] private UsableObjectSO[] allowedUsableObjectSO;
    private Outline outline;

    public void Interact(Player player)
    {
        // TODO: Should make it so only certain items can be set in `usableObject`
        if (!HasUsableObject())
        {
            // Stove does not have sommething on top of it
            if (player.HasUsableObject())
            {
                UsableObject heldObject = player.GetUsableObject();
                UsableObjectSO heldObjectSO = heldObject.GetUsableObjectSO();
                // If the object the player is holding can be on top of the stove,
                // then make stove the owner of the object
                foreach (UsableObjectSO so in allowedUsableObjectSO)
                {
                    if (heldObjectSO.GetObjectName() == so.GetObjectName())
                    {
                        heldObject.SetUsableObjectParent(this);
                        break;
                    }
                }
            }
        }
        else
        {
            // Stove has something on top of it
            if (player.HasUsableObject())
            {
                // Player is holding something
                // If player is holding on an ingredient and the UsableObject
                // on the cabinet is a pot, try to add the ingredient to the pot
                if (GetUsableObject().TryGetPot(out PotUsableObject potUsableObject))
                {
                    if (potUsableObject.TryAddIngredient(player.GetUsableObject().GetUsableObjectSO()))
                    {
                        player.GetUsableObject().DestroySelf();
                    }
                }
            }
            else
            {
                // Player is not holding something
                // Give the player the object on top of the stove
                GetUsableObject().SetUsableObjectParent(player);
            }
        }
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    // Start is called before the first frame update
    void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
