using System;
using System.IO;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;

public class QuizClient : MonoBehaviour
{
    private string serverUrl = "http://TcpIping.iptime.org:5000/";/*"http://27.113.62.74:54321/";*//*"http://192.168.0.3:54321/";*/
    private string nickname;
    private string roomCode;
    public TMP_InputField name;

    public void SetNickname()
    {
        nickname = name.text;
        var data = "{\"nickname\":\"" + nickname + "\"}";
        string response = SendPostRequest("/SetNickname", data);
        Debug.Log($"[Client] SetNickname Response: {response}");
    }

    public void CreateRoom(string newRoomCode)
    {
        roomCode = newRoomCode;
        var data = "{\"roomCode\":\"" + roomCode + "\"}";
        string response = SendPostRequest("/CreateRoom", data);
        Debug.Log($"[Client] CreateRoom Response: {response}");
    }

    public void StartGame()
    {
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("[Client] Room code not set!");
            return;
        }

        var data = "{\"roomCode\":\"" + roomCode + "\"}";
        string response = SendPostRequest("/StartGame", data);
        Debug.Log($"[Client] StartGame Response: {response}");
    }

    public void SubmitAnswer(string answer)
    {
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("[Client] Room code not set!");
            return;
        }

        var data = "{\"roomCode\":\"" + roomCode + "\", \"answer\":\"" + answer + "\"}";
        string response = SendPostRequest("/SubmitAnswer", data);
        Debug.Log($"[Client] SubmitAnswer Response: {response}");
    }

    public void EndGame()
    {
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("[Client] Room code not set!");
            return;
        }

        var data = "{\"roomCode\":\"" + roomCode + "\"}";
        string response = SendPostRequest("/EndGame", data);
        Debug.Log($"[Client] EndGame Response: {response}");
    }

    private string SendPostRequest(string endpoint, string json)
    {
        try
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverUrl + endpoint);
            request.Method = "POST";
            request.ContentType = "application/json";

            byte[] byteArray = Encoding.UTF8.GetBytes(json);
            request.ContentLength = byteArray.Length;

            using (Stream dataStream = request.GetRequestStream())
            {
                dataStream.Write(byteArray, 0, byteArray.Length);
            }

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            using (StreamReader reader = new StreamReader(response.GetResponseStream()))
            {
                return reader.ReadToEnd();
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Client] Request failed: {ex.Message}");
            return null;
        }
    }
}
