using UnityEngine;

public class Chair : MonoBehaviour, IInteractable
{
    private bool isOccupied = false;
    private NPCController occupyingNPC;
    private Outline outline;

    public void Interact(Player player)
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
        Debug.Log("OccupyChair called. Chair is now occupied by: " + npc.name);
        Debug.Log("Chair status - isOccupied: " + isOccupied + ", occupyingNPC: " + occupyingNPC.name);
    }

    public void FreeChair()
    {
        isOccupied = false;
        occupyingNPC = null;
        Debug.Log("FreeChair called. Chair is now free.");
    }

    public bool IsOccupied()
    {
        return isOccupied;
    }

    public void InteractWithChair()
    {
        Debug.Log("Interaction with chair attempted. isOccupied: " + isOccupied + ", occupyingNPC: " + (occupyingNPC != null ? occupyingNPC.name : "null"));
        if (isOccupied && occupyingNPC != null)
        {
            occupyingNPC.LeaveChair();  // Tell NPC to leave
            FreeChair();  // Free the chair
        }
        else
        {
            Debug.Log("Cannot interact. Chair is not occupied or NPC is null.");
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        outline = gameObject.GetComponentInChildren<Outline>();
        Debug.Log(outline);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
