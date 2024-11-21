using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecipeUI : MonoBehaviour
{
    [SerializeField] private GameObject tomatoRecipe;
    [SerializeField] private GameObject saladRecipe;
    [SerializeField] private GameObject burgerRecipe;

    public void Start()
    {
        EnableTomatoRecipe();
    }

    private void DisableAllRecipes()
    {
        tomatoRecipe.gameObject.SetActive(false);
        saladRecipe.gameObject.SetActive(false);
        burgerRecipe.gameObject.SetActive(false);
    }

    public void EnableTomatoRecipe()
    {
        DisableAllRecipes();
        tomatoRecipe.SetActive(true);
    }

    public void EnableSaladRecipe()
    {
        DisableAllRecipes();
        saladRecipe.SetActive(true);
    }

    public void EnableBurgerRecipe()
    {
        DisableAllRecipes();
        burgerRecipe.SetActive(true);
    }
}
