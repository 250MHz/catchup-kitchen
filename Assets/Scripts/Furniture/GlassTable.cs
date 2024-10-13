using System.Collections.Generic;
using UnityEngine;

public class GlassTable : BaseFurniture, IInteractable
{
    private Outline outline;
    private List<Transform> chairs = new List<Transform>(); // List to store chair transforms
    private List<NPCController> seatedNPCs = new List<NPCController>();

    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
        // Populate the chairs list with child objects that represent chairs
        foreach (Transform child in transform)
        {
            if (child.CompareTag("Chair")) // Add the child that tagged "chair" to the list
            {
                chairs.Add(child);
            }
        }
    }

    public void Interact(Player player)
    {
        Debug.Log("Glass Table Interact() called");

        // Find the first NPC following the player
        NPCController[] npcs = FindObjectsOfType<NPCController>();
        List<NPCController> followingNPCs = new List<NPCController>();

        foreach (var npc in npcs)
        {
            if (npc != null && npc.IsFollowingPlayer(player) && !seatedNPCs.Contains(npc)) // Check if the NPC is following the player and not already seated
            {
                followingNPCs.Add(npc); // Add to the list of NPCs to be seated
            }
        }

        // Check if we have any following NPCs to seat
        if (followingNPCs.Count > 0)
        {
            // Seat all NPCs in the following NPCs list, up to the number of available chairs
            for (int i = 0; i < followingNPCs.Count && chairs.Count > 0; i++)
            {
                SeatNPC(followingNPCs[i]);
            }
        }
    }

    private void SeatNPC(NPCController npc)
    {
        if (chairs.Count > 0)
        {
            // Choose a random chair from the available chairs
            int randomIndex = Random.Range(0, chairs.Count);
            Transform selectedChair = chairs[randomIndex];

            // Find the seat point
            Transform seatingPoint = selectedChair.Find("SeatPoint");
            if (seatingPoint != null)
            {
                // Move the NPC to the seating point position
                npc.transform.position = seatingPoint.position;

                // Adjust the NPC's rotation to face the front of the chair
                Vector3 chairForwardDirection = selectedChair.forward;
                Vector3 desiredForwardDirection = -chairForwardDirection;
                npc.transform.rotation = Quaternion.LookRotation(desiredForwardDirection);

                // Stop the NPC, and perform sitting animation
                npc.StopFollowing();
                npc.Sitting();

                Debug.Log(npc.name + " has seated at " + selectedChair.name);

                // Mark the chair as occupied by removing it from the list
                chairs.RemoveAt(randomIndex); // Remove chair from available list

                // Add NPC to the seated list
                seatedNPCs.Add(npc);
            }
            else
            {
                Debug.LogWarning("SeatingPoint not found on " + selectedChair.name);
            }
        }
        else
        {
            Debug.LogWarning("No available chairs for seating.");
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

    void Update()
    {
    }
}
