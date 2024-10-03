using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableObject : MonoBehaviour, IInteractable
{
    [SerializeField] private UsableObjectSO usableObjectSO;

    private IUsableObjectParent usableObjectParent;
    private Outline outline;
    private Rigidbody rb;
    private Collider _collider;

    public void Interact(Player player)
    {
        // TODO: this should eventually depend on what player is holding...
        if (!player.HasUsableObject())
        {
            SetUsableObjectParent(player);
        }
    }

    public IInteractable GetOutlineableObject()
    {
        return null;
    }

    public void EnableOutline()
    {
        outline.enabled = true;
    }

    public void DisableOutline()
    {
        outline.enabled = false;
    }

    // Start is called before the first frame update
    void Awake()
    {
        outline = gameObject.GetComponent<Outline>();
        rb = gameObject.GetComponent<Rigidbody>();
        _collider = gameObject.GetComponent<Collider>();
    }


    public UsableObjectSO GetUsableObjectSO()
    {
        return usableObjectSO;
    }

    public IUsableObjectParent GetUsableObjectParent()
    {
        return usableObjectParent;
    }

    public void SetUsableObjectParent(IUsableObjectParent usableObjectParent)
    {
        // TODO: We can't call this for everything...
        // When holding ingredient and looking at an ingredient, nothing should happen
        // But when holding an ingredient and looking at a pan, the ingredient should
        // go in the pan.
        if (this.usableObjectParent != null)
        {
            this.usableObjectParent.ClearUsableObject();
        }

        this.usableObjectParent = usableObjectParent;
        if (usableObjectParent != null)
        {
            if (usableObjectParent.HasUsableObject())
            {
                Debug.LogError("IUsableObjectParent already has a UsableObject");
            }
            usableObjectParent.SetUsableObject(this);
            // When using Rigidbody, we're supposed to change rotation and position
            // through Rigidbody and not Transform, but this doesn't work for some
            // reason?
            // rb.rotation = Quaternion.identity;
            transform.rotation = Quaternion.identity;
            rb.position = usableObjectParent.GetUsableObjectFollowTransform().position;
            transform.parent = usableObjectParent.GetUsableObjectFollowTransform();
            transform.localPosition = Vector3.zero;
            // When holding or on table. Only turn kinematic off and enable collider
            // when on the ground
            rb.isKinematic = true;
            _collider.enabled = false;
        }
        else if (usableObjectParent == null)
        {
            _collider.enabled = true;
            rb.isKinematic = false;
            rb.position = transform.parent.parent.position + transform.forward;
            transform.parent = null;
        }
    }
}
