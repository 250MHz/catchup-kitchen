using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool IsWalking { get; private set; }
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 720f;
    private Rigidbody playerRigidbody;
    private Vector2 moveAmount;


    // Start is called before the first frame update
    private void Start()
    {
        playerRigidbody = GetComponent<Rigidbody>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // read the value for the "move" action each event call
        moveAmount = context.ReadValue<Vector2>().normalized;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        Vector3 moveDir = new Vector3(moveAmount.x, 0, moveAmount.y);
        playerRigidbody.MovePosition(
            transform.position + moveDir * moveSpeed * Time.fixedDeltaTime
        );

        IsWalking = moveDir != Vector3.zero;
        if (IsWalking)
        {
            Quaternion toRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            playerRigidbody.MoveRotation(Quaternion.RotateTowards(
                playerRigidbody.rotation, toRotation,
                rotateSpeed * Time.fixedDeltaTime
            ));
        }
    }
}
