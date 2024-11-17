using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Linq;

public class UserList : ISingleton<UserList>
{
    private ConcurrentDictionary<uint, User> userMap; // 유저 정보를 저장할 ConcurrentDictionary
    private IMongoDatabase _database;                // MongoDB 데이터베이스
    private IMongoCollection<BsonDocument> _userCollection; // MongoDB 컬렉션

    public UserList()
    {
        // ConcurrentDictionary 초기화
        userMap = new ConcurrentDictionary<uint, User>();

        // MongoDB 데이터베이스 연결
        ConnectToDatabase("Quiz");

        // MongoDB에서 모든 유저 정보 로드
        FetchAllUsersFromDatabase();
    }

    // MongoDB 데이터베이스 연결
    private void ConnectToDatabase(string databaseName)
    {
        _database = MongoDBAccess.Instance.Client.GetDatabase(databaseName);
        if (MongoDBAccess.Instance == null || MongoDBAccess.Instance.Client == null)
        {
            UnityEngine.Debug.LogError("MongoDBAccess Singleton 인스턴스를 찾을 수 없습니다!");
            return;
        }
        _userCollection = _database.GetCollection<BsonDocument>("User");
        if (_userCollection == null)
        {
            UnityEngine.Debug.LogError("유저 정보를 연결하는데 실패했습니다.");
        }
    }

    // MongoDB에서 모든 유저 정보를 가져와 userMap에 저장
    private void FetchAllUsersFromDatabase()
    {
        var users = _userCollection.Find(Builders<BsonDocument>.Filter.Empty).ToList();

        foreach (var userDoc in users)
        {
            uint id = (uint)userDoc["id"].AsInt32; // MongoDB에서 가져온 ID
            string name = userDoc["Name"].AsString;

            // User 객체 생성 및 userMap에 추가
            User user = new User(id, name);
            userMap.TryAdd(id, user);
        }
    }

    // 새 유저가 추가될 때
    public User CreateNewUser()
    {
        UID id = UID.NewUID();
        while (userMap.ContainsKey(id)) { id = UID.NewUID(); } // userID 중복 방지

        User newUser = new User(id, "Anonymous");
        userMap.TryAdd(newUser.id, newUser);

        // MongoDB에 새 유저 저장
        SaveUserToDatabase(newUser);

        return newUser;
    }

    // 유저 정보를 MongoDB에 저장
    private void SaveUserToDatabase(User user)
    {
        var userDoc = new BsonDocument
        {
            { "id", user.id },
            { "Name", user.nickName }
        };

        _userCollection.InsertOne(userDoc);
    }

    // 유저 정보를 업데이트
    public void UpdateUser(User user)
    {
        userMap.AddOrUpdate(user.id, user, (key, existingVal) => user);

        // MongoDB에서도 업데이트
        var filter = Builders<BsonDocument>.Filter.Eq("id", user.id);
        var update = Builders<BsonDocument>.Update.Set("Name", user.nickName);

        _userCollection.UpdateOne(filter, update);
    }

    // 유저 정보를 읽기
    public User ReadUser(uint id)
    {
        if (userMap.TryGetValue(id, out User user))
        {
            return user;
        }

        // MongoDB에서 유저 정보를 가져옴
        var filter = Builders<BsonDocument>.Filter.Eq("id", id);
        var userDoc = _userCollection.Find(filter).FirstOrDefault();

        if (userDoc != null)
        {
            string name = userDoc["Name"].AsString;
            User newUser = new User(id, name);

            // userMap에 추가
            userMap.TryAdd(id, newUser);

            return newUser;
        }

        return null; // 유저를 찾을 수 없을 경우
    }

    // 유저 정보를 삭제
    public bool DeleteUser(uint id)
    {
        // userMap에서 삭제
        bool isRemoved = userMap.TryRemove(id, out User removedUser);

        // MongoDB에서도 삭제
        var filter = Builders<BsonDocument>.Filter.Eq("id", id);
        _userCollection.DeleteOne(filter);

        return isRemoved;
    }
}
