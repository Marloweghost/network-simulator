using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class UINodeInterface : MonoBehaviour
{
    [SerializeField] private GameObject textIPInputFieldCarrier;
    [SerializeField] private GameObject textCurrentIPCarrier;

    [SerializeField] private GameObject textCurrentMaskCarrier;
    [SerializeField] private GameObject textMaskInputFieldCarrier;

    [SerializeField] private GameObject textPingInputFieldCarrier;

    private TMP_Text currentIPTMProText;
    private TMP_InputField currentIPInputField;

    private TMP_Text currentMaskTMProText;
    private TMP_InputField currentMaskInputField;

    private TMP_InputField currentPingInputField;

    [HideInInspector] public PlayerInteraction playerRef;
    [HideInInspector] public NetworkAdapter networkAdapter;

    public void Activate(NetworkAdapter _networkAdapter, PlayerInteraction _playerRef)
    {
        transform.parent.gameObject.SetActive(true);
        networkAdapter = _networkAdapter;
        playerRef = _playerRef;

        currentIPTMProText = textCurrentIPCarrier.GetComponent<TMP_Text>();
        currentMaskTMProText = textCurrentMaskCarrier.GetComponent<TMP_Text>();

        UIUpdateText();
    }

    public void OnChangeIPButtonClicked()
    {
        currentIPInputField = textIPInputFieldCarrier.GetComponent<TMP_InputField>();
        networkAdapter.SetIPAddress(currentIPInputField.text);
        currentIPInputField.text = "";

        UIUpdateText();
    }

    public void OnChangeSubnetMaskClicked()
    {
        currentMaskInputField = textMaskInputFieldCarrier.GetComponent<TMP_InputField>();
        networkAdapter.SetSubnetMask(currentMaskInputField.text);
        currentMaskInputField.text = "";

        UIUpdateText();
    }

    public void OnSendEchoRequestButtonClicked()
    {
        currentPingInputField = textPingInputFieldCarrier.GetComponent<TMP_InputField>();
        networkAdapter.GetPacketSenderInstance().InitiateICMPEchoRequest(networkAdapter, networkAdapter.ipAddressString, currentPingInputField.text);
    }

    public void OnCloseButtonClicked()
    {
        Deactivate();
    }

    private void UIUpdateText()
    {
        currentIPTMProText.text = $"IP address: {networkAdapter.ipAddressString}";
        currentMaskTMProText.text = $"Subnet mask: {networkAdapter.subnetMask}";
    }

    public void Deactivate()
    {
        playerRef.gameObject.GetComponent<PlayerStateHandler>().ChangeState(PlayerStateHandler.State.Movement);
        transform.parent.gameObject.SetActive(false);
    }
}
