using Newtonsoft.Json;

public class JsonHelper<T>
{
    // 패킷을 JSON 문자열로 직렬화
    public static string Serialize(T packet)
    {
        return JsonConvert.SerializeObject(packet);
    }

    // JSON 문자열을 패킷으로 역직렬화
    public static T? Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<T>(json);
    }
}