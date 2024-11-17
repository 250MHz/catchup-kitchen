using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour, IInteractable
{
    public Transform leavingPoint;

    [SerializeField] private float moveSpeed = 2f;
    [SerializeField] private float followDistance = 1.5f;

    private bool isFollowing = false;
    private Vector3 spawnPoint;
    private Animator animator;
    private Outline outline;
    private Collider _collider;
    private GameObject npcGroup;

    private NPCSpawner npcSpawner;
    private NavMeshAgent navMeshAgent;
    private Player targetPlayer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        spawnPoint = transform.position;
        npcSpawner = FindObjectOfType<NPCSpawner>();
        _collider = GetComponent<Collider>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.speed = moveSpeed;
        navMeshAgent.isStopped = true;

        // Find the leaving point by name
        GameObject leavingPointObject = GameObject.Find("LeavingPoint");
        if (leavingPointObject != null)
        {
            leavingPoint = leavingPointObject.transform;
        }
        else
        {
            Debug.LogWarning("Leaving point not found in the scene.");
        }
    }

    public void EnableOutline() => outline.enabled = true;
    public void DisableOutline() => outline.enabled = false;

    public void Interact(Player player)
    {
        Transform npcGroupTransform = transform.parent;

        if (npcGroupTransform != null)
        {
            NPCController[] groupNPCs = npcGroupTransform.GetComponentsInChildren<NPCController>();
            if (isFollowing)
            {
                foreach (var npc in groupNPCs)
                {
                    npc.StopFollowing();
                    npc.ReturnToSpawn();
                }
            }
            else
            {
                foreach (var npc in groupNPCs) npc.FollowPlayer(player);
            }
        }
    }

    public void FollowPlayer(Player player)
    {
        isFollowing = true;
        targetPlayer = player;
        navMeshAgent.isStopped = false;
    }

    public void StopFollowing()
    {
        isFollowing = false;
        targetPlayer = null;
        // navMeshAgent.isStopped = true;
        animator.SetBool("IsWalking", false);
    }

    private void ReturnToSpawn()
    {
        isFollowing = false;
        targetPlayer = null;
        navMeshAgent.SetDestination(spawnPoint);
        navMeshAgent.isStopped = false;
        animator.SetBool("IsWalking", true);
    }

    private void Update()
    {
        if (isFollowing && targetPlayer != null)
        {
            // Follow player behavior
            float distanceToPlayer = Vector3.Distance(transform.position, targetPlayer.transform.position);
            if (distanceToPlayer > followDistance)
            {
                navMeshAgent.SetDestination(targetPlayer.transform.position);
                navMeshAgent.isStopped = false;
                animator.SetBool("IsWalking", true);
            }
            else
            {
                navMeshAgent.isStopped = true;
                animator.SetBool("IsWalking", false);
            }
        }
        else if (!isFollowing && Vector3.Distance(transform.position, spawnPoint) <= 0.2f)
        {
            // NPC reached spawn point
            navMeshAgent.isStopped = true;
            animator.SetBool("IsWalking", false);

            // Rotate to face the spawner
            Vector3 directionToSpawner = (npcSpawner.transform.position - transform.position).normalized;
            if (directionToSpawner != Vector3.zero)
            {
                Quaternion rotationToFaceSpawner = Quaternion.LookRotation(directionToSpawner);
                transform.rotation = rotationToFaceSpawner;
            }
            // Debug.Log("NPC reached spawn point and is facing spawner.");
        }
        else if (Vector3.Distance(transform.position, leavingPoint.position) < 0.2f)
        {
            // NPC reached the leaving point
            animator.SetBool("IsWalking", false);
            DestroyGroup(); // Destroy the NPC after reaching the leaving point
        }
    }

    private void DestroyGroup()
    {
        if (npcGroup != null)
        {
            foreach (NPCController npc in npcGroup.GetComponentsInChildren<NPCController>())
            {
                if (npc.navMeshAgent != null)
                {
                    npc.navMeshAgent.enabled = false; // Disable the NavMeshAgent
                }
            }
            Destroy(npcGroup); // Destroy the group
        }
        else
        {
            if (navMeshAgent != null)
            {
                navMeshAgent.enabled = false; // Disable the NavMeshAgent before destroying
            }
            Destroy(gameObject);
        }
    }


    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    public void SitAt(Transform chairSeatPoint, Quaternion forward)
    {
        navMeshAgent.enabled = false;

        transform.position = chairSeatPoint.position;
        transform.rotation = forward;

        _collider.enabled = false;

        StopFollowing();
        Sitting();

        npcSpawner.RemoveGroupAndReorder(npcGroup);
    }

    public void Sitting()
    {
        animator.SetBool("IsSitting", true);
    }

    public void WalkAway()
    {
        if (leavingPoint != null)
        {
            if (navMeshAgent != null) // Check if navMeshAgent is not null
            {
                navMeshAgent.enabled = true;
                navMeshAgent.SetDestination(leavingPoint.position);
                navMeshAgent.isStopped = false;

                // Update animator states
                animator.SetBool("IsSitting", false);
                animator.SetBool("IsWalking", true);
            }
        }
        else
        {
            Debug.LogWarning("Leaving point is null. Destroying the NPC.");
            Destroy(gameObject);
        }
    }


    internal bool IsFollowingPlayer(Player player) => isFollowing;

    public void SetNPCGroup(GameObject group) => npcGroup = group;
    public GameObject GetNPCGroup() => npcGroup;
    public NPCSpawner GetNPCSpawner() => npcSpawner;


    public void UpdateSpawnPoint(Vector3 newSpawnPoint)
    {
        Vector3 offset = -transform.forward * 0.5f;
        spawnPoint = newSpawnPoint + offset;
    }


}