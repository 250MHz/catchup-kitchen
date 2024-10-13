using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : BaseFurniture, IInteractable
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOs;

    private Outline outline;

    public void Interact(Player player)
    {
        // TODO: Only certain items here
        if (!HasUsableObject())
        {
            // Cutting board doesn't have something on top of it
            if (player.HasUsableObject())
            {
                // If the object the player is holding can be on top of the board,
                // then make board the owner of the object
                UsableObject heldObject = player.GetUsableObject();
                if (HasRecipeWithInput(heldObject.GetUsableObjectSO()))
                {
                    heldObject.SetUsableObjectParent(this);
                }
            }
        }
        else
        {
            // Board has something on top of it
            if (player.HasUsableObject())
            {
                // Player is holding something
            }
            else
            {
                // Player is not holding something
                UsableObject currentUsableObject = GetUsableObject();
                // If the current object can be cut, then cut it
                UsableObjectSO outputUsableObjectSO = GetOutputForInput(
                    currentUsableObject.GetUsableObjectSO()
                );
                if (outputUsableObjectSO != null)
                {
                    currentUsableObject.DestroySelf();
                    UsableObject.SpawnUsableObject(outputUsableObjectSO, this);
                }
                else
                {
                    // Else, the current object is already cut, so make the
                    // player hold it.
                    currentUsableObject.SetUsableObjectParent(player);
                }
            }
        }
    }

    private UsableObjectSO GetOutputForInput(UsableObjectSO input)
    {
        foreach (CuttingRecipeSO so in cuttingRecipeSOs)
        {
            if (so.GetInput() == input)
            {
                return so.GetOutput();
            }
        }
        return null;
    }

    private bool HasRecipeWithInput(UsableObjectSO input)
    {
        return GetOutputForInput(input) != null;
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
