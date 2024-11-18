using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class IngredientStorage : BaseFurniture, IInteractable
{
    [SerializeField] private UsableObjectSO usableObjectSO;
    [SerializeField] private UsableObjectSO emptyBoxSO;
    [SerializeField] private Image ingredientImage;
    [SerializeField] private TextMeshProUGUI numberText;
    [SerializeField] private int count = 0;

    private Outline outline;

    public void Interact(Player player)
    {
        if (!player.HasUsableObject() && count > 0)
        {
            UsableObject.SpawnUsableObject(usableObjectSO, player);
            count--;
            UpdateVisual();
        }
        else
        {
            if (player.GetUsableObject() is IngredientBox)
            {
                IngredientBox ingredientBox = (IngredientBox)player.GetUsableObject();
                if (ingredientBox.GetIngredientSO() == usableObjectSO)
                {
                    count += ingredientBox.GetCount();
                    UpdateVisual();
                    ingredientBox.DestroySelf();
                    UsableObject.SpawnUsableObject(emptyBoxSO, player);
                }
            }
        }
    }

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
        if (usableObjectSO == null)
        {
            Debug.LogError("UsableObjectSO can't be null");
        }
        ingredientImage.sprite = usableObjectSO.GetIcon();
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        numberText.text = $"{count}";
    }

}
