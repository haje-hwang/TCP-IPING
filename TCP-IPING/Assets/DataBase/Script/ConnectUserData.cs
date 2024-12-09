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
        // 데이터베이스의 모든 컬렉션 이름 가져오기
        var collectionNames = _database.ListCollectionNames().ToList();

        if (!collectionNames.Contains(collectionName))
        {
            // 컬렉션이 없으면 생성
            _database.CreateCollection(collectionName);
            UnityEngine.Debug.Log($"컬렉션 '{collectionName}'이 생성되었습니다.");
        }

        // 컬렉션 가져오기
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

        // 모든 유저 정보 가져오기
        var users = _Collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
        StringBuilder sb = new StringBuilder();
        foreach (var user in users)
        {
            // 디버그 로그로 출력
            sb.Append($"아이디: {user["id"]} / ");
            sb.AppendLine($"이름: {user["nickName"]}");
        }
        UnityEngine.Debug.Log(sb);
    }
}
