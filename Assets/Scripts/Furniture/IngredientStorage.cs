using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientStorage : BaseFurniture, IInteractable
{
    [SerializeField] private UsableObjectSO usableObjectSO;
    private Outline outline;

    public void Interact(Player player)
    {
        if (!player.HasUsableObject())
        {
            GameObject usableGameObject = Instantiate(usableObjectSO.GetPrefab());
            usableGameObject.GetComponent<UsableObject>().SetUsableObjectParent(player);
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
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
