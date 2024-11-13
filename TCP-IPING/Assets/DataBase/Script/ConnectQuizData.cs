using MongoDB.Bson;
using MongoDB.Driver;
using System.Diagnostics;
using UnityEngine;

public class ConnectQuiz : MonoBehaviour
{
    private IMongoDatabase _database;
    private IMongoCollection<BsonDocument> _quizCollection;

    void Start()
    {
        ConnectToDatabase("Quiz"); // 데이터베이스 이름 지정
        FetchAllQuestions();
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

        _quizCollection = _database.GetCollection<BsonDocument>("quiz_collection");
    }

    private void FetchAllQuestions()
    {
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("퀴즈 컬렉션이 설정되지 않았습니다.");
            return;
        }

        // 모든 문서 가져오기
        var questions = _quizCollection.Find(Builders<BsonDocument>.Filter.Empty).ToList();

        foreach (var question in questions)
        {
            UnityEngine.Debug.Log($"질문: {question["question"]}");
            UnityEngine.Debug.Log($"보기: {question["options"]}");
            UnityEngine.Debug.Log($"정답: {question["answer"]}");
        }
    }
}
