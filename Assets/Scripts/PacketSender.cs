using UnityEngine;

public class PacketSender : MonoBehaviour
{
    public void InitiateICMPEchoRequest(NetworkAdapter networkAdapter, string currentIP, string targetIP)
    {
        Packet outputPacket = new Packet(8, "Echo request!", currentIP, targetIP, networkAdapter.MACAddress);
        if (networkAdapter.transform.parent.tag == "Switch")
        {
            networkAdapter.SendPacket(outputPacket);
        }
        else if (networkAdapter.transform.parent.tag == "Node")
        {
            networkAdapter.SendPacket(outputPacket, 0);
        }
    }

    public void InitiateICMPEchoReply(NetworkAdapter networkAdapter, Packet packet, string currentIP, int portNumber)
    {
        Packet outputPacket = new Packet(0, "Echo reply!", currentIP, packet.SenderIP, networkAdapter.MACAddress);
        networkAdapter.SendPacket(outputPacket, portNumber);
    }

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
