using UnityEngine;
using UnityEngine.UI; // Text�� ����� ���
using TMPro; 

public class Nickname : MonoBehaviour
{
    
    public TMP_Text nicknameText; 

    void Start()
    {
        // PlayerPrefs���� �г��� ��������
        string playerNickname = PlayerPrefs.GetString("PlayerNickname", "Guest");

        // �г����� UI�� ����
        nicknameText.text = playerNickname;
    }
}