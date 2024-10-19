using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotVisual : MonoBehaviour
{
    // Can't think of a better way to do this. Idea is to make
    // different assets in the visual active depending on
    // state of currentIngredients in PotUsableObject
    [SerializeField] private GameObject tomatoSoup1;
    [SerializeField] private GameObject tomatoSoup2;
    [SerializeField] private GameObject tomatoSoup3;
    [SerializeField] private UsableObjectSO tomatoChoppedSO;

    public void UpdateVisual(Dictionary<UsableObjectSO, int> currentIngredients)
    {
        SetAllInactive();
        if (currentIngredients.ContainsKey(tomatoChoppedSO))
        {
            GameObject[] tomatoSoups = { tomatoSoup1, tomatoSoup2, tomatoSoup3 };
            tomatoSoups[currentIngredients[tomatoChoppedSO] - 1].SetActive(true);
        }
    }

    private void SetAllInactive()
    {
        GameObject[] objects = {
            tomatoSoup1,
            tomatoSoup2,
            tomatoSoup3
        };
        foreach (GameObject o in objects)
        {
            o.SetActive(false);
        }
    }
}
