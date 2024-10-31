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

    public Transform leavingPoint;

    private void Start()
    {
        // Start spawning NPC groups at regular intervals
        InvokeRepeating(nameof(SpawnNPCGroup), 0f, 5f);
    }

    private void SpawnNPCGroup()
    {
        if (npcGroups.Count >= maxGroups)
        {
            Debug.Log("Max groups reached, stopping NPC spawn.");
            CancelInvoke(nameof(SpawnNPCGroup));
            return;
        }

        GameObject npcGroup = new GameObject("NPCGroup");

        int npcCount = Random.Range(minGroupSize, maxGroupSize + 1);
        Vector3 spawnPosition = transform.position + (Vector3.right * npcGroups.Count * groupSpacing);

        for (int i = 0; i < npcCount; i++)
        {
            // Spawn NPC slightly back from the spawn point
            Vector3 npcPosition = spawnPosition + new Vector3(i * 1.5f, 0, 0) - transform.forward * 1.0f;

            // Calculate and apply rotation towards spawner
            Vector3 directionToSpawner = (transform.position - npcPosition).normalized;

            if (directionToSpawner == Vector3.zero)
            {
                directionToSpawner = Vector3.forward;
            }

            Quaternion rotationToFaceSpawner = Quaternion.LookRotation(directionToSpawner, Vector3.up);

            NPCController npc = Instantiate(npcPrefab, npcPosition, Quaternion.identity);

            npc.SetNPCGroup(npcGroup);
            npc.transform.parent = npcGroup.transform;

            // Set rotation towards the spawner
            npc.transform.rotation = rotationToFaceSpawner;
        }

        npcGroups.Add(npcGroup);
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
