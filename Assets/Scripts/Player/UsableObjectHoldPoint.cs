using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UsableObjectHoldPoint : MonoBehaviour
{
    // Prevents player from being too close to wall when holding an object
    private BoxCollider barrierCollider;

    // Start is called before the first frame update
    private void Start()
    {
        barrierCollider = GetComponent<BoxCollider>();
    }

    public void EnableCollider()
    {
        barrierCollider.enabled = true;
    }

    public void DisableCollider()
    {
        barrierCollider.enabled = false;
    }
}
