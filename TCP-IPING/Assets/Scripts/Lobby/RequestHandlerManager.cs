using UnityEngine;
using System;
using System.Net.Sockets;

public class RequestHandlerManager : MonoBehaviour
{
    // Singleton 인스턴스
    public static RequestHandlerManager Instance { get; private set; }

    private RequestHandler requestHandler;

    private void Awake()
    {
        // Singleton 패턴 구현
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // 이미 인스턴스가 있으면 현재 오브젝트를 삭제
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // 씬 전환 중에도 삭제되지 않도록 설정
    }

    private void Start()
    {
        // PlayerPrefs에서 닉네임 불러오기
        string userNickname = PlayerPrefs.GetString("PlayerNickname", "Player"); // 기본값: "Player"

        // 서버와 연결 및 RequestHandler 초기화
        User user = new User(Guid.NewGuid(), userNickname);
        TcpClient tcpClient = new TcpClient("27.113.62.74", 5000);
        requestHandler = new RequestHandler(user, tcpClient);

        Debug.Log($"Initialized with nickname: {userNickname}");
    }

    private string GenerateRoomName()
    {
        // 1000~9999 사이의 랜덤한 4자리 숫자 방 이름 생성
        int roomNumber = UnityEngine.Random.Range(1000, 10000);
        return roomNumber.ToString();
    }

    // Room 관련 기능 메서드
    public void CreateRoom()
    {
        if (requestHandler != null)
        {
            string roomName = GenerateRoomName();  // 4자리 숫자 방 이름 생성
            PlayerPrefs.SetString("RoomName", roomName);  // PlayerPrefs에 방 이름 저장
            requestHandler.CreateLobby(roomName);  // 서버에 방 생성 요청
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