using com.cyborgAssets.inspectorButtonPro;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalEnd : MonoBehaviour
{
    [SerializeField] private GameObject SignalHandlerObject;

    private SignalHandler currentSignalHandler;
    private PhysicalInterface currentPhysicalInterface;

    private void Start()
    {
        currentSignalHandler = SignalHandlerObject.GetComponent<SignalHandler>();
    }

    public void SendSignal(Packet _signalData)
    {
        if (currentSignalHandler != null)
        {
            currentSignalHandler.PassSignal(_signalData, gameObject);
        }
        else Debug.LogWarning("currentSignalHandler is undefined!");
    }

    public void ReceiveSignal(Packet _signalData)
    {
        try
        {
            currentPhysicalInterface.PassFrameData(_signalData);
        }
        catch
        { 
            Debug.Log("Возможно, нет соединения с физическим портом. Кадр утерян.");
        }
    }

    public void PassPhysicalInterfaceData(PhysicalInterface _interface)
    {
        currentPhysicalInterface = _interface;
    }
}
