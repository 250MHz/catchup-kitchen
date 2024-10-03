using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseFurniture : MonoBehaviour, IUsableObjectParent
{
    [SerializeField] private Transform topPoint;
    private UsableObject usableObject;

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
