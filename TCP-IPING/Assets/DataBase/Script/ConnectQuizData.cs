using MongoDB.Bson;
using MongoDB.Driver;
using System.Collections.Generic;
using UnityEngine;
using Newtonsoft.Json; // Json.NET ���̺귯�� ���

public class ConnectQuiz : MonoBehaviour
{
    private IMongoDatabase _database;
    private IMongoCollection<BsonDocument> _quizCollection;
    public List<Question> questions;

    void Start()
    {
        // ���� ����Ʈ �ʱ�ȭ
        questions = new List<Question>();

        // MongoDB �����ͺ��̽� ����
        ConnectToDatabase("Quiz");

        // JSON���� ���� ������ ��������
        FetchAllQuestionsAsJson();
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

    private void FetchAllQuestionsAsJson()
    {
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("���� �÷����� �������� �ʾҽ��ϴ�.");
            return;
        }

        UnityEngine.Debug.Log("���� �÷��ǿ��� �����͸� JSON �������� �����ɴϴ�...");

        // MongoDB���� ��� ������ JSON ���ڿ��� ��������
        var documents = _quizCollection.Find(Builders<BsonDocument>.Filter.Empty).ToList();

        foreach (var doc in documents)
        {
            try
            {
                // BsonDocument�� JSON ���ڿ��� ��ȯ
                string json = doc.ToJson();

                // JSON ���ڿ��� Question ��ü�� ��ȯ (Newtonsoft.Json ���)
                var question = JsonConvert.DeserializeObject<Question>(json);

                if (questions == null)
                {
                    UnityEngine.Debug.LogError("questions ����Ʈ�� �ʱ�ȭ���� �ʾҽ��ϴ�.");
                    return;
                }

                // ���� ����Ʈ�� �߰�
                questions.Add(question);
            }
            catch (System.Exception ex)
            {
                UnityEngine.Debug.LogError($"JSON ó�� �� ���� �߻�: {ex.Message}\n{ex.StackTrace}");
            }
        }

        UnityEngine.Debug.Log($"�� {questions.Count}���� ������ JSON �������� ó���߽��ϴ�.");

        // Ȯ�ο� ���� ���
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