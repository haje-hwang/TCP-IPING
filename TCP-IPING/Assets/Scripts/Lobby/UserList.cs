using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;
using Newtonsoft.Json; // Json.NET 라이브러리 사용
using UnityEngine;
using System;
using System.Diagnostics;

public class UserList : ISingleton<UserList>
{
    private IMongoDatabase _database;                // MongoDB 데이터베이스
    private IMongoCollection<BsonDocument> _userCollection; // MongoDB 컬렉션
    private ConcurrentDictionary<Guid, User> userMap; // 유저 정보를 저장할 ConcurrentDictionary
    
    public void Start()
    {
        // ConcurrentDictionary 초기화
        userMap = new ConcurrentDictionary<Guid, User>();
        ConnectToDatabase();
        // MongoDB에서 모든 유저 정보 JSON 형식으로 로드
        
    }
    public UserList()
    {
        
    }
    private void ConnectToDatabase()
    {
        if (MongoDBAccess.Instance == null || MongoDBAccess.Instance.Client == null)
        {
            UnityEngine.Debug.LogError("MongoDBAccess Singleton 인스턴스가 null입니다!");
            return;
        }

        _database = MongoDBAccess.Instance.Client.GetDatabase("User");
        if (_database == null)
        {
            UnityEngine.Debug.LogError($"데이터베이스 User를 찾을 수 없습니다!");
            return;
        }

        _userCollection = _database.GetCollection<BsonDocument>("None");
        if (_userCollection == null)
        {
            UnityEngine.Debug.LogError($"방 번호 None 을 가져오는 데 실패했습니다!");
        }
        else
        {
            UnityEngine.Debug.Log($"유저 컬렉션 '{_userCollection.CollectionNamespace.CollectionName}' 연결 성공!");
        }

    }
    public void ConnectToDatabase(string roomName)
    {
        if (MongoDBAccess.Instance == null || MongoDBAccess.Instance.Client == null)
        {
            UnityEngine.Debug.LogError("MongoDBAccess Singleton 인스턴스가 null입니다!");
            return;
        }

        _database = MongoDBAccess.Instance.Client.GetDatabase("User");
        if (_database == null)
        {
            UnityEngine.Debug.LogError($"데이터베이스 User를 찾을 수 없습니다!");
            return;
        }
        EnsureCollectionExists(roomName); // 컬렉션 확인 및 생성
        _userCollection = _database.GetCollection<BsonDocument>(roomName);
        if (_userCollection == null)
        {
            UnityEngine.Debug.LogError($"방 번호 {roomName} 을 가져오는 데 실패했습니다!");
        }
        else
        {
            UnityEngine.Debug.Log($"유저 컬렉션 '{_userCollection.CollectionNamespace.CollectionName}' 연결 성공!");
        }
    }

    // MongoDB에서 모든 유저 정보를 JSON 형식으로 가져와 userMap에 저장
    private void FetchAllUsersFromDatabase()
    {
        if (_userCollection == null)
        {
            UnityEngine.Debug.LogError("컬렉션이 초기화되지 않았습니다!");
            return;
        }

        var users = _userCollection.Find(Builders<BsonDocument>.Filter.Empty).ToList();

        foreach (var userDoc in users)
        {
            try
            {
                // BsonDocument를 JSON 문자열로 변환
                string json = userDoc.ToJson();

                // JSON 문자열을 User 객체로 변환
                var user = JsonConvert.DeserializeObject<User>(json);

                // userMap에 추가
                if (user != null)
                {
                    userMap.TryAdd(user.id, user);
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"JSON 처리 중 오류 발생: {ex.Message}\n{ex.StackTrace}");
            }
        }

        UnityEngine.Debug.Log($"총 {users.Count}명의 유저 정보를 JSON 형식으로 로드했습니다.");
    }
    private void EnsureCollectionExists(string collectionName)
    {
        var collectionList = _database.ListCollectionNames().ToList();
        if (!collectionList.Contains(collectionName))
        {
            _database.CreateCollection(collectionName);
            UnityEngine.Debug.Log($"컬렉션 '{collectionName}'이 생성되었습니다.");
        }
        else
        {
            UnityEngine.Debug.Log($"컬렉션 '{collectionName}'이 이미 존재합니다.");
        }
    }
    // 새 유저가 추가될 때
    public User CreateNewUser()
    {
        Guid id = Guid.NewGuid();
        while (userMap.ContainsKey(id)) { id = Guid.NewGuid(); } // userID 중복 방지

        // 새 User 객체 생성
        User newUser = new User(id, "Anonymous");

        // userMap에 추가
        userMap.TryAdd(newUser.id, newUser);

        // MongoDB에 새 유저 저장
        SaveUserToDatabase(newUser);

        return newUser;
    }
    public User CreateNewUser(string name)
    {
        Guid id = Guid.NewGuid();
        while (userMap.ContainsKey(id)) { id = Guid.NewGuid(); } // userID 중복 방지

        // 새 User 객체 생성
        User newUser = new User(id, name);

        // userMap에 추가
        userMap.TryAdd(newUser.id, newUser);

        // MongoDB에 새 유저 저장
        SaveUserToDatabase(newUser);

        return newUser;
    }
    // 유저 정보를 MongoDB에 저장
    private void SaveUserToDatabase(User user)
    {
        // User 객체를 JSON 문자열로 변환
        string json = JsonConvert.SerializeObject(user);

        // JSON 문자열을 BsonDocument로 변환
        var userDoc = BsonDocument.Parse(json);

        // MongoDB에 저장
        _userCollection.InsertOne(userDoc);
    }

    // 유저 정보를 업데이트
    public void UpdateUser(User user)
    {
        userMap.AddOrUpdate(user.id, user, (key, existingVal) => user);

        // User 객체를 JSON 문자열로 변환
        string json = JsonConvert.SerializeObject(user);

        // JSON 문자열을 BsonDocument로 변환
        var userDoc = BsonDocument.Parse(json);

        // MongoDB에서도 업데이트
        var filter = Builders<BsonDocument>.Filter.Eq("id", user.id.ToString());
        var update = new BsonDocument("$set", userDoc);

        _userCollection.UpdateOne(filter, update);
    }

    // 유저 정보를 읽기
    public User ReadUser(Guid id)
    {
        if (userMap.TryGetValue(id, out User user))
        {
            return user;
        }

        // MongoDB에서 유저 정보를 가져옴
        var filter = Builders<BsonDocument>.Filter.Eq("id", id.ToString());
        var userDoc = _userCollection.Find(filter).FirstOrDefault();

        if (userDoc != null)
        {
            try
            {
                // BsonDocument를 JSON 문자열로 변환
                string json = userDoc.ToJson();

                // JSON 문자열을 User 객체로 변환
                user = JsonConvert.DeserializeObject<User>(json);

                // userMap에 추가
                if (user != null)
                {
                    userMap.TryAdd(id, user);
                }
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"JSON 처리 중 오류 발생: {ex.Message}\n{ex.StackTrace}");
            }
        }

        return user; // 유저를 찾을 수 없을 경우 null 반환
    }

    // 유저 정보를 삭제
    public bool DeleteUser(Guid id)
    {
        // userMap에서 삭제
        bool isRemoved = userMap.TryRemove(id, out User removedUser);

        // MongoDB에서도 삭제
        var filter = Builders<BsonDocument>.Filter.Eq("id", id.ToString());
        _userCollection.DeleteOne(filter);

        return isRemoved;
    }
    public void DeleteAllUsers()
    {
        // userMap 비우기
        userMap.Clear();

        // MongoDB에서 모든 유저 삭제
        if (_userCollection != null)
        {
            _userCollection.DeleteMany(Builders<BsonDocument>.Filter.Empty); // 모든 문서 삭제
            UnityEngine.Debug.Log("모든 유저가 삭제되었습니다.");
        }
        else
        {
            UnityEngine.Debug.LogError("유저 컬렉션이 초기화되지 않았습니다.");
        }
    }

}

