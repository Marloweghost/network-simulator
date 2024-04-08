using System.Net;

public struct Packet
{
    private string _verificationMessage;
    private int _messageType;
    private byte[] _payload;
    private string _senderIP;
    private string _targetIP;
    private string _macAddress;
    private int _ttl;

    public string VerificationMessage
    {
        get { return _verificationMessage; }
        set { _verificationMessage = value; }
    }

    public int MessageType
    {
        get { return _messageType; }
        set { _messageType = value; }
    }

    public byte[] Payload
    {
        get { return _payload; }
        set { _payload = value; }
    }

    public string SenderIP
    {
        get { return _senderIP; }
        set { _senderIP = value; }
    }

    public string TargetIP
    {
        get { return _targetIP; }
        set { _targetIP = value; }
    }

    public string MACAddress
    {
        get { return _macAddress; }
        set { _macAddress = value; }
    }

    public int TTL
    {
        get { return _ttl; }
        set { _ttl = value; }
    }

    public Packet(int messageType, string verificationMessage, byte[] payload, string senderIP, string targetIP, string macAddress)
    {
        _messageType = messageType;
        _verificationMessage = verificationMessage;
        _payload = payload;
        _senderIP = senderIP;
        _targetIP = targetIP;
        _macAddress = macAddress;
        _ttl = 128;
    }

    public Packet(int messageType, string verificationMessage, string senderIP, string targetIP, string macAddress)
    {
        _messageType = messageType;
        _verificationMessage = verificationMessage;
        _payload = default;
        _senderIP = senderIP;
        _targetIP = targetIP;
        _macAddress = macAddress;
        _ttl = 128;
    }
}
