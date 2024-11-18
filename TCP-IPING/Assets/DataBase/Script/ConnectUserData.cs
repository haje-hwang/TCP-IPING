using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class ConnectUser : MonoBehaviour
{
    private IMongoDatabase _database;
    private IMongoCollection<BsonDocument> _Collection;

    void Start()
    {
        ConnectToDatabase("User"); // �����ͺ��̽� �̸� ����
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
        _Collection = _database.GetCollection<BsonDocument>("User");
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
        foreach (var user in users)
        {
            //�ϴ��� ����� �α׷� ��� 
            UnityEngine.Debug.Log($"���̵�: {user["id"]}");
            UnityEngine.Debug.Log($"�̸�: {user["nickName"]}");
        }
    }
}
