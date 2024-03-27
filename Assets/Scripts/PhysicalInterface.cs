using UnityEngine;

public class PhysicalInterface : MonoBehaviour
{
    public GameObject PhysicalPort;
    private GameObject lastPhysicalPortConnected;
    [SerializeField] private int portID = 0;
    [SerializeField] private Transform PhysicalPortLocation;

    private void Update()
    {
        if (lastPhysicalPortConnected != null && PhysicalPort == null)
        {
            lastPhysicalPortConnected.GetComponent<SignalEnd>().PassPhysicalInterfaceData(null);
            lastPhysicalPortConnected = null;
        }

        if (PhysicalPort != null)
        {
            lastPhysicalPortConnected = PhysicalPort;

            UpdateConnectionEndPosition();
            PhysicalPort.GetComponent<SignalEnd>().PassPhysicalInterfaceData(this);
        } 
    }
    private void UpdateConnectionEndPosition()
    {
        Transform ConnectionEnd = PhysicalPort.GetComponent<Transform>();
        ConnectionEnd.position = PhysicalPortLocation.position;
    }

    public void PassFrameData(Packet _frameData)
    {
        // Convert frame to packet
        Packet _packetData = _frameData;
        // ...

        // Pass it to higher level
        GetComponent<NetworkAdapter>().ReceivePacket(_packetData, portID);
    }

    public void SendFrame(Packet _packet)
    {
        // Convert frame to signal
        Packet _signalData = _packet;
        // ...

        // Pass it to lower level
        if (PhysicalPort != null)
        {
            PhysicalPort.GetComponent<SignalEnd>().SendSignal(_signalData);
        }
        else Debug.LogWarning("Кабель не подключён или не определён!");
    }
}
