using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlassTable : BaseFurniture, IInteractable
{
    [SerializeField] private Chair[] chairs;
    [SerializeField] private GameObject orderCardPrefab;
    [SerializeField] private UsableObjectSO[] dishes;
    [SerializeField] private float eatingSeconds = 5f;
    [SerializeField] private UsableObjectSO plateDirtySO;

    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private ProgressBar servingProgressBar;

    private Outline outline;
    private List<UsableObjectSO> currentOrders = new List<UsableObjectSO>();
    private List<NPCController> seatedNPCs = new List<NPCController>();
    private List<UsableObject> dirtyPlates = new List<UsableObject>();
    // TODO: probably change this to just a regular UI element, this is
    // also bad for showing different orders
    private GameObject activeOrderCard;
    private NPCSpawner npcSpawner;
    // TODO: probably shouldn't use GameObject for this
    private GameObject npcGroup;

    // TODO: unused reference??
    // public Canvas canvas;

    // Enum to manage interaction phases
    private enum OrderState { Seating, Ordering, Serving, Complete }
    private OrderState currentOrderState = OrderState.Seating;

    private Coroutine orderCountdownCoroutine;
    private Coroutine servingCountdownCoroutine;

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
                StartOrderCountdown();
                break;

            case OrderState.Ordering:
                PlaceOrder();
                currentOrderState = OrderState.Serving;  // Ready for serving
                StopOrderCountdown();
                StartServingCountdown();
                break;

            case OrderState.Serving:
                TryServeOrder(player.GetUsableObject());
                if (currentOrders.Count == 0)
                {
                    currentOrderState = OrderState.Complete;  // Order is complete
                    StopServingCountdown();
                    StartCoroutine(EatCoroutine());
                }
                break;

            case OrderState.Complete:
                // Interact with table to pick up dirty plates
                if (dirtyPlates.Count > 0)
                {
                    if (!player.HasUsableObject())
                    {
                        // Player is not holding something
                        // Give them one of the dirty plates
                        dirtyPlates[0].SetUsableObjectParent(player);
                        dirtyPlates.RemoveAt(0);
                        // Set to OrderState.Seating when all dirty plates are gone
                        if (dirtyPlates.Count == 0)
                        {
                            seatedNPCs.RemoveAll((NPCController npc) => true);
                            // Reset the NPCSpawner
                            npcSpawner.RemoveGroup(npcGroup);
                            npcGroup = null;
                            currentOrderState = OrderState.Seating;
                        }
                    }
                }
                break;
        }
    }

    //private IEnumerator EatCoroutine()
    //{
    //    // TODO: Eating animation
    //    // Customers eat for 5 seconds
    //    yield return new WaitForSeconds(eatingSeconds);
    //    // Make customers walk away
    //    foreach (NPCController npc in seatedNPCs)
    //    {
    //        // TODO: this currently just destroys the customers
    //        npc.WalkAway();
    //    }
    //    // Replace dishes with dirty plates
    //    foreach (Chair c in chairs)
    //    {
    //        if (c.GetUsableObject() != null)
    //        {
    //            c.GetUsableObject().DestroySelf();
    //            dirtyPlates.Add(UsableObject.SpawnUsableObject(plateDirtySO, c));
    //        }
    //    }
    //}

    private IEnumerator EatCoroutine()
    {
        // TODO: Eating animation
        // Customers eat for 5 seconds
        yield return new WaitForSeconds(eatingSeconds);

        // Make customers walk away
        foreach (NPCController npc in seatedNPCs)
        {
            // TODO: this currently just destroys the customers
            npc.WalkAway();
        }

        // Replace dishes with dirty plates
        foreach (Chair c in chairs)
        {
            if (c.GetUsableObject() != null)
            {
                c.GetUsableObject().DestroySelf();
                dirtyPlates.Add(UsableObject.SpawnUsableObject(plateDirtySO, c));
            }
        }

        // Clear current orders to prepare for new ones
        currentOrders.Clear();

        // Wait until dirty plates are removed from table before allowing new seating
        if (dirtyPlates.Count == 0)
        {
            seatedNPCs.Clear();
            npcSpawner.RemoveGroup(npcGroup);  // Remove the NPC group
            npcGroup = null;  // Reset the NPC group reference

            // Reset OrderState to allow new seating
            currentOrderState = OrderState.Seating;
        }
    }

    private void StartOrderCountdown()
    {
        if (orderCountdownCoroutine != null)
        {
            StopCoroutine(orderCountdownCoroutine);
        }
        orderCountdownCoroutine = StartCoroutine(OrderCountdownCoroutine());
    }

    private void StopOrderCountdown()
    {
        if (orderCountdownCoroutine != null)
        {
            StopCoroutine(orderCountdownCoroutine);
            orderCountdownCoroutine = null;
        }
    }

    private IEnumerator OrderCountdownCoroutine()
    {
        float countdownTime = 10f;
        while (countdownTime > 0)
        {
            countdownTime -= Time.deltaTime;
            progressBar.SetBarFillAmount(countdownTime / 10f);  // Update progress bar
            yield return null;
        }
        ResetTable();  // Reset table and remove NPCs after timeout
    }

    private void StartServingCountdown()
    {
        if (servingCountdownCoroutine != null)
        {
            StopCoroutine(servingCountdownCoroutine);
        }
        servingCountdownCoroutine = StartCoroutine(ServingCountdownCoroutine());
    }

    private void StopServingCountdown()
    {
        if (servingCountdownCoroutine != null)
        {
            StopCoroutine(servingCountdownCoroutine);
            servingCountdownCoroutine = null;
        }
    }

    private IEnumerator ServingCountdownCoroutine()
    {
        float countdown = 30f; // Serving wait time
        servingProgressBar.gameObject.SetActive(true);

        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            servingProgressBar.SetBarFillAmount(countdown / 30f); // Update progress bar
            yield return null;
        }

        ResetTable();  // Reset table and remove NPCs after serving timeout
    }

    

    private void ResetTable()
    {
        foreach (NPCController npc in seatedNPCs)
        {
            npc.WalkAway();
        }

        seatedNPCs.Clear();
        currentOrders.Clear();
        // npcSpawner.RemoveGroup(npcGroup);
        npcGroup = null;
        currentOrderState = OrderState.Seating;
        progressBar.SetBarFillAmount(0);  // Reset progress bar
        servingProgressBar.gameObject.SetActive(false);  // Hide the serving progress bar
        Debug.Log("Order timed out, resetting table.");
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
            for (int i = 0; i < followingNPCs.Count; i++)
            {
                SeatNPC(followingNPCs[i], i);
            }
            if (npcSpawner == null)
            {
                npcSpawner = followingNPCs[0].GetNPCSpawner();
            }
            npcGroup = followingNPCs[0].GetNPCGroup();
        }
    }

    private void SeatNPC(NPCController npc, int index)
    {
        Chair selectedChair = chairs[index];
        Transform seatingPoint = selectedChair.GetSeatPoint();

        if (seatingPoint != null)
        {
            npc.SitAt(
                selectedChair.GetSeatPoint(),
                Quaternion.LookRotation(-selectedChair.transform.forward)
            );
            seatedNPCs.Add(npc);
        }
    }

    private void ShowOrderUI()
    {
        // TODO: should replace this with regular 2D image than this spinning thing
        // if (activeOrderCard == null)
        // {
        //     // Position the order card slightly above the table
        //     Vector3 orderCardPosition = transform.position + Vector3.up * 1.0f;
        //     Quaternion orderCardRotation = Quaternion.Euler(0, 180, 0);

        //     // Instantiate the order card as a 3D object
        //     activeOrderCard = Instantiate(orderCardPrefab, orderCardPosition, orderCardRotation);

        //     activeOrderCard.transform.SetParent(transform, true);
        // }
    }

    private void PlaceOrder()
    {
        // Assign a random object from `dishes` for each customer
        // The customer class doesn't actually need to know anything
        // about the order, we just need to assign seats in a certain
        // order and then assign dishes in the same order.
        for (int i = 0; i < seatedNPCs.Count; i++)
        {
            currentOrders.Add(dishes[Random.Range(0, dishes.Length)]);
        }
        Debug.Log("Order placed! Preparing for service...");

        // Hide the progress bar after order placed
        if (progressBar != null)
        {
            progressBar.gameObject.SetActive(false);
        }
    }

    private void TryServeOrder(UsableObject playerHeldObject)
    {
        if (playerHeldObject is PlateUsableObject)
        {
            // N.b. playerHeldObject.GetUsableObjectSO() will always just return
            // a plate, you need to look at the currentFullPlateRecipeSO field
            // to know what recipe is met by the ingredients on the plate.
            UsableObjectSO playerHeldObjectSO =
                (playerHeldObject as PlateUsableObject)
                .GetCurrentFullPlateRecipeSO();
            if (playerHeldObjectSO == null)
            {
                return;
            }
            foreach (UsableObjectSO order in currentOrders)
            {
                if (order.GetObjectName() == playerHeldObjectSO.GetObjectName())
                {
                    // Assign plates from seatedNPCs.Count - 1 to 0
                    // We don't care who gets what, just that it gets on the table
                    playerHeldObject.SetUsableObjectParent(chairs[currentOrders.Count - 1]);
                    currentOrders.Remove(playerHeldObjectSO);

                    // Hide the serving progress bar after serving the dish
                    servingProgressBar.gameObject.SetActive(false);

                    return;
                }
            }
        }
    }

    public void EnableOutline() => outline.enabled = true;
    public void DisableOutline() => outline.enabled = false;
}
