using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateVisual : MonoBehaviour
{
    // Same idea as in PotVisual: make different assets in the visual
    // object active depending on state of currentIngredients in
    // PlateUsableObject
    // Tomato Soup
    [SerializeField] private GameObject tomatoSoupVisual;
    [SerializeField] private UsableObjectSO tomatoSoupCooked;
    // Salad
    // TODO: when adding burgers, we'd use the same UsableObjectSO,
    // but the visuals should be different
    [SerializeField] private GameObject tomatoChoppedVisual;
    [SerializeField] private UsableObjectSO tomatoChopped;
    [SerializeField] private GameObject lettuceChoppedVisual;
    [SerializeField] private UsableObjectSO lettuceChopped;

    public void UpdateVisual(Dictionary<UsableObjectSO, int> currentIngredients)
    {
        SetAllInactive();
        // TODO: want to rewrite this in a for loop, but not sure how that
        // would work for burger, so hold off for now.
        if (currentIngredients.ContainsKey(tomatoSoupCooked))
        {
            tomatoSoupVisual.SetActive(true);
        }
        if (currentIngredients.ContainsKey(tomatoChopped))
        {
            tomatoChoppedVisual.SetActive(true);
        }
        if (currentIngredients.ContainsKey(lettuceChopped))
        {
            lettuceChoppedVisual.SetActive(true);
        }
    }

    private void SetAllInactive()
    {
        GameObject[] objects = {
            tomatoSoupVisual,
            tomatoChoppedVisual,
            lettuceChoppedVisual,
        };
        foreach (GameObject o in objects)
        {
            o.SetActive(false);
        }
    }
}
