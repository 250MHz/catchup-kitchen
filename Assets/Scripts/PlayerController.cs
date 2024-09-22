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

    private Rigidbody rbPlayer1, rbPlayer2, rbPlayer3;

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
        // Move each player
        MovePlayer(rbPlayer1, moveInputPlayer1);
        MovePlayer(rbPlayer2, moveInputPlayer2);
        MovePlayer(rbPlayer3, moveInputPlayer3);
    }

    void MovePlayer(Rigidbody rb, Vector2 moveInput)
    {
        if (rb != null)
        {
            Vector3 movement = new Vector3(moveInput.x, 0f, moveInput.y) * moveSpeed * Time.fixedDeltaTime;
            rb.MovePosition(rb.position + movement);
        }
    }
}
