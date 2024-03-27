using UnityEngine;

public class PacketSender : MonoBehaviour
{
    public void InitiateARPReply(NetworkAdapter networkAdapter, Packet packet, string currentIP, int portNumber)
    {
        Packet outputPacket = new Packet(2, "This is ARP Reply!", currentIP, packet.SenderIP, networkAdapter.MACAddress);
        networkAdapter.SendPacket(outputPacket, portNumber);
    }

    public void InitiateARPRequest(NetworkAdapter networkAdapter, string currentIP, string broadcastIP)
    {
        Packet outputPacket = new Packet(1, "This is ARP Request!", currentIP, broadcastIP, networkAdapter.MACAddress);
        networkAdapter.SendPacketAllPorts(outputPacket);
    }
}
