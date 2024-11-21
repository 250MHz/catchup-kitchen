using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdAnimator : MonoBehaviour
{
    [SerializeField] private bool isEating;
    [SerializeField] private bool isIdle;

    private Animator animator;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (isEating)
        {
            animator.SetFloat("Speed_f", 0f);
            animator.SetBool("Eat_b", isEating);
        }
        else if (isIdle)
        {
            animator.SetFloat("Speed_f", 0f);
        }
    }
}
