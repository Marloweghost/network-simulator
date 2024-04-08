using System.Collections;
using UnityEngine;

public class PacketSender : MonoBehaviour
{
    public void InitiateICMPEchoRequest(NetworkAdapter networkAdapter, string currentIP, string targetIP, int repeatCount)
    {
        Packet outputPacket = new Packet(8, "Echo request!", currentIP, targetIP, networkAdapter.MACAddress);

        StartCoroutine(RepeatICMPEchoRequest(networkAdapter, outputPacket, repeatCount));
    }

    private IEnumerator RepeatICMPEchoRequest(NetworkAdapter networkAdapter, Packet packet, int repeatCount)
    {
        for (int i = 0; i < repeatCount; i++)
        {
            if (networkAdapter.transform.parent.tag == "Switch" || networkAdapter.transform.parent.tag == "Router")
            {
                networkAdapter.SendPacket(packet);
            }
            else if (networkAdapter.transform.parent.tag == "Node")
            {
                networkAdapter.SendPacket(packet, 0);
            }

            yield return new WaitForSeconds(1f);
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
