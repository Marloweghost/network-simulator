using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PacketHandler : MonoBehaviour
{
    private NetworkAdapter networkAdapterRef;
    private PacketSender packetSenderRef;
    private ARPTable arpTable;

    private Queue<PacketWithTimestamp> packetQueue = new Queue<PacketWithTimestamp>();
    private float recheckInterval = 0.2f;

    private void Start()
    {
        networkAdapterRef = GetComponent<NetworkAdapter>();
        packetSenderRef = GetComponent<PacketSender>();
        arpTable = GetComponent<ARPTable>();

        StartCoroutine(RecheckHeldPacketsCoroutine());
    }

    public void HandlePacket(Packet _packetData, string _currentIpAddress, int portNumber)
    {
        string currentBroadcastAddress = BroadcastAddressCalculator.CalculateBroadcastAddress(_currentIpAddress, networkAdapterRef.subnetMask);

        arpTable.RemoveEntry(_packetData.SenderIP);
        arpTable.AddEntry(_packetData.SenderIP, _packetData.MACAddress, portNumber, Time.time);

        if (CompareIPs(_packetData.TargetIP, _currentIpAddress) == true | CompareIPs(_packetData.TargetIP, currentBroadcastAddress) == true)
        {
            Debug.Log("Packet is addressed to me!");

            switch (_packetData.MessageType)
            {
                // Message
                case -1:
                    
                    break;

                // ICMP Echo (Request)
                case 8:
                    // Make packet with code 0 and send back
                    break;

                // ICMP Echo Reply
                case 0:
                    // Output ping data
                    break;

                // ARP Request
                case 1:
                    packetSenderRef.InitiateARPReply(networkAdapterRef, _packetData, _currentIpAddress, portNumber);
                    break;

                // ARP Reply
                case 2:
                    arpTable.RemoveEntry(_packetData.SenderIP);
                    arpTable.AddEntry(_packetData.SenderIP, _packetData.MACAddress, portNumber, Time.time);
                    Debug.Log("Got ARP Reply, written into ARP Table!");
                    break;

                default:
                    Debug.LogError("Взаимодействие с данным типом пакета неопределено");
                    break;
            }
        }
        else
        {
            if (transform.parent.tag == "Switch")
            {
                if (arpTable != null)
                {
                    ARPEntry currentEntry = arpTable.GetEntry(_packetData.TargetIP);
                    if (currentEntry != null) // Если запись есть
                    {
                        networkAdapterRef.SendPacket(_packetData, currentEntry.PortNumber);
                    }
                    else
                    {
                        // Отправить ARP Request с кодом 1
                        packetSenderRef.InitiateARPRequest(networkAdapterRef, _currentIpAddress, currentBroadcastAddress);
                        // Буфер пакетов
                        PacketWithTimestamp enqueuePacket = new PacketWithTimestamp(_packetData);
                        packetQueue.Enqueue(enqueuePacket);
                    }
                }
                else
                {
                    Debug.Log("ARP-таблица не найдена!");
                }
            }
            else
            {
                Debug.Log("Not mine packet blocked by ARM!");
            }
        }
    }

    private IEnumerator RecheckHeldPacketsCoroutine() 
    {
        while (true)
        {
            yield return new WaitForSeconds(recheckInterval);

            int packetsCount = packetQueue.Count;
            for (int i = 0; i < packetsCount; i++)
            {
                if (packetQueue.TryDequeue(out PacketWithTimestamp packet) == true)
                {
                    ARPEntry currentEntry = arpTable.GetEntry(packet.packet.TargetIP);

                    if (currentEntry == null) // || currentEntry.TimeStamp <= packet.timestamp)
                    {
                        // Denied
                        Debug.Log("Packet from " + packet.packet.SenderIP + " is denied...");
                        packetQueue.Enqueue(packet);
                    }
                    else
                    {
                        Debug.Log("Packet from " + packet.packet.SenderIP + " accepted!");
                        // Accepted
                        networkAdapterRef.SendPacket(packet.packet, currentEntry.PortNumber);
                    }
                }
            }
        }
    }

    private bool CompareIPs(string _targetIpAddress, string _currentIpAddress)
    {
        return (_targetIpAddress == _currentIpAddress);
    }
}

[System.Serializable]
public class PacketWithTimestamp
{
    public Packet packet;
    public float timestamp;

    public PacketWithTimestamp(Packet packet)
    {
        this.packet = packet;
        this.timestamp = Time.time;
    }
}
