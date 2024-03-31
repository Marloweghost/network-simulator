using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireStateHandler : MonoBehaviour
{
    private WireRenderer wireRenderer;
    private Rigidbody rb;
    [SerializeField] private Transform visual;
    public enum State
    {
        Free,
        Connected
    }

    private State currentState;

    private void Start()
    {
        rb = GetComponentInChildren<Rigidbody>();
        wireRenderer = GetComponent<WireRenderer>();

        // TODO: Что делать, если только один провод подключен?
        //if ()
        //{
        //    ChangeState(State.Connected);
        //}
        //else
        //{
        //    ChangeState(State.Free);
        //}
    }

    private void OnStateChange()
    {
        switch (currentState)
        {
            case State.Connected: 
                break;
            case State.Free:
                break;
            default:
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
