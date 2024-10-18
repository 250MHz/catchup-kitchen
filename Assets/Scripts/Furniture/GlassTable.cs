using System.Collections.Generic;
using UnityEngine;

public class GlassTable : BaseFurniture, IInteractable
{
    [SerializeField] private List<Transform> chairs = new List<Transform>();
    [SerializeField] private GameObject orderCardPrefab;

    private Outline outline;
    private List<NPCController> seatedNPCs = new List<NPCController>();
    private GameObject activeOrderCard;

    // TODO: unused reference??
    // public Canvas canvas;

    // Enum to manage interaction phases
    private enum OrderState { Seating, Ordering, Serving, Complete }
    private OrderState currentOrderState = OrderState.Seating;

    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }

    private void Update()
    {
        if (activeOrderCard != null)
        {
            // Rotate the order card slowly around the Y-axis
            activeOrderCard.transform.Rotate(0, 30 * Time.deltaTime, 0);
        }
    }

    public void Interact(Player player)
    {
        switch (currentOrderState)
        {
            case OrderState.Seating:
                SeatNPCs(player);
                ShowOrderUI();
                currentOrderState = OrderState.Ordering;  // Move to next state
                break;

            case OrderState.Ordering:
                PlaceOrder();
                currentOrderState = OrderState.Serving;  // Ready for serving
                break;

            case OrderState.Serving:
                ServeOrder();
                currentOrderState = OrderState.Complete;  // Order is complete
                break;

            case OrderState.Complete:
                Debug.Log("Order already complete.");
                break;
        }
    }

    private void SeatNPCs(Player player)
    {
        NPCController[] npcs = FindObjectsOfType<NPCController>();
        List<NPCController> followingNPCs = new List<NPCController>();

        foreach (var npc in npcs)
        {
            if (npc != null && npc.IsFollowingPlayer(player) && !seatedNPCs.Contains(npc))
            {
                followingNPCs.Add(npc);
            }
        }

        if (followingNPCs.Count > 0)
        {
            for (int i = 0; i < followingNPCs.Count && chairs.Count > 0; i++)
            {
                SeatNPC(followingNPCs[i]);
            }
        }
    }

    private void SeatNPC(NPCController npc)
    {
        if (chairs.Count > 0)
        {
            int randomIndex = Random.Range(0, chairs.Count);
            Transform selectedChair = chairs[randomIndex];
            Transform seatingPoint = selectedChair.Find("SeatPoint");

            if (seatingPoint != null)
            {
                npc.transform.position = seatingPoint.position;
                npc.transform.rotation = Quaternion.LookRotation(-selectedChair.forward);

                npc.StopFollowing();
                npc.Sitting();
                chairs.RemoveAt(randomIndex);
                seatedNPCs.Add(npc);
            }
        }
    }

    private void ShowOrderUI()
    {
        if (activeOrderCard == null)
        {
            // Position the order card slightly above the table
            Vector3 orderCardPosition = transform.position + Vector3.up * 1.0f;
            Quaternion orderCardRotation = Quaternion.Euler(0, 180, 0);

            // Instantiate the order card as a 3D object
            activeOrderCard = Instantiate(orderCardPrefab, orderCardPosition, orderCardRotation);

            activeOrderCard.transform.SetParent(transform, true);
        }
    }



    private void PlaceOrder()
    {
        Debug.Log("Order placed! Preparing for service...");
    }

    private void ServeOrder()
    {
        Debug.Log("Order served! Task complete.");
        Destroy(activeOrderCard);
    }

    public void EnableOutline() => outline.enabled = true;
    public void DisableOutline() => outline.enabled = false;
}
