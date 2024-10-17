using UnityEngine;

public class Chair : MonoBehaviour, IInteractable
{
    private bool isOccupied = false;
    private NPCController occupyingNPC;
    private Outline outline;

    public Transform SeatPoint;

    public void Interact(Player player)
    {
        Debug.Log("Chair Interact() called");
    }


    public void EnableOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }


    // Start is called before the first frame update
    private void Start()
    {
        outline = gameObject.GetComponentInChildren<Outline>();
        Debug.Log(outline);
    }


    // Update is called once per frame
    void Update()
    {

    }
}
