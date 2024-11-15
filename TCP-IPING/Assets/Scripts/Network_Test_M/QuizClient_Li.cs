using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class QuizClient_Li : MonoBehaviour
{
    // ���� �ּ�
    private string baseUrl = "http://localhost:5039/swagger/index.html";
    private string nickname = "";

    // �г��� ����
    public void SetNickname(string newNickname)
    {
        nickname = newNickname;
        StartCoroutine(SendPostRequest("/Room/SetNickname", "{\"nickname\":\"" + nickname + "\"}", (response) =>
        {
            Debug.Log("Nickname set: " + nickname);
        }));
    }

    // �� ����
    public void CreateRoom(string roomCode)
    {
        StartCoroutine(SendPostRequest("/Room/CreateRoom", "{\"roomCode\":\"" + roomCode + "\"}", (response) =>
        {
            Debug.Log("Room created: " + response);
        }));
    }

    // �� ����
    public void JoinRoom(string roomCode, string username)
    {
        StartCoroutine(SendPostRequest("/Room/JoinRoom", "{\"roomCode\":\"" + roomCode + "\", \"username\":\"" + username + "\"}", (response) =>
        {
            Debug.Log("Joined room: " + response);
        }));
    }

    // ���� ����
    public void SubmitAnswer(string roomCode, string username, string answer)
    {
        StartCoroutine(SendPostRequest("/Room/SubmitAnswer",
            "{\"roomCode\":\"" + roomCode + "\", \"username\":\"" + username + "\", \"answer\":\"" + answer + "\"}",
            (response) =>
            {
                Debug.Log("Answer submitted: " + response);
            }));
    }

    // ���� POST ��û ó�� �Լ�
    IEnumerator SendPostRequest(string endpoint, string json, System.Action<string> callback)
    {
        UnityWebRequest request = new UnityWebRequest(baseUrl + endpoint, "POST");
        byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(json);
        request.uploadHandler = new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");

        yield return request.SendWebRequest();

        if (request.result == UnityWebRequest.Result.Success)
        {
            callback?.Invoke(request.downloadHandler.text);
        }
        else
        {
            Debug.LogError("Request Error: " + request.error);
        }
    }
}
