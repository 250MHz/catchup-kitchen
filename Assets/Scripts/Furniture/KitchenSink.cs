using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenSink : BaseFurniture, IInteractable
{
    [SerializeField] private UsableObjectSO plateDirtySO; // input
    [SerializeField] private UsableObjectSO plateSO; // output
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private int cleaningProgressMax = 6;

    private Outline outline;
    private int cleaningProgress;

    public void Interact(Player player)
    {
        if (!HasUsableObject())
        {
            // Sink is empty
            if (player.HasUsableObject())
            {
                // If the object the player is holding is a dirty plate, then
                // make the sink the owner of the dirty plate
                UsableObject heldObject = player.GetUsableObject();
                if (heldObject.GetUsableObjectSO() == plateDirtySO)
                {
                    heldObject.SetUsableObjectParent(this);
                    cleaningProgress = 0;
                    UpdateProgressBar();
                }
            }
        }
        else
        {
            // Sink has a dirty plate in it
            if (!player.HasUsableObject())
            {
                // Player is not holding anything
                UsableObject currentUsableObject = GetUsableObject();
                // If plate is dirty, clean it
                if (currentUsableObject.GetUsableObjectSO() == plateDirtySO)
                {
                    cleaningProgress++;
                    UpdateProgressBar();
                    if (cleaningProgress >= cleaningProgressMax)
                    {
                        currentUsableObject.DestroySelf();
                        // Once clean, make the player hold a clean plate
                        UsableObject.SpawnUsableObject(plateSO, player);
                    }
                }
            }
        }
    }

    private void UpdateProgressBar()
    {
        progressBar.SetBarFillAmount((float)cleaningProgress / cleaningProgressMax);
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
