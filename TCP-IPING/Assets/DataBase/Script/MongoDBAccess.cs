using MongoDB.Driver;
using UnityEngine;

public class MongoDBAccess : MonoBehaviour
{
    public static MongoDBAccess Instance { get; private set; }
    private MongoClient _client;

    public MongoClient Client => _client;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            UnityEngine.Debug.Log("MongoDBAccess Singleton 인스턴스가 초기화되었습니다.");
        }
        else
        {
            UnityEngine.Debug.LogError("MongoDBAccess 인스턴스가 중복으로 생성되었습니다.");
            Destroy(gameObject);
        }
    }

    void Start()
    {
        ConnectToClient();
    }

    private void ConnectToClient()
    {
        try
        {
            string connectionString = "mongodb+srv://TcpAdmin:TcpIping@tcpiping.v8bub.mongodb.net/?retryWrites=true&w=majority&appName=TCPIPING";
            _client = new MongoClient(connectionString);
            UnityEngine.Debug.Log("MongoDB 클라이언트 연결 성공!");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"MongoDB 클라이언트 연결 중 오류 발생: {e.Message}");
        }
    }
}