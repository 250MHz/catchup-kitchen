using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public interface IInteractable
{
    public void Interact(Player player);
    public IInteractable GetOutlineableObject();
    public void EnableOutline();
    public void DisableOutline();
}

public class Player : MonoBehaviour, IUsableObjectParent
{
    public bool IsWalking { get; private set; }
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 720f;
    [SerializeField] private LayerMask interactableLayerMask;
    [SerializeField] private float interactRange;
    [SerializeField] private Transform usableObjectHoldPoint;
    private Rigidbody playerRigidbody;
    private CapsuleCollider playerCollider;
    private Vector2 moveAmount;
    private IInteractable selectedInteractable;

    private UsableObject usableObject;

    // Start is called before the first frame update
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
        playerCollider = GetComponent<CapsuleCollider>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // read the value for the "move" action each event call
        moveAmount = context.ReadValue<Vector2>().normalized;
    }

    public void OnUse(InputAction.CallbackContext context)
    {
        if (context.performed)
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
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        HandleMovement();
        HandleInteractions();
    }

    private void HandleMovement()
    {
        Vector3 moveDir = new Vector3(moveAmount.x, 0, moveAmount.y);
        playerRigidbody.MovePosition(
            transform.position + moveDir * moveSpeed * Time.fixedDeltaTime
        );

        IsWalking = moveDir != Vector3.zero;
        if (IsWalking)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
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
        return usableObjectHoldPoint;
    }

    public UsableObject GetUsableObject()
    {
        return usableObject;
    }

    public void SetUsableObject(UsableObject usableObject)
    {
        this.usableObject = usableObject;
    }

    public void ClearUsableObject()
    {
        usableObject = null;
    }

    public bool HasUsableObject()
    {
        return usableObject != null;
    }
}
