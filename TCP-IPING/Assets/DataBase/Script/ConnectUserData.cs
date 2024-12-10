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
        DontDestroyOnLoad(gameObject); // 씬 전환 후에도 유지
    }

    void Start()
    {
        ConnectToDatabase("User"); // 데이터베이스 이름 지정
        roomName = PlayerPrefs.GetString("RoomName", "None");
        EnsureCollectionExists(roomName); // 컬렉션 확인 및 생성
        FetchAllUsers();
    }

    private void ConnectToDatabase(string databaseName)
    {
        if (MongoDBAccess.Instance == null || MongoDBAccess.Instance.Client == null)
        {
            UnityEngine.Debug.LogError("MongoDBAccess Singleton 인스턴스를 찾을 수 없습니다!");
            return;
        }

        _database = MongoDBAccess.Instance.Client.GetDatabase(databaseName);
        UnityEngine.Debug.Log($"데이터베이스 '{databaseName}' 연결 성공!");
    }

    private void EnsureCollectionExists(string collectionName)
    {
        var collectionNames = _database.ListCollectionNames().ToList();

        if (!collectionNames.Contains(collectionName))
        {
            _database.CreateCollection(collectionName);
            UnityEngine.Debug.Log($"컬렉션 '{collectionName}'이 생성되었습니다.");
        }

        _Collection = _database.GetCollection<BsonDocument>(collectionName);
        UnityEngine.Debug.Log($"컬렉션 '{collectionName}' 연결 성공!");
    }

    private void FetchAllUsers()
    {
        if (_Collection == null)
        {
            UnityEngine.Debug.LogError("유저 컬렉션이 설정되지 않았습니다");
            return;
        }

        var users = _Collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
        StringBuilder sb = new StringBuilder();
        foreach (var user in users)
        {
            sb.Append($"아이디: {user["id"]} / ");
            sb.AppendLine($"이름: {user["nickName"]}");
        }
        UnityEngine.Debug.Log(sb);
    }

    /// <summary>
/// 사용자 점수를 업데이트하거나 새 사용자로 추가 (유저 닉네임 기반)
/// </summary>
/// <param name="nickName">사용자 닉네임</param>
/// <param name="score">사용자 점수</param>
public void UpdateUserScore(string nickName, int score)
{
    if (_Collection == null)
    {
        UnityEngine.Debug.LogError("유저 컬렉션이 설정되지 않았습니다.");
        return;
    }

    // 닉네임으로 필터링
    var filter = Builders<BsonDocument>.Filter.Eq("nickName", nickName);
    var update = Builders<BsonDocument>.Update
        .Set("nickName", nickName) // 닉네임 업데이트
        .Set("score", score); // 점수 업데이트

    try
    {
        var result = _Collection.UpdateOne(filter, update, new UpdateOptions { IsUpsert = true });

        if (result.ModifiedCount > 0)
        {
            UnityEngine.Debug.Log($"사용자 '{nickName}'의 점수가 {score}로 업데이트되었습니다.");
        }
        else if (result.UpsertedId != null)
        {
            UnityEngine.Debug.Log($"새로운 사용자 '{nickName}'이 추가되었습니다.");
        }
    }
    catch (System.Exception ex)
    {
        UnityEngine.Debug.LogError($"점수 업데이트 중 오류 발생: {ex.Message}");
    }
}

    public List<(string nickName, int score)> GetTopRankings(string collectionName, int limit)
    {
        if (_Collection == null)
        {
            Debug.LogError("컬렉션이 설정되지 않았습니다.");
            return new List<(string, int)>();
        }

        var rankings = new List<(string nickName, int score)>();

        try
        {
            // MongoDB에서 점수 기준으로 내림차순 정렬 후 상위 n개 가져오기
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
            Debug.LogError($"랭킹 정보를 가져오는 중 오류 발생: {ex.Message}");
        }

        return rankings;
    }
    public void DeleteAllUsers()
    {
        if (_Collection == null)
        {
            Debug.LogError("컬렉션이 설정되지 않았습니다.");
            return;
        }

        try
        {
            var result = _Collection.DeleteMany(Builders<BsonDocument>.Filter.Empty);
            Debug.Log($"컬렉션 '{roomName}'에서 모든 유저 데이터가 삭제되었습니다. 삭제된 문서 수: {result.DeletedCount}");
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"모든 유저 데이터를 삭제하는 중 오류 발생: {ex.Message}");
        }
    }


}
