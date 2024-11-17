using Newtonsoft.Json;
public class PacketHelper
{
    // 패킷을 JSON 문자열로 직렬화
    public static string Serialize(IPacket packet)
    {
        return JsonConvert.SerializeObject(packet);
    }

    // JSON 문자열을 패킷으로 역직렬화
    public static IPacket Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<IPacket>(json);
    }

    public static bool isBidirectional(PacketType type) => type switch
    {
        PacketType.__FirstJoin => true,
        PacketType.__LobbyData => true,
        PacketType.__LobbyList => true,
        _ => false
    };
}
