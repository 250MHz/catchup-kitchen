using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingBoard : BaseFurniture, IInteractable
{
    [SerializeField] private UsableObjectSO[] allowedUsableObjectSO;
    private Outline outline;

    public void Interact(Player player)
    {
        // TODO: Only certain items here
        if (!HasUsableObject())
        {
            // Cutting board doesn't have something on top of it
            if (player.HasUsableObject())
            {
                UsableObject heldObject = player.GetUsableObject();
                UsableObjectSO heldObjectSO = heldObject.GetUsableObjectSO();
                // If the object the player is holding can be on top of the board,
                // then make board the owner of the object
                foreach (UsableObjectSO so in allowedUsableObjectSO)
                {
                    if (heldObjectSO.GetObjectName() == so.GetObjectName())
                    {
                        heldObject.SetUsableObjectParent(this);
                        break;
                    }
                }
            }
        }
        else
        {
            // Board has something on top of it
            if (player.HasUsableObject())
            {
                // Player is holding something
            }
            else
            {
                // Player is not holding something
                // Give the player the object on top of the board
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
    void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {

    }
}
