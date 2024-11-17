using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json; // Json.NET 라이브러리 사용

public class ConnectQuiz : MonoBehaviour
{
    private IMongoDatabase _database;
    private IMongoCollection<BsonDocument> _quizCollection;
    public List<Question> questions;

    void Start()
    {
        // 질문 리스트 초기화
        questions = new List<Question>();

        // MongoDB 데이터베이스 연결
        ConnectToDatabase("Quiz");

        // JSON으로 질문 데이터 가져오기
        FetchAllQuestionsAsJson();
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

        _quizCollection = _database.GetCollection<BsonDocument>("기술");
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("퀴즈 컬렉션을 가져오는 데 실패했습니다.");
        }
    }

    private void FetchAllQuestionsAsJson()
    {
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("퀴즈 컬렉션이 설정되지 않았습니다.");
            return;
        }

        UnityEngine.Debug.Log("퀴즈 컬렉션에서 데이터를 JSON 형식으로 가져옵니다...");

        // MongoDB에서 모든 문서를 JSON 문자열로 가져오기
        var documents = _quizCollection.Find(Builders<BsonDocument>.Filter.Empty).ToList();

        foreach (var doc in documents)
        {
            try
            {
                // BsonDocument를 JSON 문자열로 변환
                string json = doc.ToJson();

                // JSON 문자열을 Question 객체로 변환 (Newtonsoft.Json 사용)
                var question = JsonConvert.DeserializeObject<Question>(json);

                if (questions == null)
                {
                    UnityEngine.Debug.LogError("questions 리스트가 초기화되지 않았습니다.");
                    return;
                }

                // 질문 리스트에 추가
                questions.Add(question);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"JSON 처리 중 오류 발생: {ex.Message}\n{ex.StackTrace}");
            }
        }

        UnityEngine.Debug.Log($"총 {questions.Count}개의 질문을 JSON 형식으로 처리했습니다.");

        // 확인용 질문 출력
        foreach (var q in questions)
        {
            UnityEngine.Debug.Log($"ID: {q.id}");
            UnityEngine.Debug.Log($"Question: {q.question}");
            UnityEngine.Debug.Log($"Options: {string.Join(", ", q.options)}");
            UnityEngine.Debug.Log($"Answer Index: {q.answer}");
            UnityEngine.Debug.Log($"Category: {q.category}");
            UnityEngine.Debug.Log($"Difficulty: {q.difficulty}");
            UnityEngine.Debug.Log("---------------");
        }
    }
}