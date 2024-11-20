using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PotPanVisual : MonoBehaviour
{
    public virtual void UpdateVisual(Dictionary<UsableObjectSO, int> currentIngredients)
    {
        Debug.Log("Base UpdateVisual called");
    }
}

public class PotPanUsableObject : UsableObject
{
    private enum State
    {
        Idle,
        Cooking,
        Cooked,
        Burned,
    }

    [SerializeReference] private PotPanVisual potPanVisual;
    // Array of possible recipes the pot/pan can handle
    [SerializeField] private CookingRecipeSO[] cookingRecipeSOArray;
    [SerializeField] private ProgressBar progressBar;
    [SerializeField] private UsableObjectSO potPanUsableObjectSO;

    // Histogram of ingredients currently inside the pot/pan
    private Dictionary<UsableObjectSO, int> currentIngredients;
    // Initialize fields here instead of in Start() so we can set the state of
    // the new object created in Update()'s switch-case for Cooking
    private CookingRecipeSO currentFullRecipeSO = null;
    private State state = State.Idle;
    private float cookingTimer;
    private float burningTimer;

    private AudioSource beepingAudioSource;


    private void Start()
    {
        currentIngredients = new Dictionary<UsableObjectSO, int>();
        cookingTimer = 0f;
    }

    void Awake()
    {
        outline = gameObject.GetComponentInChildren<Outline>();
        rb = gameObject.GetComponent<Rigidbody>();
        _collider = gameObject.GetComponent<Collider>();

        beepingAudioSource = GetComponent<AudioSource>();


    }

    private void Update()
    {
        if (GetUsableObjectParent() is KitchenStove)
        {
            switch (state)
            {
                case State.Idle:
                    state = currentFullRecipeSO != null ? State.Cooking : State.Idle;
                    break;
                case State.Cooking:
                    cookingTimer += Time.deltaTime;
                    progressBar.SetSafeColor();
                    progressBar.SetBarFillAmount(cookingTimer / currentFullRecipeSO.GetCookingTimerMax());
                    if (cookingTimer > currentFullRecipeSO.GetCookingTimerMax())
                    {
                        // Cooked
                        IUsableObjectParent _parent = GetUsableObjectParent();
                        DestroySelf();
                        PotPanUsableObject newPotPanUsableObject = SpawnUsableObject(
                            currentFullRecipeSO.GetOutput(),
                            _parent
                        ) as PotPanUsableObject;
                        newPotPanUsableObject.state = State.Cooked;
                        newPotPanUsableObject.burningTimer = 0f;
                        newPotPanUsableObject.currentFullRecipeSO = currentFullRecipeSO;
                        progressBar.SetBarFillAmount(0f);
                    }
                    break;
                case State.Cooked:
                    StartBeepingSound();
                    burningTimer += Time.deltaTime;
                    progressBar.SetDangerColor();
                    progressBar.SetBarFillAmount(burningTimer / currentFullRecipeSO.GetBurningTimerMax());

                    if (burningTimer > currentFullRecipeSO.GetBurningTimerMax())
                    {
                        StopBeepingSound();
                        // Burned
                        IUsableObjectParent _parent = GetUsableObjectParent();
                        DestroySelf();
                        PotPanUsableObject newPotPanUsableObject = SpawnUsableObject(
                            currentFullRecipeSO.GetBurningOutput(),
                            _parent
                        ) as PotPanUsableObject;
                        newPotPanUsableObject.state = State.Burned;
                        progressBar.SetBarFillAmount(0f);
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
        else
        {
            StopBeepingSound();
        }
    }

    public void StartBeepingSound()
    {
        if (beepingAudioSource != null && !beepingAudioSource.isPlaying)
        {
            beepingAudioSource.Play();
        }
    }

    public void StopBeepingSound()
    {
        if (beepingAudioSource != null && beepingAudioSource.isPlaying)
        {
            beepingAudioSource.Stop();
        }
    }

    public UsableObjectSO GetPotPanUsableObjectSO()
    {
        return potPanUsableObjectSO;
    }

    public bool TryFindCompleteRecipe()
    {
        // We already have a match, and since there won't be recipes that
        // overlap, just return early
        // Remove this if there are recipes with overlapping ingredients
        if (currentFullRecipeSO != null)
        {
            return true;
        }
        // If currentIngredients completely matches with a CookingRecipeSO,
        // then we have a full recipe completed
        foreach (CookingRecipeSO recipeSO in cookingRecipeSOArray)
        {
            if (recipeSO.MatchesFullRecipe(currentIngredients))
            {
                currentFullRecipeSO = recipeSO;
                return true;
            }
        }
        return false;
    }

    public bool TryAddIngredient(UsableObjectSO usableObjectSO)
    {
        // Remove this if there are recipes with overlapping ingredients
        if (currentFullRecipeSO != null)
        {
            return false;
        }
        foreach (CookingRecipeSO recipeSO in cookingRecipeSOArray)
        {
            Dictionary<UsableObjectSO, int> recipeIngredients = recipeSO.GetIngredients();
            // If currentIngredients contains any ingredients not in the recipe,
            // then the recipe isn't reachable with the currentIngredients, or
            // if usableObjectSO is not an ingredient in the recipe, then the
            // recipe is irrelevant. Either case, try the next recipe.
            bool _reachable = recipeIngredients.ContainsKey(usableObjectSO);
            foreach (UsableObjectSO ingredient in currentIngredients.Keys)
            {
                if (!recipeIngredients.ContainsKey(ingredient))
                {
                    _reachable = false;
                    break;
                }
            }
            if (!_reachable)
            {
                continue;
            }
            // Create a Dictionary to see what ingredients we need to
            // complete the recipeSO.
            Dictionary<UsableObjectSO, int> remainingIngredients
                = new Dictionary<UsableObjectSO, int>(recipeIngredients);
            // Subtract any amount from currentIngredients
            foreach ((UsableObjectSO ingredient, int amount) in currentIngredients)
            {
                remainingIngredients[ingredient] -= amount;
            }
            // If usableObjectSO's amount is > 0 in remainingIngredients, then
            // usableObjectSO would help complete a recipe, so we can add it.
            if (remainingIngredients[usableObjectSO] > 0)
            {
                currentIngredients[usableObjectSO]
                    = currentIngredients.GetValueOrDefault(usableObjectSO, 0) + 1;
                potPanVisual.UpdateVisual(currentIngredients);
                // Set the complete recipe when we add the last ingredient
                TryFindCompleteRecipe();
                return true;
            }
        }
        return false;
    }

    public override void Interact(Player player)
    {
        if (!player.HasUsableObject())
        {
            SetUsableObjectParent(player);
        }
        else
        {
            // Player is holding a UsableObject
            UsableObject playerHeldObject = player.GetUsableObject();
            // If holding a plate, try to take food out from the pot/pan
            if (playerHeldObject is PlateUsableObject)
            {
                PlateUsableObject plate = playerHeldObject as PlateUsableObject;
                if (plate.TryAddIngredient(GetUsableObjectSO()))
                {
                    // If the ingredient was added, replace this pot/pan with an empty pot/pan
                    IUsableObjectParent potPanParent = GetUsableObjectParent();
                    Transform currentTransform = transform;
                    DestroySelf();
                    SpawnUsableObject(potPanUsableObjectSO, potPanParent, currentTransform);
                }
            }
            // Otherwise, place the UsableObject in the pot/pan if possible
            if (TryAddIngredient(playerHeldObject.GetUsableObjectSO()))
            {
                playerHeldObject.DestroySelf();
            }
        }
    }

    public bool IsEmpty()
    {
        return currentIngredients.Count == 0;
    }

}
