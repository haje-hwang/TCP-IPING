using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class RankingManager : MonoBehaviour
{
    public TextMeshProUGUI rankingText1; // 1�� ��ŷ �ؽ�Ʈ
    public TextMeshProUGUI rankingText2; // 2�� ��ŷ �ؽ�Ʈ
    public TextMeshProUGUI rankingText3; // 3�� ��ŷ �ؽ�Ʈ
    public TextMeshProUGUI rankingText4; // 4�� ��ŷ �ؽ�Ʈ
    private ConnectUser connectUser;

    void Update()
    {
        connectUser = FindObjectOfType<ConnectUser>();
        if (connectUser == null)
        {
            Debug.LogError("ConnectUser�� ã�� �� �����ϴ�.");
            return;
        }

        string roomName = PlayerPrefs.GetString("RoomName", "None");
        // ���� ���� �� ���� 4���� ��ŷ ��������
        List<(string nickName, int score)> topRankings = connectUser.GetTopRankings(roomName, 4);

        // ��ŷ �ؽ�Ʈ�� ǥ��
        UpdateRankingTexts(topRankings);
    }

    private void UpdateRankingTexts(List<(string nickName, int score)> rankings)
    {
        // ���� 4���� ��ŷ ������ �ؽ�Ʈ�� ������Ʈ
        rankingText1.text = rankings.Count > 0 ? $"1��: �г���: {rankings[0].nickName}, ����: {rankings[0].score}" : "1��: ����";
        rankingText2.text = rankings.Count > 1 ? $"2��: �г���: {rankings[1].nickName}, ����: {rankings[1].score}" : "2��: ����";
        rankingText3.text = rankings.Count > 2 ? $"3��: �г���: {rankings[2].nickName}, ����: {rankings[2].score}" : "3��: ����";
        rankingText4.text = rankings.Count > 3 ? $"4��: �г���: {rankings[3].nickName}, ����: {rankings[3].score}" : "4��: ����";
    }
    public void OnHomeButton()
    {
        connectUser.DeleteAllUsers();
    }
}
