using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStateHandler : MonoBehaviour
{
    [SerializeField] private PlayerCamera playerCamera;
    private PlayerMovement movement;

    public enum State
    {
        Movement,
        Interacting
    }

    private State currentState = State.Movement;

    private void Start()
    {
        movement = GetComponent<PlayerMovement>();
    }

    private void OnStateChange()
    {
        switch (currentState)
        {
            case State.Interacting:
                playerCamera.enabled = false;
                movement.enabled = false;
                Cursor.lockState = CursorLockMode.Confined;
                Cursor.visible = true;
                break;

            case State.Movement:
                playerCamera.enabled = true;
                movement.enabled = true;
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
                break;
        }
    }

    public void ChangeState(State newState)
    {
        currentState = newState;
        OnStateChange();
    }

    public State GetCurrentState()
    {
        return currentState;
    }
}
