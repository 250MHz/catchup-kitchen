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
                // If player is holding a pot/pan...
                if (player.GetUsableObject().TryGetPotPan(out PotPanUsableObject potPanUsableObject))
                {
                    UsableObject objectOnCabinet = GetUsableObject();
                    // Try to add the object on the cabinet into the pot/pan.
                    if (potPanUsableObject.TryAddIngredient(objectOnCabinet.GetUsableObjectSO()))
                    {
                        objectOnCabinet.DestroySelf();
                    }
                    // If the object on the cabinet is a plate, try to fill the
                    // plate with the pot/pan's contents
                    else if (objectOnCabinet.TryGetPlate(out PlateUsableObject plateUsableObject))
                    {
                        if (plateUsableObject.TryAddIngredient(potPanUsableObject.GetUsableObjectSO()))
                        {
                            potPanUsableObject.DestroySelf();
                            UsableObject.SpawnUsableObject(
                                potPanUsableObject.GetPotPanUsableObjectSO(), player
                            );
                        }
                    }

                }
                // If player is holding a plate, try to add the object on the
                // cabinet onto the plate
                else if (player.GetUsableObject().TryGetPlate(out PlateUsableObject plateUsableObject))
                {
                    if (plateUsableObject.TryAddIngredient(GetUsableObject().GetUsableObjectSO()))
                    {
                        UsableObject objectOnCabinet = GetUsableObject();
                        objectOnCabinet.DestroySelf();
                        // If the object was a pot/pan, we need to replace it with an empty pot/pan
                        if (objectOnCabinet.TryGetPotPan(out PotPanUsableObject potPanUsableObject1))
                        {
                            UsableObject.SpawnUsableObject(
                                potPanUsableObject1.GetPotPanUsableObjectSO(), this
                            );
                        }
                    }
                }
                // If the player is holding on an ingredient and the UsableObject
                // on the cabinet is a plate, try to add the ingredient to the plate
                else if (GetUsableObject().TryGetPlate(out plateUsableObject))
                {
                    if (plateUsableObject.TryAddIngredient(player.GetUsableObject().GetUsableObjectSO()))
                    {
                        player.GetUsableObject().DestroySelf();
                    }
                }
                // If player is holding on an ingredient and the UsableObject
                // on the cabinet is a pot/pan, try to add the ingredient to the pot/pan
                else if (GetUsableObject().TryGetPotPan(out potPanUsableObject))
                {
                    if (potPanUsableObject.TryAddIngredient(player.GetUsableObject().GetUsableObjectSO()))
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
