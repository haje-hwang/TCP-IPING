using UnityEngine;
using UnityEngine.UI; // Text를 사용할 경우
using TMPro; // TextMeshPro를 사용할 경우 주석 해제

public class Nickname : MonoBehaviour
{
    
    public TMP_Text nicknameText; // TextMeshPro를 사용할 경우 주석 해제

    void Start()
    {
        // PlayerPrefs에서 닉네임 가져오기
        string playerNickname = PlayerPrefs.GetString("PlayerNickname", "Guest");

        // 닉네임을 UI에 설정
        nicknameText.text = playerNickname;
    }
}