using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateVisual : MonoBehaviour
{
    // Same idea as in PotVisual: make different assets in the visual
    // object active depending on state of currentIngredients in
    // PlateUsableObject
    [SerializeField] private GameObject tomatoSoup;
    [SerializeField] private UsableObjectSO tomatoSoupCooked;

    public void UpdateVisual(Dictionary<UsableObjectSO, int> currentIngredients)
    {
        SetAllInactive();
        if (currentIngredients.ContainsKey(tomatoSoupCooked))
        {
            tomatoSoup.SetActive(true);
        }
    }

    private void SetAllInactive()
    {
        GameObject[] objects = {
            tomatoSoup,
        };
        foreach (GameObject o in objects)
        {
            o.SetActive(false);
        }
    }
}
