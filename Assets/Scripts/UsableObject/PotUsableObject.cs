using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotUsableObject : UsableObject
{
    [SerializeField] private PotVisual potVisual;
    // Array of possible recipes the pot can handle
    [SerializeField] private CookingRecipeSO[] cookingRecipeSOArray;
    // Histogram of ingredients currently inside the pot
    private Dictionary<UsableObjectSO, int> currentIngredients;

    private void Start()
    {
        currentIngredients = new Dictionary<UsableObjectSO, int>();
    }

    public bool TryAddIngredient(UsableObjectSO usableObjectSO)
    {
        foreach (CookingRecipeSO recipeSO in cookingRecipeSOArray)
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
                potVisual.UpdateVisual(currentIngredients);
                // Debug.Log("TryAddIngredient true");
                return true;
            }
        }
        // Debug.Log("TryAddIngredient false");
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
            // If holding a plate, try to take food out from the pot
            // Otherwise, place the UsableObject in the pot if possible
            UsableObject playerHeldObject = player.GetUsableObject();
            if (TryAddIngredient(playerHeldObject.GetUsableObjectSO()))
            {
                playerHeldObject.DestroySelf();
            }
        }
    }
}
