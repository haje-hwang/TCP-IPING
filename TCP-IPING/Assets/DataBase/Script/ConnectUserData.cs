using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
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
        var collectionNames = _database.ListCollectionNames().ToList();

        if (!collectionNames.Contains(collectionName))
        {
            _database.CreateCollection(collectionName);
            UnityEngine.Debug.Log($"�÷��� '{collectionName}'�� �����Ǿ����ϴ�.");
        }

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

        var users = _Collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
        StringBuilder sb = new StringBuilder();
        foreach (var user in users)
        {
            sb.Append($"���̵�: {user["id"]} / ");
            sb.AppendLine($"�̸�: {user["nickName"]}");
        }
        UnityEngine.Debug.Log(sb);
    }

    /// <summary>
/// ����� ������ ������Ʈ�ϰų� �� ����ڷ� �߰� (���� �г��� ���)
/// </summary>
/// <param name="nickName">����� �г���</param>
/// <param name="score">����� ����</param>
public void UpdateUserScore(string nickName, int score)
{
    if (_Collection == null)
    {
        UnityEngine.Debug.LogError("���� �÷����� �������� �ʾҽ��ϴ�.");
        return;
    }

    // �г������� ���͸�
    var filter = Builders<BsonDocument>.Filter.Eq("nickName", nickName);
    var update = Builders<BsonDocument>.Update
        .Set("nickName", nickName) // �г��� ������Ʈ
        .Set("score", score); // ���� ������Ʈ

    try
    {
        var result = _Collection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

        if (result.ModifiedCount > 0)
        {
            UnityEngine.Debug.Log($"����� '{nickName}'�� ������ {score}�� ������Ʈ�Ǿ����ϴ�.");
        }
        else if (result.UpsertedId != null)
        {
            UnityEngine.Debug.Log($"���ο� ����� '{nickName}'�� �߰��Ǿ����ϴ�.");
        }
    }
    catch (System.Exception ex)
    {
        UnityEngine.Debug.LogError($"���� ������Ʈ �� ���� �߻�: {ex.Message}");
    }
}

    public List<(string nickName, int score)> GetTopRankings(string collectionName, int limit)
    {
        if (_Collection == null)
        {
            Debug.LogError("�÷����� �������� �ʾҽ��ϴ�.");
            return new List<(string, int)>();
        }

        var rankings = new List<(string nickName, int score)>();

        try
        {
            // MongoDB���� ���� �������� �������� ���� �� ���� n�� ��������
            var filter = Builders<BsonDocument>.Filter.Empty;
            var sort = Builders<BsonDocument>.Sort.Descending("score");

            var documents = _Collection.Find(filter).Sort(sort).Limit(limit).ToList();

            foreach (var doc in documents)
            {
                string nickName = doc.Contains("nickName") ? doc["nickName"].AsString : "Unknown";
                int score = doc.Contains("score") ? doc["score"].AsInt32 : 0;

                rankings.Add((nickName, score));
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"��ŷ ������ �������� �� ���� �߻�: {ex.Message}");
        }

        return rankings;
    }
    public void DeleteAllUsers()
    {
        if (_Collection == null)
        {
            Debug.LogError("�÷����� �������� �ʾҽ��ϴ�.");
            return;
        }

        try
        {
            var result = _Collection.DeleteMany(Builders<BsonDocument>.Filter.Empty);
            Debug.Log($"�÷��� '{roomName}'���� ��� ���� �����Ͱ� �����Ǿ����ϴ�. ������ ���� ��: {result.DeletedCount}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"��� ���� �����͸� �����ϴ� �� ���� �߻�: {ex.Message}");
        }
    }


}
