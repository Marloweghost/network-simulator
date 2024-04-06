using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WireStateHandler : MonoBehaviour
{
    private Rigidbody rb;
    private WireRenderer wireRenderer;

    public enum State
    {
        Free,
        Connected
    }

    private State currentState;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        wireRenderer = GetComponent<WireRenderer>();
    }

    private void OnStateChange()
    {
        switch (currentState)
        {
            case State.Connected:
                rb.isKinematic = true;
                transform.position = (wireRenderer.startObject.localToWorldMatrix.GetPosition() + wireRenderer.endObject.localToWorldMatrix.GetPosition()) / 2f;
                wireRenderer.InitiateRenderLine();
                break;
            case State.Free:
                rb.isKinematic = false;
                wireRenderer.StopRenderLine();
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
