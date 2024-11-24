using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using System.Collections.Generic;
using UnityEngine;

public class ConnectQuiz : MonoBehaviour
{
    private IMongoDatabase _database;
    private IMongoCollection<BsonDocument> _quizCollection;
    public Quiz quiz;
    private int cNum;
    private void Awake()
    {
        DontDestroyOnLoad(gameObject); // 씬 전환 후에도 유지
    }
    void Start()
    {
        // Quiz 초기화
        cNum = 0;
        quiz = new Quiz();
        quiz.Initialize(); // 리스트 초기화
        // MongoDB 데이터베이스 연결
        ConnectToDatabase("Quiz");

        // 질문 데이터 가져오기
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
            switch (cNum)
        {
            case 0:
                _quizCollection = _database.GetCollection<BsonDocument>("기술");
                break;
            case 1:
                _quizCollection = _database.GetCollection<BsonDocument>("문화");
                break;
            case 2:
                _quizCollection = _database.GetCollection<BsonDocument>("스포츠");
                break;
            case 3:
                _quizCollection = _database.GetCollection<BsonDocument>("역사");
                break;
            case 4:
                _quizCollection = _database.GetCollection<BsonDocument>("영화");
                break;
        }
            if (_quizCollection == null)
            {
            UnityEngine.Debug.LogError("퀴즈 컬렉션을 가져오는 데 실패했습니다.");
            }
    }
    private void UpdateQuizCollection()
    {
        // cNum 값에 따라 컬렉션 설정
        switch (cNum)
        {
            case 0:
                _quizCollection = _database.GetCollection<BsonDocument>("기술");
                break;
            case 1:
                _quizCollection = _database.GetCollection<BsonDocument>("문화");
                break;
            case 2:
                _quizCollection = _database.GetCollection<BsonDocument>("스포츠");
                break;
            case 3:
                _quizCollection = _database.GetCollection<BsonDocument>("역사");
                break;
            case 4:
                _quizCollection = _database.GetCollection<BsonDocument>("영화");
                break;
            default:
                UnityEngine.Debug.LogError("유효하지 않은 카테고리 번호입니다.");
                return;
        }

        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("퀴즈 컬렉션을 가져오는 데 실패했습니다.");
        }
        else
        {
            UnityEngine.Debug.Log($"'{cNum}'에 해당하는 컬렉션이 설정되었습니다.");
        }
    }

    public void ChangeCategory(int categoryNumber)
    {
        cNum = categoryNumber; // cNum 값을 변경
        UpdateQuizCollection(); // 변경된 cNum에 따라 컬렉션 업데이트
        FetchAllQuestions(); // 새로운 카테고리의 질문 가져오기
    }
    private void FetchAllQuestions()
    {
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("퀴즈 컬렉션이 설정되지 않았습니다.");
            return;
        }
        if (quiz.questions.Count > 1) // 리스트의 항목 개수 확인
        {
            quiz.questions.Clear(); // 리스트를 초기화
        }

        UnityEngine.Debug.Log("퀴즈 컬렉션에서 데이터를 가져옵니다...");

        // MongoDB에서 모든 문서 가져오기
        var documents = _quizCollection.Find(Builders<BsonDocument>.Filter.Empty).ToList();

        if (documents == null || documents.Count == 0)
        {
            UnityEngine.Debug.LogWarning("퀴즈 데이터가 없습니다.");
            return;
        }

        foreach (var doc in documents)
        {
            try
            {
                // BsonDocument를 Question으로 변환
                var question = doc.ToObject<Question>();

                if (quiz.questions == null)
                {
                    UnityEngine.Debug.LogError("quiz.questions 리스트가 초기화되지 않았습니다.");
                    return;
                }

                // 질문 리스트에 추가
                quiz.questions.Add(question);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"문서 처리 중 오류 발생: {ex.Message}\n{ex.StackTrace}");
            }
        }

        UnityEngine.Debug.Log($"총 {quiz.questions.Count}개의 질문을 가져왔습니다.");

        // 확인용 질문 출력
        foreach (var q in quiz.questions)
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
    public List<Question> FetchFilteredQuestions(string category, string difficulty, int limit = 10)
    {
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("퀴즈 컬렉션이 설정되지 않았습니다.");
            return null;
        }

        UnityEngine.Debug.Log($"카테고리: {category}, 난이도: {difficulty}에 맞는 데이터를 가져옵니다...");

        var filter = Builders<BsonDocument>.Filter.Empty;

        // 카테고리 필터 추가
        if (!string.IsNullOrEmpty(category))
        {
            filter &= Builders<BsonDocument>.Filter.Eq("category", category);
        }

        // 난이도 필터 추가
        if (!string.IsNullOrEmpty(difficulty))
        {
            filter &= Builders<BsonDocument>.Filter.Eq("difficulty", difficulty);
        }

        // 데이터 가져오기
        var documents = _quizCollection.Find(filter).Limit(limit).ToList();

        var filteredQuestions = new List<Question>();

        foreach (var doc in documents)
        {
            try
            {
                // BsonDocument를 Question 객체로 변환
                var question = doc.ToObject<Question>();
                filteredQuestions.Add(question);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"문서 변환 중 오류 발생: {ex.Message}");
            }
        }

        UnityEngine.Debug.Log($"필터링된 질문 {filteredQuestions.Count}개를 가져왔습니다.");
        return filteredQuestions;
    }
}

// BsonDocument를 특정 객체로 변환하는 확장 메서드
public static class BsonDocumentExtensions
{
    public static T ToObject<T>(this BsonDocument document)
    {
        // _id를 무시하고 역직렬화
        if (document.Contains("_id"))
        {
            document.Remove("_id");
        }
        return BsonSerializer.Deserialize<T>(document);
    }
}