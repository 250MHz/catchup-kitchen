using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCController : MonoBehaviour, IInteractable
{
    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float followDistance = 1.5f;  // Minimum distance to maintain from the player

    private bool isFollowing = false;  // To track if NPCs are following the player
    private Vector3 spawnPoint;  // To store the NPC's spawn point
    private Animator animator;
    private Outline outline;
    private Collider _collider;
    private GameObject npcGroup;

    // Reference to the NPCSpawner
    private NPCSpawner npcSpawner;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spawnPoint = transform.position;  // Store the spawn point on awake
        npcSpawner = FindObjectOfType<NPCSpawner>();
        _collider = GetComponent<Collider>();
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

        Transform npcGroupTransform = transform.parent;

        if (npcGroupTransform != null)
        {
            NPCController[] groupNPCs = npcGroupTransform.GetComponentsInChildren<NPCController>();

            if (isFollowing)
            {
                foreach (var npc in groupNPCs)
                {
                    npc.ReturnToSpawn();
                }
            }
            else
            {
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
        animator.SetBool("IsWalking", false);
    }

    private void ReturnToSpawn()
    {
        isFollowing = false;
        Debug.Log("NPCs are returning to spawn point.");
        transform.position = spawnPoint;
    }

    private IEnumerator FollowCoroutine(Player player)
    {
        while (isFollowing)
        {
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            if (distanceToPlayer > followDistance)
            {
                Vector3 direction = (player.transform.position - transform.position).normalized;
                transform.position += direction * moveSpeed * Time.deltaTime;
                animator.SetBool("IsWalking", true);
            }
            else
            {
                animator.SetBool("IsWalking", false);
            }

            yield return null;
        }

        animator.SetBool("IsWalking", false);
    }

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void SitAt(Transform chairSeatPoint, Quaternion forward)
    {
        transform.position = chairSeatPoint.position;
        transform.rotation = forward;
        _collider.enabled = false;
        StopFollowing();
        Sitting();

        // Notify NPCSpawner to update group order
        npcSpawner.RemoveGroupAndReorder(npcGroup);
    }

    public void Sitting()
    {
        animator.SetBool("IsSitting", true);
    }

    public void WalkAway()
    {
        Destroy(gameObject);
    }

    internal bool IsFollowingPlayer(Player player)
    {
        return isFollowing;
    }

    public void SetNPCGroup(GameObject group)
    {
        npcGroup = group;
    }

    public GameObject GetNPCGroup()
    {
        return npcGroup;
    }

    public NPCSpawner GetNPCSpawner()
    {
        return npcSpawner;
    }
}
