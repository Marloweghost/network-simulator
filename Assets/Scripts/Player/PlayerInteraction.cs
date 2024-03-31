using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 5f;

    [SerializeField] private Transform playerCamera;
    [SerializeField] private LayerMask deviceLayerMask;
    [SerializeField] private GameObject interactionSubPanel;
    private PlayerStateHandler playerStateHandler;
    private UINodeInterface uiNodeInterface;

    private void Start()
    {
        playerStateHandler = GetComponent<PlayerStateHandler>();
        uiNodeInterface = interactionSubPanel.GetComponent<UINodeInterface>();
    }

    private void Update()
    {
        HandleNodeInteraction();
        HandleToolInteraction();
    }

    // TODO: Почистить
    private void HandleNodeInteraction()
    {
        switch (playerStateHandler.GetCurrentState())
        {
            case PlayerStateHandler.State.Movement:
                if (Input.GetKeyDown(KeyCode.F))
                {
                    if (Physics.Raycast(transform.position, playerCamera.forward, out RaycastHit hitInfo, interactionDistance, deviceLayerMask))
                    {
                        playerStateHandler.ChangeState(PlayerStateHandler.State.Interacting);
                        NetworkAdapter hitNetworkAdapter = hitInfo.transform.GetComponentInChildren<NetworkAdapter>();
                        uiNodeInterface.Activate(hitNetworkAdapter, this);
                    }
                }
                break;
            case PlayerStateHandler.State.Interacting:
                if (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.F))
                {
                    playerStateHandler.ChangeState(PlayerStateHandler.State.Movement);
                    uiNodeInterface.Deactivate();
                }
                break;
            default:
                Debug.Log("State undefined!");
                break;
        }
    }

    private void HandleToolInteraction()
    {
        if (playerStateHandler.GetCurrentState() == PlayerStateHandler.State.Movement)
        {

        }
    }
}
