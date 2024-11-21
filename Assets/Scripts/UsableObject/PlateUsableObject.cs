using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateUsableObject : UsableObject
{
    [SerializeField] private PlateVisual plateVisual;
    [SerializeField] private PlateRecipeSO[] plateRecipeSOArray;
    [SerializeField] private PlateRecipeSO currentFullPlateRecipeSO;

    // Histogram of ingredients currently on the plate
    private Dictionary<UsableObjectSO, int> currentIngredients;

    private void Start()
    {
        currentIngredients = new Dictionary<UsableObjectSO, int>();
    }

    public bool TryFindCompleteRecipe()
    {
        // If currentIngredients completely matches with a PlateRecipeSO,
        // then we have a full recipe completed
        foreach (PlateRecipeSO recipeSO in plateRecipeSOArray)
        {
            if (recipeSO.MatchesFullRecipe(currentIngredients))
            {
                currentFullPlateRecipeSO = recipeSO;
                return true;
            }
        }
        // If we don't fully match something, set it to null
        currentFullPlateRecipeSO = null;
        return false;
    }

    public UsableObjectSO GetCurrentFullPlateRecipeSO()
    {
        return currentFullPlateRecipeSO?.GetOutput();
    }

    public bool TryAddIngredient(UsableObjectSO usableObjectSO)
    {
        foreach (PlateRecipeSO recipeSO in plateRecipeSOArray)
        {
            Dictionary<UsableObjectSO, int> recipeIngredients = recipeSO.GetIngredients();
            // If currentIngredients contains any ingredients not in the recipe,
            // then the recipe isn't reachable with the currentIngredients, or
            // if usableObjectSO is not an ingredient in the recipe, then the
            // recipe is irrelevant. Either case, try the next recipe.
            bool _reachable = recipeIngredients.ContainsKey(usableObjectSO);
            foreach (UsableObjectSO ingredient in currentIngredients.Keys)
            {
                if (!recipeIngredients.ContainsKey(ingredient))
                {
                    _reachable = false;
                    break;
                }
            }
            if (!_reachable)
            {
                continue;
            }
            // Create a Dictionary to see what ingredients we need to
            // complete the recipeSO.
            Dictionary<UsableObjectSO, int> remainingIngredients
                = new Dictionary<UsableObjectSO, int>(recipeIngredients);
            // Subtract any amount from currentIngredients
            foreach ((UsableObjectSO ingredient, int amount) in currentIngredients)
            {
                remainingIngredients[ingredient] -= amount;
            }
            // If usableObjectSO's amount is > 0 in remainingIngredients, then
            // usableObjectSO would help complete a recipe, so we can add it.
            if (remainingIngredients[usableObjectSO] > 0)
            {
                currentIngredients[usableObjectSO]
                    = currentIngredients.GetValueOrDefault(usableObjectSO, 0) + 1;
                plateVisual.UpdateVisual(currentIngredients);
                // Set the complete recipe when we add the last ingredient
                TryFindCompleteRecipe();
                return true;
            }
        }
        return false;
    }

    public override void Interact(Player player)
    {
        if (!player.HasUsableObject())
        {
            SetUsableObjectParent(player);
        }
        else
        {
            // Player is holding a UsableObject
            UsableObject playerHeldObject = player.GetUsableObject();
            // Try to place the held object onto the plate if possible
            if (TryAddIngredient(playerHeldObject.GetUsableObjectSO()))
            {
                // If it's a pot/pan, replace the held object with an empty
                // pot/pan
                playerHeldObject.DestroySelf();
                if (playerHeldObject.TryGetPotPan(out PotPanUsableObject potPanUsableObject))
                {
                    SpawnUsableObject(potPanUsableObject.GetPotPanUsableObjectSO(), player);
                }
            }
        }
    }
}
