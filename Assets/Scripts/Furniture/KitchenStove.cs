using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenStove : BaseFurniture, IInteractable
{
    [SerializeField] private UsableObjectSO potUsableObjectSO;
    [SerializeField] private UsableObjectSO[] allowedUsableObjectSO;
    private Outline outline;

    private AudioSource cookingSound;

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

                        // Check if the held object is a pot
                        if (heldObject.TryGetPot(out PotUsableObject potUsableObject))
                        {
                            // Play cooking sound only if the pot is not empty
                            if (!potUsableObject.IsEmpty() && !cookingSound.isPlaying)
                            {
                                cookingSound.Play();
                            }
                        }

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
                // If the object on the stove is a pot
                if (GetUsableObject().TryGetPot(out PotUsableObject potUsableObject))
                {
                    // If player is holding on an ingredient, try to add it to
                    // the pot
                    if (potUsableObject.TryAddIngredient(player.GetUsableObject().GetUsableObjectSO()))
                    {
                        player.GetUsableObject().DestroySelf();

                        // Play cooking sound if the pot now contains ingredients
                        if (!potUsableObject.IsEmpty() && !cookingSound.isPlaying)
                        {
                            cookingSound.Play();
                        }
                    }
                    // If the player is holding on a plate, try to take the ingredient
                    // from the pot anad put it in the plate
                    else if (player.GetUsableObject().TryGetPlate(out PlateUsableObject plateUsableObject))
                    {
                        if (plateUsableObject.TryAddIngredient(GetUsableObject().GetUsableObjectSO()))
                        {
                            UsableObject objectOnStove = GetUsableObject();
                            objectOnStove.DestroySelf();
                            // Replace the pot with an empty pot
                            UsableObject.SpawnUsableObject(potUsableObjectSO, this);

                            if (cookingSound.isPlaying)
                            {
                                cookingSound.Stop();
                            }

                        }
                    }

                }
            }
            else
            {
                // Player is not holding something
                // Give the player the object on top of the stove
                GetUsableObject().SetUsableObjectParent(player);

                if (cookingSound.isPlaying)
                {
                    cookingSound.Stop();
                }
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
        cookingSound = gameObject.GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
