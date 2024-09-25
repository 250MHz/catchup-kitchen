using UnityEngine;

public class NPCController : MonoBehaviour
{
    private Chair targetChair;
    public float moveSpeed = 2f;
    private bool isLeaving = false;

    public void SetTargetChair(Chair chair)
    {
        targetChair = chair;
        targetChair.OccupyChair(this);  // Pass this NPC to the chair
    }

    private void Update()
    {
        if (targetChair != null && !isLeaving)
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
            Debug.Log("NPC has reached the chair.");
        }
    }

    public void LeaveChair()
    {
        isLeaving = true;
        Debug.Log("NPC is leaving the chair.");
    }

    private void MoveAway()
    {
        
        transform.position += Vector3.back * moveSpeed * Time.deltaTime;

        if (transform.position.z < -10f)
        {
            Destroy(gameObject);
        }
    }
}
