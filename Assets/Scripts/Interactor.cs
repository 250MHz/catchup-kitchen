using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

interface IInteractable
{
    public void Interact();
    public void EnableOutline();
    public void DisableOutline();
}

public class Interactor : MonoBehaviour
{
    public Transform InteractorSource;
    public LayerMask InteractableLayermask;
    public float InteractRange;

    IInteractable interactable;

    // Start is called before the first frame update
    void Start()
    {

    }

    public void OnUse(InputValue value)
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange, InteractableLayermask))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (value.isPressed)
                {
                    interactable.Interact();
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        Ray r = new Ray(InteractorSource.position, InteractorSource.forward);
        if (Physics.Raycast(r, out RaycastHit hitInfo, InteractRange, InteractableLayermask))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IInteractable interactObj))
            {
                if (interactable == null || !interactable.Equals(interactObj))
                {
                    if (interactable != null)
                    {
                        interactable.DisableOutline();
                    }
                    interactable = interactObj;
                    interactable.EnableOutline();
                }
            }
        }
        else if (interactable != null)
        {
            interactable.DisableOutline();
            interactable = null;
        }
    }
}
