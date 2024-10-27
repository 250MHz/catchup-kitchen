using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public NPCController npcPrefab;
    public int maxGroups = 3; // Maximum number of groups that can exist at a time
    public float groupSpacing = 5f; // Distance between groups
    public int minGroupSize = 1; // Minimum number of NPCs in a group
    public int maxGroupSize = 4; // Maximum number of NPCs in a group

    private List<GameObject> npcGroups = new List<GameObject>();

    private void Start()
    {
        // Start spawning NPC groups at regular intervals
        InvokeRepeating(nameof(SpawnNPCGroup), 0f, 5f);
    }

    private void SpawnNPCGroup()
    {
        // Check if we've reached the maximum group limit
        if (npcGroups.Count >= maxGroups)
        {
            Debug.Log("Max groups reached, stopping NPC spawn.");
            CancelInvoke(nameof(SpawnNPCGroup)); // Stop spawning new groups
            return;
        }

        // Create an empty GameObject to hold the NPC group
        GameObject npcGroup = new GameObject("NPCGroup");

        // Determine the number of NPCs for this group
        int npcCount = Random.Range(minGroupSize, maxGroupSize + 1);

        // Determine the position for the new group with spacing
        Vector3 spawnPosition = transform.position + (Vector3.right * npcGroups.Count * groupSpacing);

        // Spawn NPCs and add them as children of the group object
        for (int i = 0; i < npcCount; i++)
        {
            Vector3 npcPosition = spawnPosition + new Vector3(i * 1.5f, 0, 0); // Space out NPCs within the group
            NPCController npc = Instantiate(npcPrefab, npcPosition, Quaternion.identity);
            npc.SetNPCGroup(npcGroup);
            npc.transform.parent = npcGroup.transform;
        }

        // Add the group to the list and place it in the world
        npcGroups.Add(npcGroup);
        Debug.Log($"Spawned a group of {npcCount} NPCs at position: {spawnPosition}");
    }

    public void RemoveGroup(GameObject group)
    {
        npcGroups.Remove(group);
        Destroy(group);

        if (npcGroups.Count < maxGroups && !IsInvoking(nameof(SpawnNPCGroup)))
        {
            InvokeRepeating(nameof(SpawnNPCGroup), 0f, 5f);
        }
    }

    public void RemoveGroupAndReorder(GameObject seatedGroup)
    {
        npcGroups.Remove(seatedGroup);  // Remove the seated group from the list


        // Reorder remaining groups
        for (int i = 0; i < npcGroups.Count; i++)
        {
            GameObject npcGroup = npcGroups[i];
            NPCController[] npcsInGroup = npcGroup.GetComponentsInChildren<NPCController>();

            for (int j = 0; j < npcsInGroup.Length; j++)
            {
                // Calculate a new position for each NPC within the group
                Vector3 newPosition = CalculatePosition(i) + new Vector3(j * 1.5f, 0, 0); // Space out each NPC within the group
                npcsInGroup[j].transform.position = newPosition;
            }
        }
    }


    private Vector3 CalculatePosition(int index)
    {
        // Offset based on the NPCSpawner's starting position
        return transform.position + new Vector3(index * groupSpacing, 0, 0); // Spacing based on group index
    }

}
