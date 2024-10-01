using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KitchenSink : BaseFurniture, IInteractable
{
    private Outline outline;

    public void Interact(Player player)
    {
        // TODO: If player is holding a dirty plate take it and replace it with
        // a clean plate
        Debug.Log("Kitchen Sink Interact() called");
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
