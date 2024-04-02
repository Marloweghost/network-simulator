using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerInteraction : MonoBehaviour
{
    public float interactionDistance = 5f;

    [SerializeField] private Transform playerCamera;
    [SerializeField] private LayerMask deviceLayerMask;
    [SerializeField] private GameObject interactionSubPanel;
    private PlayerStateHandler playerStateHandler;
    private UINodeInterface uiNodeInterface;

    [Header("Pickup")]
    private Transform heldObject;
    public float pickupRange = 2f;
    public Transform hand;

    [Header("Connecting wires")]
    private PhysicalInterface firstConnectedInterface;
    private PhysicalInterface secondConnectedInterface;
    [SerializeField] private GameObject wirePrefab;

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
            if (Input.GetMouseButtonDown(1))
            {
                if (heldObject == null)
                {
                    Collider[] colliders = Physics.OverlapSphere(hand.position, pickupRange);
                    foreach (Collider collider in colliders)
                    {
                        if (collider.gameObject.GetComponent<Rigidbody>() != null)
                        {
                            if (collider.gameObject.tag == "Wire" && collider.gameObject.GetComponent<WireStateHandler>().GetCurrentState() == WireStateHandler.State.Connected)
                            {
                                Destroy(collider.gameObject);
                                heldObject = Instantiate(wirePrefab).transform;
                            }
                            else
                            {
                                heldObject = collider.gameObject.transform;
                            }

                            heldObject.SetParent(hand);
                            heldObject.localPosition = Vector3.zero;
                            heldObject.GetComponent<Rigidbody>().useGravity = false;
                            heldObject.GetComponent<Collider>().isTrigger = true;
                            break;
                        }
                    }
                }
                else
                {
                    DropObject();
                }
            } 
            else if (Input.GetMouseButtonDown(0))
            {
                if (heldObject != null) 
                {
                    switch (heldObject.tag)
                    {
                        case "Wire":
                            HandleWireUsage();
                            break;
                        default:
                            break;
                    }
                }
            }
        }
    }

    private void HandleWireUsage()
    {
        if (Physics.Raycast(transform.position, playerCamera.forward, out RaycastHit hitInfo, interactionDistance, deviceLayerMask))
        {
            if (firstConnectedInterface != null)
            {
                secondConnectedInterface = hitInfo.collider.gameObject.GetComponentInChildren<NetworkAdapter>().GetFreePhysicalInterface();

                if (secondConnectedInterface != null)
                {
                    firstConnectedInterface.PhysicalPort = heldObject.GetChild(0).gameObject;
                    secondConnectedInterface.PhysicalPort = heldObject.GetChild(1).gameObject;
                    firstConnectedInterface = null;
                    secondConnectedInterface = null;
                    heldObject.GetComponent<WireStateHandler>().ChangeState(WireStateHandler.State.Connected);
                    DropObject();
                }
                else
                {
                    Debug.Log("Все интерфейсы заняты!");
                }
            }
            else
            {
                firstConnectedInterface = hitInfo.collider.gameObject.GetComponentInChildren<NetworkAdapter>().GetFreePhysicalInterface();

                if (firstConnectedInterface == null)
                {
                    Debug.Log("Все интерфейсы заняты!");
                }
            }
        }
        else
        {
            firstConnectedInterface = null;
            Debug.Log("Выбор сброшен!");
        }
    }

    private void DropObject()
    {
        heldObject.SetParent(null);
        heldObject.GetComponent<Rigidbody>().useGravity = true;
        heldObject.GetComponent<Collider>().isTrigger = false;
        heldObject = null;
    }
}
