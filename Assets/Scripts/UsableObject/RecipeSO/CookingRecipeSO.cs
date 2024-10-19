using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu()]
public class CookingRecipeSO : ScriptableObject
{
    // Unity doesn't serialize Dictionary, so we map array elements of
    // `inputKeys` with the number of that ingredient in `inputValues`
    [SerializeField] private UsableObjectSO[] inputKeys;
    [SerializeField] private int[] inputValues;
    [SerializeField] private UsableObjectSO output;
    [SerializeField] private UsableObjectSO burningOutput;
    [SerializeField] private float cookingTimerMax;
    [SerializeField] private float burningTimerMax;

    public Dictionary<UsableObjectSO, int> GetIngredients()
    {
        Dictionary<UsableObjectSO, int> ingredients = new Dictionary<UsableObjectSO, int>();
        for (int i = 0; i < inputKeys.Length; i++)
        {
            ingredients.Add(inputKeys[i], inputValues[i]);
        }
        return ingredients;
    }

    public bool MatchesFullRecipe(Dictionary<UsableObjectSO, int> ingredients)
    {
        // TODO: probably more effiicent way to do this
        Dictionary<UsableObjectSO, int> thisRecipesIngredients = GetIngredients();
        if (thisRecipesIngredients.Count != ingredients.Count)
        {
            return false;
        }
        foreach ((UsableObjectSO recipeIngredient, int amount) in thisRecipesIngredients)
        {
            if (ingredients.GetValueOrDefault(recipeIngredient, 0) != amount)
            {
                return false;
            }
        }
        foreach ((UsableObjectSO recipeIngredient, int amount) in ingredients)
        {
            if (thisRecipesIngredients.GetValueOrDefault(recipeIngredient, 0) != amount)
            {
                return false;
            }
        }
        return true;
    }

    public UsableObjectSO GetOutput()
    {
        return output;
    }

    public UsableObjectSO GetBurningOutput()
    {
        return burningOutput;
    }

    public float GetCookingTimerMax()
    {
        return cookingTimerMax;
    }

    public float GetBurningTimerMax()
    {
        return burningTimerMax;
    }
}
