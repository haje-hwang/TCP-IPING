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
        ConnectToDatabase("Quiz"); // �����ͺ��̽� �̸� ����
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

        _quizCollection = _database.GetCollection<BsonDocument>("quiz_collection");
    }

    private void FetchAllQuestions()
    {
        if (_quizCollection == null)
        {
            UnityEngine.Debug.LogError("���� �÷����� �������� �ʾҽ��ϴ�.");
            return;
        }

        // ��� ���� ��������
        var questions = _quizCollection.Find(Builders<BsonDocument>.Filter.Empty).ToList();

        foreach (var question in questions)
        {
            UnityEngine.Debug.Log($"����: {question["question"]}");
            UnityEngine.Debug.Log($"����: {question["options"]}");
            UnityEngine.Debug.Log($"����: {question["answer"]}");
        }
    }
}
