using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private Animator animator;   // Animator component
    private Vector2 moveInput;   // Movement input

    void Awake()
    {
        // Get the Animator component attached to this game object
        animator = GetComponent<Animator>();
    }

    // This method will be called from the movement controller to update the move input
    public void UpdateMovement(Vector2 input)
    {
        moveInput = input;

        // Set the "isWalking" parameter based on whether the player is walking
        if (moveInput != Vector2.zero)
        {
            animator.SetBool("isWalking", true);
        }
        else
        {
            animator.SetBool("isWalking", false);
        }
    }
}