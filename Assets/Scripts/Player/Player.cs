using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.ProBuilder.MeshOperations;

public interface IInteractable
{
    public void Interact(Player player);
    public void EnableOutline();
    public void DisableOutline();
}

public class Player : MonoBehaviour, IUsableObjectParent
{
    public bool IsWalking { get; private set; }

    private enum State
    {
        Playing,
        Dialog,
        Shop,
    }

    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 720f;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private float interactRange;
    [SerializeField] private UsableObjectHoldPoint usableObjectHoldPoint;
    [SerializeField] private DialogManager dialogManager;
    [SerializeField] private ShopController shopController;
    [SerializeField] private ControlsUI controlsUI;

    private PlayerInput playerInput;
    private Rigidbody playerRigidbody;
    private CapsuleCollider playerCollider;
    private Vector2 moveAmount;
    private IInteractable selectedInteractable;
    private UsableObject usableObject;
    private State state;
    private string controlScheme; // WASD, IJKL, or Arrows

    // Start is called before the first frame update
    private void Start()
    {
        state = State.Playing;
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
        playerInput = GetComponent<PlayerInput>();

        controlsUI.gameObject.SetActive(true);
        controlsUI.SetKeyText(controlScheme, playerInput);
        controlsUI.ShowTemporarily();

        dialogManager.OnShowDialog += () => { state = State.Dialog; };
        // N.b. in the video https://youtu.be/2CmG7ZtrWso there are multiple
        // states, and it's possible to jump from Dialog to a state besides the
        // default one. In that case, you'd have if statements after closing
        // dialog, but this doesn't apply to us (we only have 2 states)
        dialogManager.OnCloseDialog += () => { state = State.Playing; };
        shopController.OnStart += () => { state = State.Shop; };
        shopController.OnFinish += () => { state = State.Playing; };
    }

    public void SetControlScheme(string scheme)
    {
        controlScheme = scheme;
    }

    public DialogManager GetDialogManager()
    {
        return dialogManager;
    }

    public ShopController GetShopController()
    {
        return shopController;
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // read the value for the "move" action each event call
        moveAmount = context.ReadValue<Vector2>().normalized;
        if (context.performed && state == State.Dialog)
        {
            dialogManager.OnMove(moveAmount);
        }
        else if (context.performed && state == State.Shop)
        {
            shopController.OnMove(moveAmount);
        }
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed && state == State.Playing)
        {
            if (selectedInteractable != null)
            {
                selectedInteractable.Interact(this);
            }
            else if (usableObject != null)
            {
                // Drop held object if holding on one
                usableObject.SetUsableObjectParent(null);
            }
        }
        else if (context.performed && state == State.Dialog)
        {
            dialogManager.OnUse();
        }
        else if (context.performed && state == State.Shop)
        {
            shopController.OnUse(this);
        }
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        if (context.performed && state == State.Dialog)
        {
            dialogManager.OnCancel();
        }
        else if (context.performed && state == State.Shop)
        {
            shopController.OnCancel(this);
        }
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!RoundSystem.Instance.isGameActive)
        {
            playerRigidbody.velocity = Vector3.zero;
            return;
        }
        if (state == State.Playing)
        {
            HandleMovement();
            HandleInteractions();
        }
        else if (state == State.Dialog)
        {
            dialogManager.HandleUpdate();
        }
    }

    private void HandleMovement()
    {
        // Use velocity instead of MovePosition to prevent clipping through
        // walls.
        Vector3 moveDir = new Vector3(moveAmount.x, 0, moveAmount.y) * moveSpeed;
        playerRigidbody.velocity = new Vector3(moveDir.x, playerRigidbody.velocity.y, moveDir.z);

        IsWalking = moveDir != Vector3.zero;
        if (IsWalking)
        {
            // An angularVelocity solution is better than MoveRotation because
            // if we have a small collider near the usableObjectHoldPoint, the
            // collider would not get past walls with angularVelocity but it
            // can with MoveRotation.
            // But I wasn't able to get rotation to work with the same way with
            // angular velocity, so settled on creating a bigger collider for
            // hold point and putting it above the player.
            Quaternion toRotation = Quaternion.LookRotation(moveDir / moveSpeed, Vector3.up);
            playerRigidbody.MoveRotation(Quaternion.RotateTowards(
                playerRigidbody.rotation, toRotation,
                rotateSpeed * Time.fixedDeltaTime
            ));
        }
    }

    private void HandleInteractions()
    {
        Vector3 p1 = transform.position;
        Vector3 p2 = p1 + Vector3.up * playerCollider.height;
        // This checks for collisions with objects on Ground 1, so `interactObj`
        // should not be an object on top of an object.
        if (Physics.CapsuleCast(p1, p2, playerCollider.radius, transform.forward,
            out RaycastHit hitInfo, interactRange, interactableLayerMask))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (selectedInteractable != interactObj)
                {
                    SetSelectedInteractable(interactObj);
                }
            }
        }
        else
        {
            SetSelectedInteractable(null);
        }
    }

    private void SetSelectedInteractable(IInteractable interactObj)
    {
        selectedInteractable?.DisableOutline();
        selectedInteractable = interactObj;
        // This isn't right. We don't want to enable outline for the interactale
        // we want to outline the object on top of the interactable (if there is one)
        selectedInteractable?.EnableOutline();
    }

    public Transform GetUsableObjectFollowTransform()
    {
        return usableObjectHoldPoint.transform;
    }

    public UsableObject GetUsableObject()
    {
        return usableObject;
    }

    public void SetUsableObject(UsableObject usableObject)
    {
        this.usableObject = usableObject;
        if (usableObject != null)
        {
            usableObjectHoldPoint.EnableCollider();
        }
    }

    public void ClearUsableObject()
    {
        usableObject = null;
        usableObjectHoldPoint.DisableCollider();
    }

    public bool HasUsableObject()
    {
        return usableObject != null;
    }
}
