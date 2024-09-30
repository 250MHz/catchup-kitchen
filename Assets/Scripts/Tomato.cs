using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tomato : MonoBehaviour, IInteractable
{
    private Outline outline;

    public void Interact(Player player)
    {
        Debug.Log("Tomato Interact() called");
    }

    public IInteractable GetOutlineableObject()
    {
        return null;
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
