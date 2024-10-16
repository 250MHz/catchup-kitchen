using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenCabinet : BaseFurniture, IInteractable
{
    private Outline outline;

    public void Interact(Player player)
    {
        if (!HasUsableObject())
        {
            // There is no UsableObject here
            if (player.HasUsableObject())
            {
                // Player is carrying something
                player.GetUsableObject().SetUsableObjectParent(this);
            }
            else
            {
                // Player not carrying anything
            }
        }
        else
        {
            // There is a UsableObject here
            if (player.HasUsableObject())
            {
                // Player is carrying something
                // If player is holding a pot, try to add the object on the
                // cabinet into the pot.
                if (player.GetUsableObject().TryGetPot(out PotUsableObject potUsableObject))
                {
                    if (potUsableObject.TryAddIngredient(GetUsableObject().GetUsableObjectSO()))
                    {
                        GetUsableObject().DestroySelf();
                    }
                }
                // If player is holding on an ingredient and the UsableObject
                // on the cabinet is a pot, try to add the ingredient to the pot
                else if (GetUsableObject().TryGetPot(out potUsableObject))
                {
                    if (potUsableObject.TryAddIngredient(player.GetUsableObject().GetUsableObjectSO()))
                    {
                        player.GetUsableObject().DestroySelf();
                    }
                }
            }
            else
            {
                // Player is not carrying anything
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
    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
