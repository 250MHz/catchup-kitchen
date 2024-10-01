using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Chair targetChair;
    public float moveSpeed = 2f;
    private bool isLeaving = false;
    private bool isSitting = false;

    private Animator animator;  // Reference to Animator component
    private Outline outline;

    public void Interact()
    {
        Debug.Log("NPCController Interact() called");
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }

    public void SetTargetChair(Chair chair)
    {
        targetChair = chair;
        targetChair.OccupyChair(this);  // Pass this NPC to the chair
        Debug.Log("NPC has set the target chair: " + chair.name + " and occupied it.");
    }


    private void Awake()
    {
        // Get the Animator component attached to the NPC
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (targetChair != null && !isLeaving && !isSitting)
        {
            MoveToChair();
        }
        else if (isLeaving)
        {
            MoveAway();
        }
    }

    private void MoveToChair()
    {
        // Move towards the target chair
        transform.position = Vector3.MoveTowards(transform.position, targetChair.transform.position, moveSpeed * Time.deltaTime);

        // Check if NPC has reached the chair
        if (Vector3.Distance(transform.position, targetChair.transform.position) < 0.1f)
        {
            // NPC reaches the chair, align with the chair and sit
            SitOnChair();
        }
        else
        {
            // Trigger walking animation while moving
            animator.SetBool("isWalking", true);
        }
    }

    private void SitOnChair()
    {
        Debug.Log("NPC has reached and is sitting on the chair: " + targetChair.name);
        // Stop walking animation
        animator.SetBool("isWalking", false);

        // Trigger sitting animation
        animator.SetBool("isSitting", true);

        // Align NPC's position and rotation to match the chair
        transform.position = targetChair.transform.position;
        transform.rotation = targetChair.transform.rotation * Quaternion.Euler(0, 180, 0);

        // Mark NPC as sitting
        isSitting = true;
        // Occupy the chair
        targetChair.OccupyChair(this);
        Debug.Log("NPC has called OccupyChair on the chair.");
    }

    public void LeaveChair()
    {
        isLeaving = true;
        isSitting = false;  // NPC is no longer sitting
        animator.SetBool("isSitting", false);  // Disable sitting animation
        Debug.Log("NPC is leaving the chair.");
    }

    private void MoveAway()
    {
        // Move NPC backward
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;

        // Trigger walking animation
        animator.SetBool("isWalking", true);

        // Destroy the NPC when they move far enough away
        if (transform.position.z < -10f)
        {
            Destroy(gameObject);
        }
    }
}
