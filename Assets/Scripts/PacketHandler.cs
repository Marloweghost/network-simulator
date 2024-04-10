using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PacketHandler : MonoBehaviour
{
    private NetworkAdapter networkAdapterRef;
    private PacketSender packetSenderRef;
    private ARPTable arpTable;
    private RoutingTable routingTable;

    private Queue<PacketWithTimestamp> packetQueue = new Queue<PacketWithTimestamp>();
    private float recheckInterval = 0.2f;
    private float packetTTL = 1f;

    private void Start()
    {
        networkAdapterRef = GetComponent<NetworkAdapter>();
        packetSenderRef = GetComponent<PacketSender>();
        arpTable = GetComponent<ARPTable>();

        if (transform.parent.tag == "Router")
        {
            routingTable = GetComponent<RoutingTable>();
        }

        StartCoroutine(RecheckHeldPacketsCoroutine());
    }

    public ARPEntry GetARPTableEntry(string ipAddress)
    {
        return arpTable.GetEntry(ipAddress);
    }

    public void HandlePacket(Packet _packetData, string _currentIpAddress, int portNumber)
    {
        string currentBroadcastAddress = BroadcastAddressCalculator.CalculateBroadcastAddress(_currentIpAddress, networkAdapterRef.subnetMask);
        bool forceSelfForwardPacket = false;

        if (_packetData.SenderIP != _currentIpAddress)
        {
            arpTable.RemoveEntry(_packetData.SenderIP);
            arpTable.AddEntry(_packetData.SenderIP, _packetData.MACAddress, portNumber, Time.time);
        }

        //if (transform.parent.tag == "Router")
        //{
        //    Debug.Log($"Got packet from {_packetData.SenderIP} to {_packetData.TargetIP}!");
        //}

        if (transform.parent.tag == "Router")
        {
            _packetData.TTL -= 1;

            if (_packetData.TTL < 0)
            {
                Debug.Log("Packet got into routing loop, expired!");
                return;
            }

            Route route = GetRouteInTableByIP(_packetData.TargetIP, routingTable);
            if (route != null && _packetData.MessageType == 1)
            {
                forceSelfForwardPacket = true;
            }
        }

        if (CompareIPs(_packetData.TargetIP, _currentIpAddress) == true | CompareIPs(_packetData.TargetIP, currentBroadcastAddress) == true | forceSelfForwardPacket == true)
        {
            Debug.Log($"Packet type {_packetData.MessageType} is addressed to me!");

            switch (_packetData.MessageType)
            {
                // Message
                case -1:
                    
                    break;

                // ICMP Echo (Request)
                case 8:
                    // Make packet with code 0 and send back
                    packetSenderRef.InitiateICMPEchoReply(networkAdapterRef, _packetData, _currentIpAddress, portNumber);
                    break;

                // ICMP Echo Reply
                case 0:
                    // Output ping data
                    string log = $"Reply from {_packetData.SenderIP}: bytes=- time=-ms TTL={_packetData.TTL}";
                    Debug.Log(log);
                    networkAdapterRef.uINodeInterface.AddConsoleText(log);
                    TaskManager.onActionCompleted.Invoke(new ActionInfoPing(transform.parent.name, _packetData.SenderName));
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
                        PacketWithTimestamp enqueuePacket = new PacketWithTimestamp(_packetData, _packetData.TargetIP);
                        packetQueue.Enqueue(enqueuePacket);
                    }
                }
                else
                {
                    Debug.Log("ARP-таблица не найдена!");
                }
            }
            else if (transform.parent.tag == "Router")
            {
                Route foundRoute = null;

                if (routingTable != null)
                {
                    foundRoute = GetRouteInTableByIP(_packetData.TargetIP, routingTable);

                    if (foundRoute == null)
                    {
                        Debug.Log("Запись в таблице маршрутизации не найдена!");
                        Debug.Log("Destination unreachable!");
                    }
                    else
                    {
                        if (arpTable != null)
                        {
                            ARPEntry currentEntry = arpTable.GetEntry(foundRoute.NextHop);
                            if (currentEntry != null) // Если запись есть
                            {
                                networkAdapterRef.SendPacket(_packetData, currentEntry.PortNumber);
                            }
                            else
                            {
                                // Отправить ARP Request с кодом 1
                                packetSenderRef.InitiateARPRequest(networkAdapterRef, _currentIpAddress, currentBroadcastAddress);
                                // Буфер пакетов
                                PacketWithTimestamp enqueuePacket = new PacketWithTimestamp(_packetData, foundRoute.NextHop);
                                packetQueue.Enqueue(enqueuePacket);
                            }
                        }
                        else
                        {
                            Debug.Log("ARP-таблица не найдена!");
                        }
                    }
                }
                else
                {
                    Debug.Log("Таблица маршрутизации не найдена!");

                }
                // Определить, на какой узел отправлять пакет
            }
            else
            {
                Debug.Log($"Not mine packet blocked by ARM {_currentIpAddress}!");
            }
        }
    }

    public Route GetRouteInTableByIP(string ip, RoutingTable _routingTable)
    {
        Route[] currentRoutes = _routingTable.GetAllRoutes();

        foreach (Route route in currentRoutes)
        {
            if (IsIpInSubnet(ip, route.Destination, route.SubnetMask))
            {
                return route;
            }
        }

        return null;
    }

    private bool IsIpInSubnet(string ip, string subnet, string mask)
    {
        // Разбиваем IP-адрес, подсеть и маску на октеты
        string[] ipOctets = ip.Split('.');
        string[] subnetOctets = subnet.Split('.');
        string[] maskOctets = mask.Split('.');

        // Преобразуем октеты в целые числа
        int[] ipNumbers = Array.ConvertAll(ipOctets, int.Parse);
        int[] subnetNumbers = Array.ConvertAll(subnetOctets, int.Parse);
        int[] maskNumbers = Array.ConvertAll(maskOctets, int.Parse);

        // Вычисляем адрес сети
        int[] networkNumbers = new int[4];
        for (int i = 0; i < 4; i++)
        {
            networkNumbers[i] = subnetNumbers[i] & maskNumbers[i];
        }

        // Вычисляем адрес IP-адреса в сети
        int[] ipInNetworkNumbers = new int[4];
        for (int i = 0; i < 4; i++)
        {
            ipInNetworkNumbers[i] = ipNumbers[i] & maskNumbers[i];
        }

        // Проверяем, что адрес IP-адреса в сети совпадает с адресом сети
        for (int i = 0; i < 4; i++)
        {
            if (ipInNetworkNumbers[i] != networkNumbers[i])
            {
                return false;
            }
        }

        return true;
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
                    ARPEntry currentEntry = arpTable.GetEntry(packet.firstTargetIP);

                    if (Time.time - packet.timestamp > packetTTL) 
                    {
                        ARPEntry defaultGatewayEntry = arpTable.GetEntry(networkAdapterRef.defaultGateway);
                        if (defaultGatewayEntry != null)
                        {
                            networkAdapterRef.SendPacket(packet.packet, defaultGatewayEntry.PortNumber);
                            Debug.Log("Packet from " + packet.packet.SenderIP + packet.packet.VerificationMessage + " is rerouted by " + networkAdapterRef.ipAddressString + " to default gateway!");
                            break;
                        }
                        else
                        {
                            Debug.Log("Packet from " + packet.packet.SenderIP + " is expired inside " + networkAdapterRef.ipAddressString + "!");
                            break;
                        }
                    }

                    if (currentEntry == null) // || currentEntry.TimeStamp <= packet.timestamp)
                    {
                        // Denied
                        Debug.Log("Packet from " + packet.packet.SenderIP + " is denied by " + networkAdapterRef.ipAddressString + "");
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
    public string firstTargetIP;

    public PacketWithTimestamp(Packet packet, string firstTargetIP)
    {
        this.packet = packet;
        this.timestamp = Time.time;
        this.firstTargetIP = firstTargetIP;
    }
}
