using Newtonsoft.Json;

public class JsonHelper<T>
{
    //감싸진 객체의 타입도 표시함
    static JsonSerializerSettings settings = new JsonSerializerSettings { TypeNameHandling = TypeNameHandling.None };

    // 패킷을 JSON 문자열로 직렬화
    public static string Serialize(T packet)
    {
        return JsonConvert.SerializeObject(packet, Newtonsoft.Json.Formatting.Indented, settings);
    }

    // JSON 문자열을 패킷으로 역직렬화
    public static T? Deserialize(string json)
    {
        return JsonConvert.DeserializeObject<T>(json, settings);
    }
}