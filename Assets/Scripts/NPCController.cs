using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NPCController : MonoBehaviour, IInteractable
{
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
                foreach (var npc in groupNPCs) npc.ReturnToSpawn();
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
        navMeshAgent.isStopped = true;
        animator.SetBool("IsWalking", false);
    }

    private void ReturnToSpawn()
    {
        isFollowing = false;
        targetPlayer = null;
        navMeshAgent.SetDestination(spawnPoint);  // Walk back to spawn point
        animator.SetBool("IsWalking", true);
    }

    private void Start()
    {
        outline = GetComponent<Outline>();
    }

    private void Update()
    {
        if (isFollowing && targetPlayer != null)
        {
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
        Destroy(gameObject);
    }

    internal bool IsFollowingPlayer(Player player) => isFollowing;

    public void SetNPCGroup(GameObject group) => npcGroup = group;
    public GameObject GetNPCGroup() => npcGroup;
    public NPCSpawner GetNPCSpawner() => npcSpawner;
}
