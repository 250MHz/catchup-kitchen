using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUsableObjectParent {
    public Transform GetUsableObjectFollowTransform();
    public void SetUsableObject(UsableObject usableObjec);
    public UsableObject GetUsableObject();
    public void ClearUsableObject();
    public bool HasUsableObject();
}
