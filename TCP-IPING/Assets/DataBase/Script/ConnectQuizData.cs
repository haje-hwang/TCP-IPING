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

    void Start()
    {
        // Quiz �ʱ�ȭ
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

        _quizCollection = _database.GetCollection<BsonDocument>("���");
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("���� �÷����� �������� �� �����߽��ϴ�.");
        }
    }

    private void FetchAllQuestions()
    {
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("���� �÷����� �������� �ʾҽ��ϴ�.");
            return;
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