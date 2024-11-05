using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientShop : BaseFurniture, IInteractable
{
    // TODO: probably should've created a new SO just for shop related stuff.
    // Description / icon / price doesn't apply to the majority of usable objects.
    [SerializeField] private List<UsableObjectSO> availableItems;
    [SerializeField] private Dialog dialog;
    [SerializeField] private UsableObjectSO usableObjectSO;
    [SerializeField] private Transform itemSpawnPoint;
    [SerializeField] private ClerkVisual clerk;

    private Outline outline;

    public void Interact(Player player)
    {
        clerk.transform.LookAt(player.transform);
        player.GetShopController().StartShopping(this);
    }

    public void HandlePurchase(Player player, UsableObjectSO ingredient, int count)
    {
        clerk.HandOff();

        GameObject usableGameObject = Instantiate(usableObjectSO.GetPrefab());
        IngredientBox ingredientBox = (IngredientBox)usableGameObject
            .GetComponent<UsableObject>();
        ingredientBox.SetIngredientSO(ingredient);
        ingredientBox.SetCount(count);

        if (!player.HasUsableObject())
        {
            ingredientBox.SetUsableObjectParent(player);
        }
        else
        {
            // Spawn next to shop if player is already holding something
            usableGameObject.transform.position = itemSpawnPoint.position;
        }
    }

    public List<UsableObjectSO> AvailableItems => availableItems;

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    // Start is called before the first frame update
    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
