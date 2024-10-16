using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrashCan : MonoBehaviour, IInteractable
{
    private Outline outline;

    public void Interact(Player player)
    {
        if (player.HasUsableObject())
        {
            UsableObject usableObject = player.GetUsableObject();
            player.ClearUsableObject();
            Destroy(usableObject.gameObject);
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
