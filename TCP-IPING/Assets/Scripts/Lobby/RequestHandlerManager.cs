using UnityEngine;
using System;
using System.Net.Sockets;

public class RequestHandlerManager : MonoBehaviour
{
    // Singleton �ν��Ͻ�
    public static RequestHandlerManager Instance { get; private set; }

    private RequestHandler requestHandler;

    private void Awake()
    {
        // Singleton ���� ����
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // �̹� �ν��Ͻ��� ������ ���� ������Ʈ�� ����
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // �� ��ȯ �߿��� �������� �ʵ��� ����
    }

    private void Start()
    {
        // PlayerPrefs���� �г��� �ҷ�����
        string userNickname = PlayerPrefs.GetString("PlayerNickname", "Player"); // �⺻��: "Player"

        // ������ ���� �� RequestHandler �ʱ�ȭ
        User user = new User(Guid.NewGuid(), userNickname);
        TcpClient tcpClient = new TcpClient("27.113.62.74", 5000);
        requestHandler = new RequestHandler(user, tcpClient);

        Debug.Log($"Initialized with nickname: {userNickname}");
    }

    private string GenerateRoomName()
    {
        // 1000~9999 ������ ������ 4�ڸ� ���� �� �̸� ����
        int roomNumber = UnityEngine.Random.Range(1000, 10000);
        return roomNumber.ToString();
    }

    // Room ���� ��� �޼���
    public void CreateRoom()
    {
        if (requestHandler != null)
        {
            string roomName = GenerateRoomName();  // 4�ڸ� ���� �� �̸� ����
            PlayerPrefs.SetString("RoomName", roomName);  // PlayerPrefs�� �� �̸� ����
            requestHandler.CreateLobby(roomName);  // ������ �� ���� ��û
            Debug.Log($"Room '{roomName}' created.");
        }
        else
        {
            Debug.LogWarning("RequestHandler is not initialized!");
        }
    }

    public void JoinRoom(Guid roomID)
    {
        if (requestHandler != null)
        {
            requestHandler.JoinLobby(roomID); 
            Debug.Log($"Joined room with ID: {roomID}");
        }
        else
        {
            Debug.LogWarning("RequestHandler is not initialized!");
        }
    }
}