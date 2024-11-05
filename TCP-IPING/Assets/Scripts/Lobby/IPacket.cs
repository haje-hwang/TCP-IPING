using System;

[Serializable]
public class IPacket
{
    public string PacketType { get; set; } // 패킷의 유형
    public object Data { get; set; }        // 전송할 데이터
    public IPacket(string packetType, object data)
    {
        PacketType = packetType;
        Data = data;
    }
}
