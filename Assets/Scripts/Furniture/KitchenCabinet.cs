using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenCabinet : BaseFurniture, IInteractable
{
    private Outline outline;

    public void Interact(Player player)
    {
        if (!HasUsableObject())
        {
            // There is no UsableObject here
            if (player.HasUsableObject())
            {
                // Player is carrying something
                player.GetUsableObject().SetUsableObjectParent(this);
            }
            else
            {
                // Player not carrying anything
            }
        }
        else
        {
            // There is a UsableObject here
            if (player.HasUsableObject())
            {
                // Player is carrying something
            }
            else
            {
                // Player is not carrying anything
                GetUsableObject().SetUsableObjectParent(player);
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
    }

    // Update is called once per frame
    private void Update()
    {
    }
}
