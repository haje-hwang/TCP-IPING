using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField nicknameInputField;
    public Button RoomCreate;
    public Button RoomJoin;
    public TMP_InputField RoomJoinInput;
    public GameObject panel;
    public Button RoomJoinX;
    public Button RoomJoinConfirm;

    private string playerNickname;

    void Start()
    {
        RoomCreate.onClick.AddListener(OnCreateRoom);
        RoomJoin.onClick.AddListener(OnJoinRoom);
        RoomJoinConfirm.onClick.AddListener(OnConfirmJoinRoom);
        RoomJoinX.onClick.AddListener(OnRoomJoinX);
    }

    public void SaveNickname()
    {
        playerNickname = nicknameInputField.text;
        PlayerPrefs.SetString("PlayerNickname", playerNickname);
        Debug.Log("닉네임 저장됨: " + playerNickname);
    }

    public void OnCreateRoom()
    {
        SaveNickname();

        if (!string.IsNullOrEmpty(playerNickname))
        {
            RequestHandlerManager.Instance.CreateRoom(); // 방 생성 요청
            SceneManager.LoadScene("WaitingScene"); // 대기실로 이동
        }
        else
        {
            Debug.LogWarning("닉네임을 입력해주세요.");
        }
    }

    void OnJoinRoom()
    {
        SaveNickname();
        panel.SetActive(true); // 방 번호 입력 UI 표시
    }

    public void OnRoomJoinX()
    {
        panel.SetActive(false); // 방 번호 입력 UI 숨김
    }

    public void OnConfirmJoinRoom()
    {
        string roomNumber = RoomJoinInput.text;

        if (!string.IsNullOrEmpty(roomNumber))
        {
            try
            {
                string formattedRoomNumber = roomNumber.PadLeft(32, '0'); // 4자리 숫자를 32자리로 변환
                Guid roomID = new Guid(formattedRoomNumber); // 방 번호를 Guid로 변환
                RequestHandlerManager.Instance.JoinRoom(roomID); // 방 입장 요청

                SceneManager.LoadScene("WaitingScene"); // 대기실로 이동
            }
            catch (FormatException)
            {
                Debug.LogError("잘못된 방 번호 형식입니다. 유효한 숫자를 입력해주세요.");
            }
        }
        else
        {
            Debug.LogWarning("방 번호를 입력해주세요.");
        }
    }
}
