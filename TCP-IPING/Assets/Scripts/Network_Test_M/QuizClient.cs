using System;
using System.IO;
using System.Net;
using System.Text;
using TMPro;
using UnityEngine;

public class QuizClient : MonoBehaviour
{
    private string serverUrl = "http://localhost:5000";
    private string nickname;
    private string roomCode;
    public TMP_InputField name;

    // �г��� ����
    public void SetNickname(string newNickname)
    {
        nickname = newNickname;
        var data = "{\"nickname\":\"" + name + "\"}";
        string response = SendPostRequest("/SetNickname", data);
        Debug.Log("SetNickname Response: " + response);
    }

    // �� ����
    public void CreateRoom(string newRoomCode)
    {
        roomCode = newRoomCode;
        var data = "{\"roomCode\":\"" + roomCode + "\"}";
        string response = SendPostRequest("/CreateRoom", data);
        Debug.Log("CreateRoom Response: " + response);
    }

    // ���� ���� ��û (���常 ����)
    public void StartGame()
    {
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("Room code not set!");
            return;
        }

        var data = "{\"roomCode\":\"" + roomCode + "\"}";
        string response = SendPostRequest("/StartGame", data);
        Debug.Log("StartGame Response: " + response);
    }

    // ���� ����
    public void SubmitAnswer(string answer)
    {
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("Room code not set!");
            return;
        }

        var data = "{\"roomCode\":\"" + roomCode + "\", \"answer\":\"" + answer + "\"}";
        string response = SendPostRequest("/SubmitAnswer", data);
        Debug.Log("SubmitAnswer Response: " + response);
    }

    // ���� ���� �� ��ŷ ǥ��
    public void EndGame()
    {
        if (string.IsNullOrEmpty(roomCode))
        {
            Debug.LogError("Room code not set!");
            return;
        }

        var data = "{\"roomCode\":\"" + roomCode + "\"}";
        string response = SendPostRequest("/EndGame", data);
        Debug.Log("EndGame Response: " + response);
    }

    // POST ��û ó�� �Լ�
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
            Debug.LogError("Request failed: " + ex.Message);
            return null;
        }
    }
}
