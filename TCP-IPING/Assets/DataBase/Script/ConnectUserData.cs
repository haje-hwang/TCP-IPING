using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using UnityEngine;

public class ConnectUser : MonoBehaviour
{
    private IMongoDatabase _database;
    private IMongoCollection<BsonDocument> _Collection;
    private string roomName;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // �� ��ȯ �Ŀ��� ����
    }

    void Start()
    {
        ConnectToDatabase("User"); // �����ͺ��̽� �̸� ����
        roomName = PlayerPrefs.GetString("RoomName", "None");
        EnsureCollectionExists(roomName); // �÷��� Ȯ�� �� ����
        FetchAllUsers();
    }

    private void ConnectToDatabase(string databaseName)
    {
        if (MongoDBAccess.Instance == null || MongoDBAccess.Instance.Client == null)
        {
            UnityEngine.Debug.LogError("MongoDBAccess Singleton �ν��Ͻ��� ã�� �� �����ϴ�!");
            return;
        }

        _database = MongoDBAccess.Instance.Client.GetDatabase(databaseName);
        UnityEngine.Debug.Log($"�����ͺ��̽� '{databaseName}' ���� ����!");
    }

    private void EnsureCollectionExists(string collectionName)
    {
        // �����ͺ��̽��� ��� �÷��� �̸� ��������
        var collectionNames = _database.ListCollectionNames().ToList();

        if (!collectionNames.Contains(collectionName))
        {
            // �÷����� ������ ����
            _database.CreateCollection(collectionName);
            UnityEngine.Debug.Log($"�÷��� '{collectionName}'�� �����Ǿ����ϴ�.");
        }

        // �÷��� ��������
        _Collection = _database.GetCollection<BsonDocument>(collectionName);
        UnityEngine.Debug.Log($"�÷��� '{collectionName}' ���� ����!");
    }

    private void FetchAllUsers()
    {
        if (_Collection == null)
        {
            UnityEngine.Debug.LogError("���� �÷����� �������� �ʾҽ��ϴ�");
            return;
        }

        // ��� ���� ���� ��������
        var users = _Collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
        StringBuilder sb = new StringBuilder();
        foreach (var user in users)
        {
            // ����� �α׷� ���
            sb.Append($"���̵�: {user["id"]} / ");
            sb.AppendLine($"�̸�: {user["nickName"]}");
        }
        UnityEngine.Debug.Log(sb);
    }
}
