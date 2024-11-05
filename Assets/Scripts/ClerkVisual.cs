using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClerkVisual : MonoBehaviour
{
    private Animator anim;

    private void Start()
    {
        anim = GetComponent<Animator>();
    }

    public void HandOff()
    {
        anim.SetTrigger("Interact");
    }
}
