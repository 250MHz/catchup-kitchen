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
    [Space(10)]
    [SerializeField] private float baseOrderTime;
    [SerializeField, Tooltip(
        "For each extra customer at table, increase order time by this amount")
    ] private float extraTimePerCustomer;


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

    private bool isEating = false;

    private int check;

    private float totalPreparationTime;
    private float remainingServingTime;

    private AudioSource plateSound;


    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
        HideOrderUI();

        plateSound = gameObject.GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Debug.Log($"Remaining Dishes: {remainingDishes.Count}, Dirty Plates: {dirtyPlates.Count}");
        // Debug.Log($"Table State: {currentOrderState}");
    }

    public void Interact(Player player)
    {
        if (isEating)
        {
            Debug.Log("Cannot interact while eating.");
            return; // Prevent further interaction, fixed the error that player interact with the table while EatCoruntine() is running
        }

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
                    check = CalculateCheck();
                    Debug.Log($"Order reward calculated: ${check}");

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

    private int CalculateCheck()
    {
        int totalReward = 0;
        foreach (UsableObject servedDish in remainingDishes)
        {
            UsableObjectSO dishSO = (servedDish as PlateUsableObject)?.GetCurrentFullPlateRecipeSO();
            if (dishSO != null)
            {
                totalReward += Mathf.RoundToInt(dishSO.GetPrice());
            }
        }
        return totalReward;
    }


    private void HandleCompleteState(Player player)
    {
        // Start checking for plates if not already checking
        if (currentOrderState == OrderState.Complete && orderCountdownCoroutine == null)
        {
            StartCoroutine(CheckForPlatesCoroutine());
        }

        // Allow the player to pick up dirty plates or remaining dishes
        AllowPlayerToPickUp(player);
    }

    private IEnumerator CheckForPlatesCoroutine()
    {
        while (currentOrderState == OrderState.Complete)
        {
            // Check if there are dirty plates or remaining dishes
            if (dirtyPlates.Count == 0 && remainingDishes.Count == 0)
            {
                Debug.Log("No plates on the table. Resetting the table.");
                ResetTable();
                yield break; // Exit the coroutine
            }

            yield return new WaitForSeconds(1f); // Check every second (adjust the interval as needed)
        }
    }

    private void AllowPlayerToPickUp(Player player)
    {
        if (dirtyPlates.Count > 0)
        {
            Debug.Log("There are dirty plates on the table. Allowing player to pick them up.");
            if (!player.HasUsableObject())
            {
                // Allow the player to pick up a dirty plate
                dirtyPlates[0].SetUsableObjectParent(player);
                dirtyPlates.RemoveAt(0);
            }
        }
        else if (remainingDishes.Count > 0)
        {
            Debug.Log("There are remaining dishes on the table. Allowing player to pick them up.");
            if (!player.HasUsableObject())
            {
                // Allow the player to pick up a remaining dish
                remainingDishes[0].SetUsableObjectParent(player);
                remainingDishes.RemoveAt(0);
            }
        }
    }


    private IEnumerator EatCoroutine()
    {
        isEating = true;

        if (check > 0)
        {
            int tip = CalculateTip();

            Wallet.Instance.AddMoney(check, tip);

            if (tip > 0)
            {
                Debug.Log($"Player received an additional tip of ${tip}.");
            }

            check = 0;
        }

        remainingDishes.Clear();

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

                dish.DestroySelf(); // Remove the dish from the chair
            }
        }

        currentOrders.Clear();
        // No need to reset the table yet, will check in HandleCompleteState
        currentOrderState = OrderState.Complete;
        isEating = false;
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
        totalPreparationTime = CalculateTotalPreparationTime();
        remainingServingTime = totalPreparationTime;
        servingCountdownCoroutine = StartCoroutine(ServingCountdownCoroutine(totalPreparationTime));
    }

    private void StopServingCountdown()
    {
        if (servingCountdownCoroutine != null)
        {
            StopCoroutine(servingCountdownCoroutine);
            servingCountdownCoroutine = null;
        }
    }

    private IEnumerator ServingCountdownCoroutine(float totalTime)
    {
        float countdown = totalTime; // Serving wait time, number of NPCs times 20 seconds
        servingProgressBar.gameObject.SetActive(true);

        while (countdown > 0)
        {
            countdown -= Time.deltaTime;
            remainingServingTime = countdown;
            servingProgressBar.SetBarFillAmount(countdown / totalTime); // Update progress bar
            yield return null;
        }

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
        currentOrderState = OrderState.Complete;
        HideOrderUI();

        StartCoroutine(CheckForPlatesCoroutine());

        servingProgressBar.gameObject.SetActive(false);
        HideOrderUI();
    }

    private float CalculateTotalPreparationTime()
    {
        return baseOrderTime + (seatedNPCs.Count - 1) * extraTimePerCustomer;
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

                    int totalCheck = CalculateCheck();
                    int tip = CalculateTip();

                    totalCheck += tip;
                    Debug.Log($"Total Reward with Tip: ${totalCheck}");

                    // Assign the plate to the chair of the last order
                    playerHeldObject.SetUsableObjectParent(chairs[currentOrders.Count - 1]);

                    PlayPlateSound();

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

    private void PlayPlateSound()
    {
        if (plateSound != null && !plateSound.isPlaying)
        {
            plateSound.Play();
        }
    }

    private int CalculateTip()
    {
        // Tip is 10% if the player served with more than 60% time remaining
        if (remainingServingTime > totalPreparationTime * 0.6f)
        {
            return Mathf.RoundToInt(CalculateCheck() * 0.1f); // 0.1f represents that 10% of the check will be given as an additional tip, can change the value here
        }

        // No tip if time is less than 50%
        return 0;
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