using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SignalHandler : MonoBehaviour
{
    [SerializeField] private GameObject End1;
    [SerializeField] private GameObject End2;
    private SignalEnd signalEnd1 = null;
    private SignalEnd signalEnd2 = null;

    private void Start()
    {
        signalEnd1 = End1.GetComponent<SignalEnd>();
        signalEnd2 = End2.GetComponent<SignalEnd>();
    }

    private Packet bufferSignalData;

    public void PassSignal(Packet _signalData, GameObject _sender)
    {
        bufferSignalData = _signalData;
        
        if (_sender.gameObject.name == "End1")
        {
            signalEnd2.ReceiveSignal(bufferSignalData);
        }
        else 
        if (_sender.gameObject.name == "End2")
        {
            signalEnd1.ReceiveSignal(bufferSignalData);
        }
    }
}
