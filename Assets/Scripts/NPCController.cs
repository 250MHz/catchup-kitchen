using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    public float moveSpeed = 2f;
    public float followDistance = 1.5f;  // Minimum distance to maintain from the player
    private bool isFollowing = false;  // To track if NPCs are following the player
    private Vector3 spawnPoint;  // To store the NPC's spawn point
    private Animator animator;
    private Outline outline;

    // Reference to the NPCSpawner
    private NPCSpawner npcSpawner;

    private void Awake()
    {
        // Get the Animator component attached to the NPC
        animator = GetComponent<Animator>();
        spawnPoint = transform.position;  // Store the spawn point on awake
        // Get the NPCSpawner component in the scene
        npcSpawner = FindObjectOfType<NPCSpawner>();
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    public void Interact(Player player)
    {
        Debug.Log("NPCController Interact() called");

        // Get the parent of this NPC, which is the NPCGroup
        Transform npcGroupTransform = transform.parent;

        // Check if the group of NPCs exists
        if (npcGroupTransform != null)
        {
            // Get all NPCs in the group
            NPCController[] groupNPCs = npcGroupTransform.GetComponentsInChildren<NPCController>();

            // Check if this NPC is following the player
            if (isFollowing)
            {
                // If already following, return all NPCs to spawn point
                foreach (var npc in groupNPCs)
                {
                    npc.ReturnToSpawn();
                }
            }
            else
            {
                // If not following, start following for all NPCs in the group
                foreach (var npc in groupNPCs)
                {
                    npc.FollowPlayer(player);
                }
            }
        }
    }

    public void FollowPlayer(Player player)
    {
        isFollowing = true;
        StartCoroutine(FollowCoroutine(player));
    }

    public void StopFollowing()
    {
        isFollowing = false;
        StopAllCoroutines();
        animator.SetBool("isWalking", false);
    }

    private void ReturnToSpawn()
    {
        isFollowing = false;
        Debug.Log("NPCs are returning to spawn point.");
        transform.position = spawnPoint;  // Directly set the position to spawn point
    }

    private IEnumerator FollowCoroutine(Player player)
    {
        while (isFollowing)
        {
            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer > followDistance)
            {
                // Move towards the player's position if outside the follow distance
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;

                animator.SetBool("isWalking", true);
            }
            else
            {
                // Stop moving when within follow distance
                animator.SetBool("isWalking", false);
            }

            yield return null;  // Wait for the next frame
        }

        animator.SetBool("isWalking", false);  // Stop walking animation when not following
    }

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void Sitting()
    {
        animator.SetBool("isSitting", true);
    }

    internal bool IsFollowingPlayer(Player player)
    {
        return isFollowing;
    }
}
