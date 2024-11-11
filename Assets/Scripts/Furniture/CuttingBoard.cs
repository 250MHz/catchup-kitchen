using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : BaseFurniture, IInteractable
{
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOs;
    [SerializeField] private ProgressBar progressBar;

    private Outline outline;
    private int cuttingProgress;

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
                    cuttingProgress = 0;
                    // It doesn't matter what value we pass here, just that it's not 0.
                    // cuttingProgress is now 0 so it'll always set the progress to 0
                    UpdateProgressBar(1);
                }
            }
        }
        else
        {
            // Board has something on top of it
            if (player.HasUsableObject())
            {
                // Player is holding something
                // If player is holding a pot/pan, try to add the object on the
                // cutting board into the pot/pan.
                if (player.GetUsableObject().TryGetPotPan(out PotPanUsableObject potPanUsableObject))
                {
                    if (potPanUsableObject.TryAddIngredient(GetUsableObject().GetUsableObjectSO()))
                    {
                        GetUsableObject().DestroySelf();
                    }
                }
                // If player is holding a plate, try to add the object on the
                // cutting board onto the plate
                else if (player.GetUsableObject().TryGetPlate(out PlateUsableObject plateUsableObject))
                {
                    if (plateUsableObject.TryAddIngredient(GetUsableObject().GetUsableObjectSO()))
                    {
                        GetUsableObject().DestroySelf();
                    }
                }
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
                    cuttingProgress++;
                    CuttingRecipeSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(
                        currentUsableObject.GetUsableObjectSO()
                    );
                    int progressMax = cuttingRecipeSO.GetCuttingProgressMax();
                    UpdateProgressBar(progressMax);
                    if (cuttingProgress >= progressMax)
                    {
                        currentUsableObject.DestroySelf();
                        UsableObject.SpawnUsableObject(outputUsableObjectSO, this);
                    }
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

    private void UpdateProgressBar(int max)
    {
        progressBar.SetBarFillAmount((float)cuttingProgress / max);
    }

    private CuttingRecipeSO GetCuttingRecipeSOWithInput(UsableObjectSO input)
    {
        foreach (CuttingRecipeSO so in cuttingRecipeSOs)
        {
            if (so.GetInput() == input)
            {
                return so;
            }
        }
        return null;
    }

    private UsableObjectSO GetOutputForInput(UsableObjectSO input)
    {
        return GetCuttingRecipeSOWithInput(input)?.GetOutput();
    }

    private bool HasRecipeWithInput(UsableObjectSO input)
    {
        return GetCuttingRecipeSOWithInput(input) != null;
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
