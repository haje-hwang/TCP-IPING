using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public InputField NicknameInput; // 닉네임 입력 필드
    public Button RoomCreate;  // 방 만들기 버튼
    public Button RoomJoin;    // 방 입장 버튼

    private string playerNickname;

    void Start()
    {
        // 버튼 클릭 시 호출할 함수 연결
        RoomCreate.onClick.AddListener(OnCreateRoom);
        //RoomJoin.onClick.AddListener(OnJoinRoom);
    }

    public void SaveNickname()
    {
        playerNickname = NicknameInput.text;
        PlayerPrefs.SetString("PlayerNickname", playerNickname); // 닉네임을 저장
        Debug.Log("닉네임 저장됨: " + playerNickname);
    }

    public void OnCreateRoom()
    {
        SaveNickname();
    }


}


