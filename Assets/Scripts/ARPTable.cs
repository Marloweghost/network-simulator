using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ARPTable : MonoBehaviour
{
    private float checkInterval = 1f;
    private float entryTTL = 10f;

    private Dictionary<string, ARPEntry> _arpTable;

    public ARPTable()
    {
        _arpTable = new Dictionary<string, ARPEntry>();
    }

    private void Start()
    {
        StartCoroutine(CheckDictionaryCoroutine());
    }

    public void AddEntry(string ipAddress, string macAddress, int portNumber, float timeStamp)
    {
        ARPEntry entry = new ARPEntry(ipAddress, macAddress, portNumber, timeStamp);
        // Debug.Log(entry.TimeStamp);
        if (!_arpTable.ContainsKey(ipAddress))
        {
            _arpTable.Add(ipAddress, entry);
        }
        else
        {
            Debug.LogWarning("IP address already exists in ARP table");
        }
    }

    public void RemoveEntry(string ipAddress)
    {
        if (_arpTable.ContainsKey(ipAddress))
        {
            _arpTable.Remove(ipAddress);
        }
        else
        {
            Debug.LogWarning("IP address not found in ARP table");
        }
    }

    public ARPEntry GetEntry(string ipAddress)
    {
        if (_arpTable.ContainsKey(ipAddress))
        {
            return _arpTable[ipAddress];
        }
        else
        {
            Debug.LogWarning("IP address not found in ARP table");
            return null;
        }
    }

    public IEnumerator CheckDictionaryCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(checkInterval);

            List<string> expiredKeys = new List<string>();

            foreach (KeyValuePair<string, ARPEntry> entry in _arpTable)
            {
                // Debug.Log(entry.Value.TimeStamp);
                if (IsEntryExpired(entry.Value) == true)
                {
                    expiredKeys.Add(entry.Key);
                }
            }

            foreach (string ipAddress in expiredKeys)
            {
                _arpTable.Remove(ipAddress);
            }
        }
    }

    private bool IsEntryExpired(ARPEntry entry)
    {
        float timeSinceEntryUpdate = Time.time - entry.TimeStamp;
        //Debug.Log(timeSinceEntryUpdate);
        return timeSinceEntryUpdate > entryTTL;
    }
}

public class ARPEntry
{
    public string IPAddress { get; set; }
    public string MACAddress { get; set; }
    public int PortNumber { get; set; } // или string, если это необходимо
    public float TimeStamp { get; set; }

    public ARPEntry(string ipAddress, string macAddress, int portNumber, float timeStamp)
    {
        IPAddress = ipAddress;
        MACAddress = macAddress;
        PortNumber = portNumber;
        TimeStamp = timeStamp;
    }
}