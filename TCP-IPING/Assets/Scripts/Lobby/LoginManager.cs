using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoginManager : MonoBehaviour
{
    public TMP_InputField nicknameInputField; // �г��� �Է� �ʵ�
    public Button RoomCreate;  // �� ����� ��ư
    public Button RoomJoin;    // �� ���� ��ư
    public TMP_InputField RoomJoinInput;
    public GameObject panel;

    private string playerNickname;

    void Start()
    {
        // ��ư Ŭ�� �� ȣ���� �Լ� ����
        RoomCreate.onClick.AddListener(OnCreateRoom);
        RoomJoin.onClick.AddListener(OnJoinRoom);
    }

    public void SaveNickname()
    {
        playerNickname = nicknameInputField.text;
        PlayerPrefs.SetString("PlayerNickname", playerNickname); // �г����� ����
        Debug.Log("�г��� �����: " + playerNickname);
    }

    public void OnCreateRoom()
    {
        SaveNickname();

        // �г����� ������� ������ WaitingRoom ������ �̵�
        if (!string.IsNullOrEmpty(playerNickname))
        {
            SceneManager.LoadScene("WaitingScene");
        }
        else
        {
            Debug.LogWarning("�г����� �Է����ּ���.");
        }
    }

    void OnJoinRoom()
    {
        panel.SetActive(true);  // Panel�� �ٽ� ���̰� ��
    }
}
