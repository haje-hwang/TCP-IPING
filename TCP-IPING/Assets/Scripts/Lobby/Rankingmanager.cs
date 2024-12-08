using UnityEngine;
using TMPro;

public class RankingManager : MonoBehaviour
{
    public TextMeshProUGUI rankingText; // 랭킹을 표시할 TextMeshProUGUI

    void Start()
    {
        // PlayerPrefs에서 닉네임과 점수 불러오기
        string playerNickname = PlayerPrefs.GetString("PlayerNickname", "Unknown");
        int playerScore = PlayerPrefs.GetInt("PlayerScore", 0);

        // 랭킹 텍스트에 닉네임과 점수 표시
        rankingText.text = $"닉네임: {playerNickname}     점수: {playerScore}";
    }
}