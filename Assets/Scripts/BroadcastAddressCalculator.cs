using UnityEngine;

public static class BroadcastAddressCalculator
{
    public static string CalculateBroadcastAddress(string ipAddress, string subnetMask)
    {
        uint ip = ParseIpAddress(ipAddress);
        uint mask = ParseIpAddress(subnetMask);

        uint networkAddress = ip & mask;
        uint broadcastAddress = networkAddress | ~mask;

        return ConvertIpToString(broadcastAddress);
    }

    private static uint ParseIpAddress(string ipAddress)
    {
        string[] parts = ipAddress.Split('.');
        if (parts.Length != 4)
        {
            Debug.LogError("Invalid IP address format");
        }

        uint result = 0;
        for (int i = 0; i < 4; i++)
        {
            result <<= 8;
            result += uint.Parse(parts[i]);
        }

        return result;
    }

    private static string ConvertIpToString(uint ip)
    {
        byte[] bytes = new byte[4];
        bytes[0] = (byte)((ip >> 24) & 0xFF);
        bytes[1] = (byte)((ip >> 16) & 0xFF);
        bytes[2] = (byte)((ip >> 8) & 0xFF);
        bytes[3] = (byte)(ip & 0xFF);

        return string.Join(".", bytes);
    }
}