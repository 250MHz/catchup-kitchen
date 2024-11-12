using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlateVisual : MonoBehaviour
{
    // Same idea as in PotPanVisual: make different assets in the visual
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

    /* Burger
    With 4 ingredients there are 16 possibilties:
    (buns) (meat) (lettuce) (tomato)
    0000 is empty plate
    -----
    0001
    0010
    0011
    ----- 1-3 are just salad recipe
    Burgers only become relevant >= 4
    0100
    0101
    0110
    0111
    ----- 4-7 don't have a bun
    1000
    1001
    1010
    1011
    1100
    1101
    1110
    ----- 8-14 have a bun
    1111 is complete burger

    Use a different visual for meat / lettuce / tomato in 4-7 and 8-14
    The one we render depends on whether buns are present
    */
    [SerializeField] private UsableObjectSO pattyCooked;
    [SerializeField] private UsableObjectSO burgerBuns;
    [SerializeField] private GameObject pattyNoBunsVisual;
    [SerializeField] private GameObject lettuceNoBunsVisual;
    [SerializeField] private GameObject tomatoNoBunsVisual;
    [SerializeField] private GameObject bunsVisual;
    [SerializeField] private GameObject pattyWithBunsVisual;
    [SerializeField] private GameObject lettuceWithBunsVisual;
    [SerializeField] private GameObject tomatoWithBunsVisual;
    [SerializeField] private GameObject completeBurgerVisual;

    public void UpdateVisual(Dictionary<UsableObjectSO, int> currentIngredients)
    {
        SetAllInactive();
        if (currentIngredients.ContainsKey(tomatoSoupCooked))
        {
            tomatoSoupVisual.SetActive(true);
        }
        else if (currentIngredients.ContainsKey(burgerBuns))
        {
            // Has buns
            if (currentIngredients.ContainsKey(pattyCooked)
                && currentIngredients.ContainsKey(lettuceChopped)
                && currentIngredients.ContainsKey(tomatoChopped))
            {
                // If have everything render the whole burger
                completeBurgerVisual.SetActive(true);
            }
            else
            {
                bunsVisual.SetActive(true);
                // Render the ingredients on top of buns
                pattyWithBunsVisual.SetActive(currentIngredients.ContainsKey(pattyCooked));
                lettuceWithBunsVisual.SetActive(currentIngredients.ContainsKey(lettuceChopped));
                tomatoWithBunsVisual.SetActive(currentIngredients.ContainsKey(tomatoChopped));
            }
        }
        else if (currentIngredients.ContainsKey(pattyCooked))
        {
            // Has patty, but no buns
            pattyNoBunsVisual.SetActive(true);
            lettuceNoBunsVisual.SetActive(currentIngredients.ContainsKey(lettuceChopped));
            tomatoNoBunsVisual.SetActive(currentIngredients.ContainsKey(tomatoChopped));
        }
        else
        {
            // No patty or buns, regular salad
            tomatoChoppedVisual.SetActive(currentIngredients.ContainsKey(tomatoChopped));
            lettuceChoppedVisual.SetActive(currentIngredients.ContainsKey(lettuceChopped));
        }
    }

    private void SetAllInactive()
    {
        GameObject[] objects = {
            tomatoSoupVisual,
            tomatoChoppedVisual,
            lettuceChoppedVisual,
            pattyNoBunsVisual,
            lettuceNoBunsVisual,
            tomatoNoBunsVisual,
            bunsVisual,
            pattyWithBunsVisual,
            lettuceWithBunsVisual,
            tomatoWithBunsVisual,
        };
        foreach (GameObject o in objects)
        {
            o.SetActive(false);
        }
    }
}
