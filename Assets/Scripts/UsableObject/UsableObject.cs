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

    public virtual void Interact(Player player)
    {
        // TODO: this should eventually depend on what player is holding...
        if (!player.HasUsableObject())
        {
            // Player is not holding something
            SetUsableObjectParent(player);
        }
        else
        {
            // Player is holding something
            // If player is holding a pot/pan, try to add the object into the pot/pan.
            if (player.GetUsableObject().TryGetPotPan(out PotPanUsableObject potPanUsableObject))
            {
                if (potPanUsableObject.TryAddIngredient(usableObjectSO))
                {
                    DestroySelf();
                }
            }
            // If player is holding a plate, try to add the object into the plate
            // Don't handle the Pot interaction here, that's overriden in PotPanUsableObject
            else if (player.GetUsableObject().TryGetPlate(out PlateUsableObject plateUsableObject))
            {
                if (plateUsableObject.TryAddIngredient(usableObjectSO))
                {
                    DestroySelf();
                }
            }
        }
    }

    public IInteractable GetOutlineableObject()
    {
        return null;
    }

    public void EnableOutline()
    {
        if (outline != null)
        {
            outline.enabled = true;
        }
    }

    public void DisableOutline()
    {
        if (outline != null)
        {
            outline.enabled = false;
        }
    }

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
            // transform.parent.parent is meant to get the position of the player,
            // but when we spawn an object in (e.g., to replace a pot with soup on
            // the floor with an empty pot), transform.parent.parent is null, so
            // perform this check
            if (transform.parent.parent != null)
            {
                rb.position = transform.parent.parent.position + transform.forward;
            }
            transform.parent = null;
        }
    }

    public void DestroySelf()
    {
        usableObjectParent?.ClearUsableObject();
        Destroy(gameObject);
    }

    public bool TryGetPotPan(out PotPanUsableObject potPanUsableObject)
    {
        if (this is PotPanUsableObject)
        {
            potPanUsableObject = this as PotPanUsableObject;
            return true;
        }
        else
        {
            potPanUsableObject = null;
            return false;
        }
    }

    public bool TryGetPlate(out PlateUsableObject plateUsableObject)
    {
        if (this is PlateUsableObject)
        {
            plateUsableObject = this as PlateUsableObject;
            return true;
        }
        else
        {
            plateUsableObject = null;
            return false;
        }
    }

    public static UsableObject SpawnUsableObject(
        UsableObjectSO usableObjectSO,
        IUsableObjectParent parent,
        Transform optionalTransform = null)
    {
        GameObject usableGameObject;
        if (optionalTransform == null)
        {
            usableGameObject = Instantiate(usableObjectSO.GetPrefab());
        }
        else
        {
            usableGameObject = Instantiate(usableObjectSO.GetPrefab(), optionalTransform);
        }
        UsableObject usableObject = usableGameObject.GetComponent<UsableObject>();
        usableObject.SetUsableObjectParent(parent);
        return usableObject;
    }
}
