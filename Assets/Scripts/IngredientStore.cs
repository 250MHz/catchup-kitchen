using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IngredientStore : MonoBehaviour, IInteractable, IUsableObjectParent
{
    [SerializeField] private UsableObjectSO usableObjectSO;
    [SerializeField] private Transform topPoint;
    private UsableObject usableObject;
    private Outline outline;

    public void Interact(Player player)
    {
        if (!player.HasUsableObject()) {
            GameObject usableGameObject = Instantiate(usableObjectSO.GetPrefab());
            usableGameObject.GetComponent<UsableObject>().SetUsableObjectParent(player);
        }
    }

    public IInteractable GetOutlineableObject() {
        return usableObject;
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
    private void Start()
    {
        outline = gameObject.GetComponent<Outline>();
    }

    // Update is called once per frame
    private void Update()
    {
    }

    public Transform GetUsableObjectFollowTransform()
    {
        return topPoint;
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
