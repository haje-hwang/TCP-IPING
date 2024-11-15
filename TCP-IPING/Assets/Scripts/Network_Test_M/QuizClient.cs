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

    // 닉네임 설정
    public void SetNickname(string newNickname)
    {
        nickname = newNickname;
        var data = "{\"nickname\":\"" + name + "\"}";
        string response = SendPostRequest("/SetNickname", data);
        Debug.Log("SetNickname Response: " + response);
    }

    // 방 생성
    public void CreateRoom(string newRoomCode)
    {
        roomCode = newRoomCode;
        var data = "{\"roomCode\":\"" + roomCode + "\"}";
        string response = SendPostRequest("/CreateRoom", data);
        Debug.Log("CreateRoom Response: " + response);
    }

    // 게임 시작 요청 (방장만 가능)
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

    // 정답 제출
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

    // 게임 종료 및 랭킹 표시
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

    // POST 요청 처리 함수
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
