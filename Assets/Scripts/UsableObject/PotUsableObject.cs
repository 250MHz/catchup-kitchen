using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering;
using UnityEngine;

public class PotUsableObject : UsableObject
{
    private enum State
    {
        Idle,
        Cooking,
        Cooked,
        Burned,
    }

    [SerializeField] private PotVisual potVisual;
    // Array of possible recipes the pot can handle
    [SerializeField] private CookingRecipeSO[] cookingRecipeSOArray;
    [SerializeField] private ProgressBar progressBar;

    // Histogram of ingredients currently inside the pot
    private Dictionary<UsableObjectSO, int> currentIngredients;
    // Initialize fields here instead of in Start() so we can set the state of
    // the new object created in Update()'s switch-case for Cooking
    private CookingRecipeSO currentFullRecipeSO = null;
    private State state = State.Idle;
    private float cookingTimer;
    private float burningTimer;


    private void Start()
    {
        currentIngredients = new Dictionary<UsableObjectSO, int>();
        cookingTimer = 0f;
    }

    private void Update()
    {
        if (GetUsableObjectParent() is KitchenStove)
        {
            switch (state)
            {
                case State.Idle:
                    state = currentFullRecipeSO != null ? State.Cooking : State.Idle;
                    break;
                case State.Cooking:
                    cookingTimer += Time.deltaTime;
                    progressBar.SetSafeColor();
                    progressBar.SetBarFillAmount(cookingTimer / currentFullRecipeSO.GetCookingTimerMax());
                    if (cookingTimer > currentFullRecipeSO.GetCookingTimerMax())
                    {
                        // Cooked
                        IUsableObjectParent _parent = GetUsableObjectParent();
                        DestroySelf();
                        PotUsableObject newPotUsableObject = SpawnUsableObject(
                            currentFullRecipeSO.GetOutput(),
                            _parent
                        ) as PotUsableObject;
                        newPotUsableObject.state = State.Cooked;
                        newPotUsableObject.burningTimer = 0f;
                        newPotUsableObject.currentFullRecipeSO = currentFullRecipeSO;
                        progressBar.SetBarFillAmount(0f);
                    }
                    break;
                case State.Cooked:
                    burningTimer += Time.deltaTime;
                    progressBar.SetDangerColor();
                    progressBar.SetBarFillAmount(burningTimer / currentFullRecipeSO.GetBurningTimerMax());
                    if (burningTimer > currentFullRecipeSO.GetBurningTimerMax())
                    {
                        // Burned
                        IUsableObjectParent _parent = GetUsableObjectParent();
                        DestroySelf();
                        PotUsableObject newPotUsableObject = SpawnUsableObject(
                            currentFullRecipeSO.GetBurningOutput(),
                            _parent
                        ) as PotUsableObject;
                        newPotUsableObject.state = State.Burned;
                        progressBar.SetBarFillAmount(0f);
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }

    public bool TryFindCompleteRecipe()
    {
        // We already have a match, and since there won't be recipes that
        // overlap, just return early
        if (currentFullRecipeSO != null)
        {
            return true;
        }
        // If currentIngredients completely matches with a CookingRecipeSO,
        // then we have a full recipe completed
        foreach (CookingRecipeSO recipeSO in cookingRecipeSOArray)
        {
            if (recipeSO.MatchesFullRecipe(currentIngredients))
            {
                currentFullRecipeSO = recipeSO;
                return true;
            }
        }
        return false;
    }

    public bool TryAddIngredient(UsableObjectSO usableObjectSO)
    {
        if (currentFullRecipeSO != null)
        {
            return false;
        }
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
                // Set the complete recipe when we add the last ingredient
                TryFindCompleteRecipe();
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
