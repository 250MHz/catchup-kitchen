using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Player : MonoBehaviour
{
    public bool IsWalking { get; private set; }
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotateSpeed = 10f;
    private Vector2 moveAmount;


    // Start is called before the first frame update
    private void Start()
    {

    }

    public void OnMove(InputAction.CallbackContext context)
    {
        // read the value for the "move" action each event call
        moveAmount = context.ReadValue<Vector2>().normalized;
    }

    // Update is called once per frame
    private void Update()
    {
        Vector3 moveDir = new Vector3(moveAmount.x, 0, moveAmount.y);
        transform.position += moveDir * moveSpeed * Time.deltaTime;

        IsWalking = moveDir != Vector3.zero;
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * rotateSpeed);
    }


}
