using UnityEngine;

public class Chair : MonoBehaviour, IInteractable
{
    private bool isOccupied = false;
    private NPCController occupyingNPC;
    private Outline outline;

    public void Interact()
    {
        Debug.Log("Chair Interact() called");
        InteractWithChair();
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

    public void OccupyChair(NPCController npc)
    {
        isOccupied = true;
        occupyingNPC = npc;
        Debug.Log("Chair is now occupied.");
    }

    public void FreeChair()
    {
        isOccupied = false;
        occupyingNPC = null;
        Debug.Log("Chair is now free.");
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void InteractWithChair()
    {
        if (isOccupied && occupyingNPC != null)
        {
            occupyingNPC.LeaveChair();  // Tell NPC to leave
            FreeChair();  // Free the chair
        }
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
