using com.cyborgAssets.inspectorButtonPro;
using System;
using UnityEngine;

public class NetworkAdapter : MonoBehaviour
{
    public string MACAddress;
    public string ipAddressString;
    public string subnetMask = "255.255.255.0";

    [SerializeField] private UITextHandler textHandler;
    private Packet debugPacket;
    private PacketHandler packetHandler;
    private PacketSender packetSender;
    [Header("--SendMessage--")]
    [SerializeField] private string TargetIpAddress = "192.168.0.2";
    [Header("--DEBUG--")]
    [SerializeField] private string ipAddress;


    private void Start()
    {
        MACAddress = GenerateRandomMAC();
        Debug.Log(MACAddress);
        SetIPAddress(ipAddressString);

        packetHandler = GetComponent<PacketHandler>();
        packetSender = GetComponent<PacketSender>();
    }

    [ProButton]
    public void DebugNodeInterfaceChangeIP()
    {
        SetIPAddress(ipAddressString);
    }

    public void SetIPAddress(string IPAddress)
    {
        ipAddressString = IPAddress;
        ipAddress = IPAddress;
        Debug.Log(ipAddress.ToString());
        UpdateUI(IPAddress);
    }

    public void SetSubnetMask(string _subnetMask)
    {
        subnetMask = _subnetMask;
        Debug.Log(subnetMask);
    }

    public void UpdateUI(string IPAddress)
    {
        textHandler.TextUpdateIP(IPAddress);
    }

    private string GenerateRandomMAC()
    {
        System.Random random = new System.Random();

        byte[] macBytes = new byte[6];
        random.NextBytes(macBytes);

        macBytes[0] = (byte)(macBytes[0] & 0xFE);

        return string.Join(":", BitConverter.ToString(macBytes).Replace("-", ":"));
    }

    public void ReceivePacket(Packet _packetData, int portNumber)
    {
        Debug.Log(transform.parent.name + "(" + ipAddress.ToString() + ")" + " принял следующие данные: ");
        Debug.Log(_packetData.VerificationMessage + " from " + _packetData.SenderIP);

        packetHandler.HandlePacket(_packetData, ipAddress, portNumber);
    }
    [ProButton]
    private void ARMSendDebugPacket()
    {
        debugPacket = new Packet(-1, "This is a debug message", ipAddress, TargetIpAddress, MACAddress);
        SendPacket(debugPacket, 0);
    }
    [ProButton]
    private void ARMSendDebugARPRequest()
    {
        packetSender.InitiateARPRequest(this, ipAddress, BroadcastAddressCalculator.CalculateBroadcastAddress(ipAddress, subnetMask));
    }

    public void SendPacket(Packet _packet, int physicalInterfaceNumber)
    {
        GetComponents<PhysicalInterface>()[physicalInterfaceNumber].SendFrame(_packet);
        Debug.Log("Пакет отправлен по порту " + physicalInterfaceNumber);
    }

    public void SendPacketAllPorts(Packet _packet)
    {
        PhysicalInterface[] interfaces = GetComponents<PhysicalInterface>();

        for (int interfaceID = 0; interfaceID < interfaces.Length; interfaceID++)
            interfaces[interfaceID].SendFrame(_packet);
    }
}
