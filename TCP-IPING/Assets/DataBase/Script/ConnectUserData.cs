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
        ConnectToDatabase("User"); // 데이터베이스 이름 지정
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
        _Collection = _database.GetCollection<BsonDocument>("User");
    }

    private void FetchAllUsers()
    {
        if (_Collection == null)
        {
            UnityEngine.Debug.LogError("퀴즈 컬렉션이 설정되지 않았습니다");
            return;
        }

        // 모든 유저 정보 가져오기
        var users = _Collection.Find(Builders<BsonDocument>.Filter.Empty).ToList();
        foreach (var user in users)
        {
            //일단은 디버그 로그로 출력 
            UnityEngine.Debug.Log($"아이디: {user["id"]}");
            UnityEngine.Debug.Log($"이름: {user["nickName"]}");
        }
    }
}
