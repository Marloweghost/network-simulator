using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed;

    [SerializeField] private float groundDrag;

    [Header("Ground Check")]
    [SerializeField] private float playerHeight;
    [SerializeField] private LayerMask whatIsGround;
    private bool grounded;

    [SerializeField] private Transform orientation;

    private float horizontalInput;
    private float verticalInput;
    private int upInput;
    private int downInput;

    Vector3 moveDirection;

    Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
    }

    private void Update()
    {
        ReadInput();
        LimitSpeed();

        if (Input.GetKeyDown(KeyCode.G)) 
        {
            if (currentMovementMode == MovementMode.Movement)
            {
                ChangeMovementMode(MovementMode.Ghost);
            }
            else
            {
                ChangeMovementMode(MovementMode.Movement);
            }
        }

        if (currentMovementMode == MovementMode.Ghost)
        {
            
        }
        else
        {
            grounded = Physics.Raycast(transform.position, Vector3.down, playerHeight * 0.5f + 0.2f, whatIsGround);

            // Исправить
            if (grounded)
            {
                rb.drag = groundDrag;
            }
            else
            {
                rb.drag = groundDrag;
            }
        }
    }

    private enum MovementMode
    {
        Movement,
        Ghost
    }

    private MovementMode currentMovementMode;

    private void ChangeMovementMode(MovementMode newMode)
    {
        currentMovementMode = newMode;
        OnMovementModeChange();
    }

    private void OnMovementModeChange()
    {
        switch (currentMovementMode)
        {
            case MovementMode.Movement:
                rb.isKinematic = false;
                rb.useGravity = true;
                break;
            case MovementMode.Ghost:
                rb.isKinematic = true;
                rb.useGravity = false;
                break;
            default:
                break;
        }
    }

    private void FixedUpdate()
    {
        if (currentMovementMode == MovementMode.Ghost)
        {
            MoveGhostPlayer();
        }
        else
        {
            MovePlayer();
        }
    }

    private void ReadInput()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");
        upInput = Input.GetKey(KeyCode.Space) ? 1 : 0;
        downInput = Input.GetKey(KeyCode.LeftControl) ? 1 : 0;
    }

    private void MovePlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput;

        rb.AddForce(moveDirection.normalized * moveSpeed * 10f, ForceMode.Force);
    }

    private void MoveGhostPlayer()
    {
        moveDirection = orientation.forward * verticalInput + orientation.right * horizontalInput + orientation.up * upInput + orientation.up * -1 * downInput;

        transform.Translate(moveDirection * moveSpeed * 0.01f);
    }

    private void LimitSpeed()
    {
        Vector3 flatVelocity = new Vector3(rb.velocity.x, 0f, rb.velocity.z);

        if (flatVelocity.magnitude > moveSpeed)
        {
            Vector3 limitedVelocity = flatVelocity.normalized * moveSpeed;
            rb.velocity = new Vector3(limitedVelocity.x, rb.velocity.y, limitedVelocity.z);
        }
    }
}

