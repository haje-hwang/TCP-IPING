using System;
using System.Collections;
using System.Collections.Generic;
using Lobby;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class _test_241117 : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField_CreateRoom;
    // [SerializeField] Button sendButton_CreateRoom;
    [SerializeField] TMP_InputField inputField_JoinRoom;
    // [SerializeField] Button sendButton_JoinRoom;
    [SerializeField] _testClient client;
    [SerializeField] DisplayLobbyData displayLobbyData;
    public event Action<LobbyData> OnDataReceived;
    public void test_CreateRoom()
    {
        Debug.Log(inputField_CreateRoom.text);
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
    private void LobbyUpdate(object sender, LobbyData data)
    {
        Debug.Log($"LobbyData: {data}");
        MainThreadDispatcher.ExecuteOnMainThread(() =>
        {
            Debug.Log($"LobbyData: {data}");
            displayLobbyData.DisplayData(data);
        });
    }

    public void test_JoinRoom()
    {
        Debug.Log(inputField_JoinRoom.text);
        Guid input = Guid.Parse(inputField_JoinRoom.text);
        client.GetHandler().JoinLobby(input);
        inputField_JoinRoom.text = "";
    }
}
