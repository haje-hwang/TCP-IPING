using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    public TextMeshProUGUI rankingText1; // 1위 랭킹 텍스트
    public TextMeshProUGUI rankingText2; // 2위 랭킹 텍스트
    public TextMeshProUGUI rankingText3; // 3위 랭킹 텍스트
    public TextMeshProUGUI rankingText4; // 4위 랭킹 텍스트
    private ConnectUser connectUser;

    void Update()
    {
        connectUser = FindObjectOfType<ConnectUser>();
        if (connectUser == null)
        {
            Debug.LogError("ConnectUser를 찾을 수 없습니다.");
            return;
        }

        string roomName = PlayerPrefs.GetString("RoomName", "None");
        // 점수 정렬 및 상위 4명의 랭킹 가져오기
        List<(string nickName, int score)> topRankings = connectUser.GetTopRankings(roomName, 4);

        // 랭킹 텍스트에 표시
        UpdateRankingTexts(topRankings);
    }

    private void UpdateRankingTexts(List<(string nickName, int score)> rankings)
    {
        // 상위 4명의 랭킹 정보를 텍스트로 업데이트
        rankingText1.text = rankings.Count > 0 ? $"1위: 닉네임: {rankings[0].nickName}, 점수: {rankings[0].score}" : "1위: 없음";
        rankingText2.text = rankings.Count > 1 ? $"2위: 닉네임: {rankings[1].nickName}, 점수: {rankings[1].score}" : "2위: 없음";
        rankingText3.text = rankings.Count > 2 ? $"3위: 닉네임: {rankings[2].nickName}, 점수: {rankings[2].score}" : "3위: 없음";
        rankingText4.text = rankings.Count > 3 ? $"4위: 닉네임: {rankings[3].nickName}, 점수: {rankings[3].score}" : "4위: 없음";
    }
    public void OnHomeButton()
    {
        connectUser.DeleteAllUsers();
    }
}
