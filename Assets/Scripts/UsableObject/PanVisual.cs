using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanVisual : PotPanVisual
{
    // Can't think of a better way to do this. Idea is to make
    // different assets in the visual active depending on
    // state of PotPanUsableObject
    [SerializeField] private GameObject pattyRaw;
    [SerializeField] private UsableObjectSO pattyRawSO;

    public override void UpdateVisual(Dictionary<UsableObjectSO, int> currentIngredients)
    {
        pattyRaw.SetActive(currentIngredients.ContainsKey(pattyRawSO));
    }
}
