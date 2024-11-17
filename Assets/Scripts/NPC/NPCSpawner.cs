using System.Collections.Generic;
using UnityEngine;

public class NPCSpawner : MonoBehaviour
{
    public NPCController npcPrefab;
    public Transform leavingPoint;
    public int maxGroups = 3; // Maximum number of groups that can exist at a time
    public float groupSpacing = 5f; // Distance between groups
    public int minGroupSize = 1; // Minimum number of NPCs in a group
    public int maxGroupSizeRound1 = 2;
    public int maxGroupSizeRound2 = 3;
    public int maxGroupSizeRound3 = 4;

    private List<GameObject> npcGroups = new List<GameObject>();
    private float roundInterval;

    private void Start()
    {
        // Start spawning NPC groups at regular intervals
        InvokeRepeating(nameof(SpawnNPCGroup), 0f, 5f);
    }

    private void SpawnNPCGroup()
    {
        // Debug.Log("Attempting to spawn NPC group.");

        if (npcGroups.Count >= maxGroups)
        {
            Debug.Log("Max groups reached, stopping NPC spawn.");
            return;
        }

        int maxGroupSize = GetMaxGroupSizeForRound();

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
        // Debug.Log("NPC group spawned successfully.");
    }

    private int GetMaxGroupSizeForRound()
    {
        int round = RoundSystem.Instance.roundNumber;
        // Use <= since RoundSystem.Instance.roundNumber might not be updated
        // yet when game starts
        if (round <= 1)
        {
            return maxGroupSizeRound1;
        }
        else if (round == 2)
        {
            return maxGroupSizeRound2;
        }
        else
        {
            return maxGroupSizeRound3;
        }
    }

    //public void RemoveGroup(GameObject group)
    //{
    //    npcGroups.Remove(group);
    //    Destroy(group);
    //    Debug.Log("NPC group removed.");

    //    if (npcGroups.Count < maxGroups && !IsInvoking(nameof(SpawnNPCGroup)))
    //    {
    //        Debug.Log("Re-invoking NPC spawn.");
    //        InvokeRepeating(nameof(SpawnNPCGroup), 0f, 5f);
    //    }
    //}

    public void RemoveGroupAndReorder(GameObject seatedGroup)
    {
        npcGroups.Remove(seatedGroup);  // Remove the seated group from the list
        // Debug.Log("Group seated and removed, reordering remaining groups.");

        // Loop through each remaining group to reposition it
        for (int i = 0; i < npcGroups.Count; i++)
        {
            GameObject npcGroup = npcGroups[i];

            // Calculate the new position for this group
            Vector3 groupPosition = CalculatePosition(i);
            npcGroup.transform.position = groupPosition;

            // Adjust each NPC's local position within the group to maintain spacing
            NPCController[] npcsInGroup = npcGroup.GetComponentsInChildren<NPCController>();

            for (int j = 0; j < npcsInGroup.Length; j++)
            {
                // Set the local position of each NPC within the group
                npcsInGroup[j].transform.localPosition = new Vector3(j * 1.5f, 0, 0);
                npcsInGroup[j].UpdateSpawnPoint(npcsInGroup[j].transform.position);
            }
        }
    }

    private Vector3 CalculatePosition(int index)
    {
        return transform.position + new Vector3(index * groupSpacing, 0, 0);
    }
}
