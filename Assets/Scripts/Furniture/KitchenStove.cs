using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenStove : BaseFurniture, IInteractable
{
    [SerializeField] private UsableObjectSO[] allowedUsableObjectSO;
    private Outline outline;

    public void Interact(Player player)
    {
        if (!HasUsableObject())
        {
            // Stove does not have something on top of it
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
                // If the object on the stove is a pot/pan
                if (GetUsableObject().TryGetPotPan(out PotPanUsableObject potPanUsableObject))
                {
                    // If player is holding on an ingredient, try to add it to
                    // the pot/pan
                    if (potPanUsableObject.TryAddIngredient(player.GetUsableObject().GetUsableObjectSO()))
                    {
                        player.GetUsableObject().DestroySelf();
                    }
                    // If the player is holding on a plate, try to take the ingredient
                    // from the pot/pan anad put it in the plate
                    else if (player.GetUsableObject().TryGetPlate(out PlateUsableObject plateUsableObject))
                    {
                        if (plateUsableObject.TryAddIngredient(GetUsableObject().GetUsableObjectSO()))
                        {
                            UsableObject objectOnStove = GetUsableObject();
                            objectOnStove.DestroySelf();
                            // Replace the pot with an empty pot
                            UsableObject.SpawnUsableObject(
                                potPanUsableObject.GetPotPanUsableObjectSO(), this
                            );
                        }
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
