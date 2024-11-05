using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GlassTable : BaseFurniture, IInteractable
{
    [SerializeField] private Chair[] chairs;
    [SerializeField] private Image[] orderIcons;
    [SerializeField] private UsableObjectSO[] dishes;
    [SerializeField] private float eatingSeconds = 5f;
    [SerializeField] private UsableObjectSO plateDirtySO;

    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private ProgressBar servingProgressBar;

    private Outline outline;
    private List<UsableObjectSO> currentOrders = new List<UsableObjectSO>();
    private List<NPCController> seatedNPCs = new List<NPCController>();
    private List<UsableObject> dirtyPlates = new List<UsableObject>();
    private NPCSpawner npcSpawner;
    // TODO: probably shouldn't use GameObject for this
    private GameObject npcGroup;
    private List<UsableObject> remainingDishes = new List<UsableObject>();

    // Enum to manage interaction phases
    private enum OrderState { Seating, Ordering, Serving, Complete }
    private OrderState currentOrderState = OrderState.Seating;

    private Coroutine orderCountdownCoroutine;
    private Coroutine servingCountdownCoroutine;

    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
        HideOrderUI();
    }

    private void Update()
    {
        Debug.Log($"Remaining Dishes: {remainingDishes.Count}, Dirty Plates: {dirtyPlates.Count}");
    }

    public void Interact(Player player)
    {
        switch (currentOrderState)
        {
            case OrderState.Seating:
                if (HasFollowingNPCs(player))
                {
                    SeatNPCs(player);
                    currentOrderState = OrderState.Ordering;  // Move to next state
                    StartOrderCountdown();
                }
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
                    HideOrderUI();
                    StopServingCountdown();
                    StartCoroutine(EatCoroutine());
                }
                break;

            case OrderState.Complete:
                HandleCompleteState(player);

                break;
        }
    }

    private void HandleCompleteState(Player player)
    {
        // Check if there are dirty plates on the table
        if (dirtyPlates.Count > 0)
        {
            Debug.Log("There are dirty plates on the table. Allowing player to pick them up.");
            if (!player.HasUsableObject())
            {
                // Allow the player to pick up a dirty plate
                dirtyPlates[0].SetUsableObjectParent(player);
                dirtyPlates.RemoveAt(0);
                return; // Return early if a plate was picked up
            }
        }

        if (remainingDishes.Count > 0)
        {
            Debug.Log("There are remaining dishes on the table. Allowing player to pick them up.");
            if (!player.HasUsableObject())
            {
                // Allow the player to pick up a remaining dish
                remainingDishes[0].SetUsableObjectParent(player);
                remainingDishes.RemoveAt(0);
                return; // Return early if a dish was picked up
            }
        }

        // If no dirty plates or remaining dishes, reset the table
        ResetTable();
        Debug.Log("Table is now reset.");
    }

    private IEnumerator EatCoroutine()
    {
        yield return new WaitForSeconds(eatingSeconds);

        foreach (NPCController npc in seatedNPCs)
        {
            npc.WalkAway();
        }

        foreach (Chair c in chairs)
        {
            if (c.GetUsableObject() != null)
            {
                UsableObject dish = c.GetUsableObject();
                dirtyPlates.Add(UsableObject.SpawnUsableObject(plateDirtySO, c));
                remainingDishes.Add(dish); // Add remaining dishes to the list
                dish.DestroySelf(); // Remove the dish from the chair
            }
        }

        currentOrders.Clear();
        // No need to reset the table yet, will check in HandleCompleteState
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

        currentOrderState = OrderState.Complete;

        // Make NPCs walk away
        foreach (NPCController npc in seatedNPCs)
        {
            if (npc != null) // Check if the npc is still valid
            {
                npc.WalkAway();
            }
        }

        // yield return new WaitForSeconds(1f);

        // ResetTable();

        servingProgressBar.gameObject.SetActive(false);
        HideOrderUI();
    }


    private void ResetTable()
    {
        // Resetting NPCs and orders
        foreach (NPCController npc in seatedNPCs)
        {
            npc.WalkAway();
        }

        seatedNPCs.Clear();
        currentOrders.Clear();
        currentOrderState = OrderState.Seating;
        progressBar.SetBarFillAmount(0);  // Reset progress bar
        HideOrderUI();
        //servingProgressBar.gameObject.SetActive(false);  // Hide the serving progress bar
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

    private void PlaceOrder()
    {
        // Assign a random object from dishes for each customer
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
        ShowOrderUI();
    }

    private void ShowOrderUI()
    {
        for (int i = 0; i < seatedNPCs.Count; i++)
        {
            Image orderIcon = orderIcons[i];
            orderIcon.gameObject.SetActive(true);
            // ShowOrderUI() has to be called after PlaceOrder() so that
            // currentOrders is not empty
            orderIcon.sprite = currentOrders[i].GetIcon();
        }
    }

    private void HideOrderUI()
    {
        foreach (Image order in orderIcons)
        {
            order.sprite = null;
            order.gameObject.SetActive(false);
        }
    }

    private void TryServeOrder(UsableObject playerHeldObject)
    {
        if (playerHeldObject is PlateUsableObject)
        {
            UsableObjectSO playerHeldObjectSO =
                (playerHeldObject as PlateUsableObject)
                .GetCurrentFullPlateRecipeSO();

            foreach (UsableObjectSO order in currentOrders)
            {
                if (order.GetObjectName() == playerHeldObjectSO.GetObjectName())
                {
                    // Assign the plate to the chair of the last order
                    playerHeldObject.SetUsableObjectParent(chairs[currentOrders.Count - 1]);

                    // Add the served dish to the remaining dishes list
                    remainingDishes.Add(playerHeldObject); // Add the plate being served to the remaining dishes list
                    currentOrders.Remove(order); // Remove the served order

                    // Disable the sprite for the order that was served
                    foreach (Image orderImage in orderIcons)
                    {
                        if (orderImage.IsActive() && orderImage.sprite == order.GetIcon())
                        {
                            orderImage.gameObject.SetActive(false);
                            break;
                        }
                    }

                    // Hide the serving progress bar after serving the dish
                    servingProgressBar.gameObject.SetActive(false);

                    return;
                }
            }
        }
    }

    public void EnableOutline() => outline.enabled = true;
    public void DisableOutline() => outline.enabled = false;

    // This function check any NPCs are following the player
    // If no NPCs following, the state machine will not go to next state, fixed the NullReferenceException when interact it
    private bool HasFollowingNPCs(Player player)
    {
        NPCController[] npcs = FindObjectsOfType<NPCController>();
        foreach (var npc in npcs)
        {
            if (npc != null && npc.IsFollowingPlayer(player))
            {
                return true;
            }
        }
        return false;
    }
}