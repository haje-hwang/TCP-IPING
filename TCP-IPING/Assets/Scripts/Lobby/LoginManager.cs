using UnityEditor.XR;
using UnityEngine;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public InputField nicknameInput; // �г��� �Է� �ʵ�
    public Button RoomCreate;  // �� ����� ��ư
    public Button RoomJoin;    // �� ���� ��ư

    private string playerNickname;

    void Start()
    {
        // ��ư Ŭ�� �� ȣ���� �Լ� ����
        RoomCreate.onClick.AddListener(OnCreateRoom);
        //RoomJoin.onClick.AddListener(OnJoinRoom);
    }

    public void SaveNickname()
    {
        playerNickname = nicknameInput.text;
        PlayerPrefs.SetString("PlayerNickname", playerNickname); // �г����� ����
        Debug.Log("�г��� �����: " + playerNickname);
    }

    public void OnCreateRoom()
    {
        SaveNickname();
    }


}


