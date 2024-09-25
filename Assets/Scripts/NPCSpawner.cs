using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public List<Chair> chairs;  // List of available chairs

    private void Start()
    {
        InvokeRepeating(nameof(SpawnNPC), 0f, 5f);  // Spawn every 5 seconds
    }

    private void SpawnNPC()
    {
        // Find unoccupied chairs
        List<Chair> availableChairs = chairs.FindAll(chair => !chair.IsOccupied());

        if (availableChairs.Count > 0)
        {
            // Select a random unoccupied chair
            Chair randomChair = availableChairs[Random.Range(0, availableChairs.Count)];

            // Spawn NPC and assign it to the selected chair
            GameObject npc = Instantiate(npcPrefab, transform.position, Quaternion.identity);
            NPCController npcController = npc.GetComponent<NPCController>();
            npcController.SetTargetChair(randomChair);
        }
        else
        {
            Debug.Log("No available chairs!");
        }
    }
}
