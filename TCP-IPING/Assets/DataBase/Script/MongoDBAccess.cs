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
            UnityEngine.Debug.Log("MongoDBAccess Singleton �ν��Ͻ��� �ʱ�ȭ�Ǿ����ϴ�.");
        }
        else
        {
            UnityEngine.Debug.LogError("MongoDBAccess �ν��Ͻ��� �ߺ����� �����Ǿ����ϴ�.");
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
            UnityEngine.Debug.Log("MongoDB Ŭ���̾�Ʈ ���� ����!");
        }
        catch (System.Exception e)
        {
            UnityEngine.Debug.LogError($"MongoDB Ŭ���̾�Ʈ ���� �� ���� �߻�: {e.Message}");
        }
    }
}