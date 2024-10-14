using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu()]
public class CookingRecipeSO : ScriptableObject
{
    // Unity doesn't serialize Dictionary, so we map array elements of
    // `inputKeys` with the number of that ingredient in `inputValues`
    [SerializeField] private UsableObjectSO[] inputKeys;
    [SerializeField] private int[] inputValues;
    [SerializeField] private UsableObjectSO output;
    [SerializeField] private float cookingTimerMax;

    public Dictionary<UsableObjectSO, int> GetIngredients()
    {
        Dictionary<UsableObjectSO, int> ingredients = new Dictionary<UsableObjectSO, int>();
        for (int i = 0; i < inputKeys.Length; i++) {
            ingredients.Add(inputKeys[i], inputValues[i]);
        }
        return ingredients;
    }

    public UsableObjectSO GetOutput()
    {
        return output;
    }

    public float GetCookingTimerMax()
    {
        return cookingTimerMax;
    }
}
