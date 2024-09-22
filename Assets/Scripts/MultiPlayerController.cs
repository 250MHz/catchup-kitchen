using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class MultiPlayerController : MonoBehaviour
{
    public PlayerControls controls;

    public GameObject player1, player2, player3;  // The three player characters
    private Vector2 moveInputPlayer1, moveInputPlayer2, moveInputPlayer3;  // Movement inputs for each player

    public float moveSpeed = 5f;
    public float rotationSpeed = 720f;  // Speed of rotation

    private Rigidbody rbPlayer1, rbPlayer2, rbPlayer3;

    private AnimationController animController1, animController2, animController3;  // Reference to AnimationController for each player

    void Awake()
    {
        // Initialize controls
        controls = new PlayerControls();

        // Player 1 - WASD
        controls.Player1.Move.performed += ctx => moveInputPlayer1 = ctx.ReadValue<Vector2>();
        controls.Player1.Move.canceled += ctx => moveInputPlayer1 = Vector2.zero;

        // Player 2 - IJKL
        controls.Player2.Move.performed += ctx => moveInputPlayer2 = ctx.ReadValue<Vector2>();
        controls.Player2.Move.canceled += ctx => moveInputPlayer2 = Vector2.zero;

        // Player 3 - Arrow Keys
        controls.Player3.Move.performed += ctx => moveInputPlayer3 = ctx.ReadValue<Vector2>();
        controls.Player3.Move.canceled += ctx => moveInputPlayer3 = Vector2.zero;

        // Get Rigidbody components for each player
        rbPlayer1 = player1.GetComponent<Rigidbody>();
        rbPlayer2 = player2.GetComponent<Rigidbody>();
        rbPlayer3 = player3.GetComponent<Rigidbody>();

        // Get AnimationController components for each player
        animController1 = player1.GetComponent<AnimationController>();
        animController2 = player2.GetComponent<AnimationController>();
        animController3 = player3.GetComponent<AnimationController>();
    }

    void OnEnable()
    {
        controls.Player1.Enable();
        controls.Player2.Enable();
        controls.Player3.Enable();
    }

    void OnDisable()
    {
        controls.Player1.Disable();
        controls.Player2.Disable();
        controls.Player3.Disable();
    }

    void FixedUpdate()
    {
        // Move and animate each player
        MovePlayer(rbPlayer1, animController1, moveInputPlayer1);
        MovePlayer(rbPlayer2, animController2, moveInputPlayer2);
        MovePlayer(rbPlayer3, animController3, moveInputPlayer3);
    }

    void MovePlayer(Rigidbody rb, AnimationController animController, Vector2 moveInput)
    {
        if (rb != null && moveInput != Vector2.zero)
        {
            // Movement logic
            Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);

            // Rotation logic - rotate character in the direction of movement
            Vector3 direction = new Vector3(moveInput.x, 0f, moveInput.y);
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            rb.MoveRotation(Quaternion.RotateTowards(rb.rotation, toRotation, rotationSpeed * Time.fixedDeltaTime));

            // Update animation based on movement
            animController.UpdateMovement(moveInput);
        }
        else
        {
            // Set animation to idle when there is no movement
            animController.UpdateMovement(Vector2.zero);
        }
    }
}
