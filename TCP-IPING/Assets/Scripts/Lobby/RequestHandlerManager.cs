using UnityEngine;
using System;
using System.Net.Sockets;

public class RequestHandlerManager : MonoBehaviour
{
    public static RequestHandlerManager Instance { get; private set; }
    private RequestHandler requestHandler;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        string userNickname = PlayerPrefs.GetString("PlayerNickname", "Player");
        User user = new User(Guid.NewGuid(), userNickname);
        TcpClient tcpClient = new TcpClient("127.0.0.1", 5000); //Ŭ�� �׽�Ʈ�� 
        //TcpClient tcpClient = new TcpClient("27.113.62.74", 5000); //�ܺ����ӿ�
        requestHandler = new RequestHandler(user, tcpClient);

        Debug.Log($"Initialized with nickname: {userNickname}");
    }

    private string GenerateRoomName()
    {
        int roomNumber = UnityEngine.Random.Range(1000, 10000); // 1000 ~ 9999 ������ ���� ����
        Debug.Log($"Generated Room Number: {roomNumber}");
        return roomNumber.ToString();
    }

    public void CreateRoom()
    {
        if (requestHandler != null)
        {
            // �� ��ȣ ����
            string roomName = GenerateRoomName();
            PlayerPrefs.SetString("RoomName", roomName); // PlayerPrefs�� ����

            // ������ �� ���� ��û
            requestHandler.CreateLobby(roomName);

            Debug.Log($"Room '{roomName}' created and sent to server.");

            // UI ������Ʈ
            RoomName roomNameScript = FindObjectOfType<RoomName>();
            if (roomNameScript != null)
            {
                roomNameScript.UpdateRoomNameUI();
                Debug.Log($"UI Updated with Room Name: {roomName}");
            }
            else
            {
                Debug.LogWarning("RoomName script not found in scene!");
            }
        }
        else
        {
            Debug.LogWarning("RequestHandler is not initialized!");
        }
    }

    public void JoinRoom(string roomName)
    {
        if (requestHandler != null)
        {
            requestHandler.JoinLobbyByName(roomName);
            Debug.Log($"Joined room with ID: {roomName}");
        }
        else
        {
            Debug.LogWarning("RequestHandler is not initialized!");
        }
    }
}
