using System;
using TMPro;
using UnityEngine;
using Server_TCP.Lobby;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;

public class _test_241117 : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField_CreateRoom;
    // [SerializeField] Button sendButton_CreateRoom;
    [SerializeField] TMP_InputField inputField_JoinRoom;
    // [SerializeField] Button sendButton_JoinRoom;
    [SerializeField] _testClient client;
    [SerializeField] DisplayLobbyData displayLobbyData;
    public void test_CreateRoom()
    {
        Debug.Log($"Create Room Named: {inputField_CreateRoom.text}");
        if(!string.IsNullOrWhiteSpace(inputField_CreateRoom.text))
        {
            client.GetHandler().OnLobbyUpdate -= LobbyUpdate;
            client.GetHandler().OnLobbyUpdate += LobbyUpdate;
            client.GetHandler().CreateLobby(inputField_CreateRoom.text);
            inputField_CreateRoom.text = "";
        }
        else
        {
            inputField_CreateRoom.text = "Try Again";
        }
    }
    private void LobbyUpdate(LobbyData data, JArray users)
    {
        MainThreadDispatcher.ExecuteOnMainThread(() =>
        {
            displayLobbyData.DisplayData(data, users);
        });
    }

    public void test_JoinRoom()
    {
        string input = inputField_JoinRoom.text;
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Input string cannot be null or empty.");

        // 공백 제거
        string sanitizedInput = input.Replace(" ", "");
        if(sanitizedInput.Length == 36)
        {
            try
            {
                Guid input_roomID = Guid.Parse(inputField_JoinRoom.text);
                client.GetHandler().JoinLobby(input_roomID);
            }
            catch
            {
                throw;
            }
        }
        else
        {
            client.GetHandler().JoinLobbyByName(input);
        }
        
        inputField_JoinRoom.text = "";
    }
}
