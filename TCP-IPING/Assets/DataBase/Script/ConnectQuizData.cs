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
        DontDestroyOnLoad(gameObject); // �� ��ȯ �Ŀ��� ����
    }
    void Start()
    {
        // Quiz �ʱ�ȭ
        cNum = 0;
        quiz = new Quiz();
        quiz.Initialize(); // ����Ʈ �ʱ�ȭ
        // MongoDB �����ͺ��̽� ����
        ConnectToDatabase("Quiz");

        // ���� ������ ��������
        FetchAllQuestions();
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
            switch (cNum)
        {
            case 0:
                _quizCollection = _database.GetCollection<BsonDocument>("���");
                break;
            case 1:
                _quizCollection = _database.GetCollection<BsonDocument>("��ȭ");
                break;
            case 2:
                _quizCollection = _database.GetCollection<BsonDocument>("������");
                break;
            case 3:
                _quizCollection = _database.GetCollection<BsonDocument>("����");
                break;
            case 4:
                _quizCollection = _database.GetCollection<BsonDocument>("��ȭ");
                break;
        }
            if (_quizCollection == null)
            {
            UnityEngine.Debug.LogError("���� �÷����� �������� �� �����߽��ϴ�.");
            }
    }
    private void UpdateQuizCollection()
    {
        // cNum ���� ���� �÷��� ����
        switch (cNum)
        {
            case 0:
                _quizCollection = _database.GetCollection<BsonDocument>("���");
                break;
            case 1:
                _quizCollection = _database.GetCollection<BsonDocument>("��ȭ");
                break;
            case 2:
                _quizCollection = _database.GetCollection<BsonDocument>("������");
                break;
            case 3:
                _quizCollection = _database.GetCollection<BsonDocument>("����");
                break;
            case 4:
                _quizCollection = _database.GetCollection<BsonDocument>("��ȭ");
                break;
            default:
                UnityEngine.Debug.LogError("��ȿ���� ���� ī�װ� ��ȣ�Դϴ�.");
                return;
        }

        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("���� �÷����� �������� �� �����߽��ϴ�.");
        }
        else
        {
            UnityEngine.Debug.Log($"'{cNum}'�� �ش��ϴ� �÷����� �����Ǿ����ϴ�.");
        }
    }

    public void ChangeCategory(int categoryNumber)
    {
        cNum = categoryNumber; // cNum ���� ����
        UpdateQuizCollection(); // ����� cNum�� ���� �÷��� ������Ʈ
        FetchAllQuestions(); // ���ο� ī�װ��� ���� ��������
    }
    private void FetchAllQuestions()
    {
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("���� �÷����� �������� �ʾҽ��ϴ�.");
            return;
        }
        if (quiz.questions.Count > 1) // ����Ʈ�� �׸� ���� Ȯ��
        {
            quiz.questions.Clear(); // ����Ʈ�� �ʱ�ȭ
        }

        UnityEngine.Debug.Log("���� �÷��ǿ��� �����͸� �����ɴϴ�...");

        // MongoDB���� ��� ���� ��������
        var documents = _quizCollection.Find(Builders<BsonDocument>.Filter.Empty).ToList();

        if (documents == null || documents.Count == 0)
        {
            UnityEngine.Debug.LogWarning("���� �����Ͱ� �����ϴ�.");
            return;
        }

        foreach (var doc in documents)
        {
            try
            {
                // BsonDocument�� Question���� ��ȯ
                var question = doc.ToObject<Question>();

                if (quiz.questions == null)
                {
                    UnityEngine.Debug.LogError("quiz.questions ����Ʈ�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
                    return;
                }

                // ���� ����Ʈ�� �߰�
                quiz.questions.Add(question);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"���� ó�� �� ���� �߻�: {ex.Message}\n{ex.StackTrace}");
            }
        }

        UnityEngine.Debug.Log($"�� {quiz.questions.Count}���� ������ �����Խ��ϴ�.");

        // Ȯ�ο� ���� ���
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
            UnityEngine.Debug.LogError("���� �÷����� �������� �ʾҽ��ϴ�.");
            return null;
        }

        UnityEngine.Debug.Log($"ī�װ�: {category}, ���̵�: {difficulty}�� �´� �����͸� �����ɴϴ�...");

        var filter = Builders<BsonDocument>.Filter.Empty;

        // ī�װ� ���� �߰�
        if (!string.IsNullOrEmpty(category))
        {
            filter &= Builders<BsonDocument>.Filter.Eq("category", category);
        }

        // ���̵� ���� �߰�
        if (!string.IsNullOrEmpty(difficulty))
        {
            filter &= Builders<BsonDocument>.Filter.Eq("difficulty", difficulty);
        }

        // ������ ��������
        var documents = _quizCollection.Find(filter).Limit(limit).ToList();

        var filteredQuestions = new List<Question>();

        foreach (var doc in documents)
        {
            try
            {
                // BsonDocument�� Question ��ü�� ��ȯ
                var question = doc.ToObject<Question>();
                filteredQuestions.Add(question);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"���� ��ȯ �� ���� �߻�: {ex.Message}");
            }
        }

        UnityEngine.Debug.Log($"���͸��� ���� {filteredQuestions.Count}���� �����Խ��ϴ�.");
        return filteredQuestions;
    }
}

// BsonDocument�� Ư�� ��ü�� ��ȯ�ϴ� Ȯ�� �޼���
public static class BsonDocumentExtensions
{
    public static T ToObject<T>(this BsonDocument document)
    {
        // _id�� �����ϰ� ������ȭ
        if (document.Contains("_id"))
        {
            document.Remove("_id");
        }
        return BsonSerializer.Deserialize<T>(document);
    }
}