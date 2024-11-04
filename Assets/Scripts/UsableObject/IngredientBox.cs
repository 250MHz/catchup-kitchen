using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientBox : UsableObject
{
    [SerializeField] private Image ingredientImage;
    [SerializeField] private TextMeshProUGUI numberText;

    private UsableObjectSO ingredientSO;
    private int count;

    public void SetIngredientSO(UsableObjectSO ingredientSO)
    {
        this.ingredientSO = ingredientSO;
        UpdateVisual();
    }

    public void SetCount(int count)
    {
        this.count = count;
        UpdateVisual();
    }

    public UsableObjectSO GetIngredientSO()
    {
        return ingredientSO;
    }

    public int GetCount()
    {
        return count;
    }

    private void UpdateVisual()
    {
        ingredientImage.sprite = ingredientSO.GetIcon();
        numberText.text = $"{count}";
    }
}
