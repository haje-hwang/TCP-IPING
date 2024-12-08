using UnityEngine;
using TMPro;

public class RankingManager : MonoBehaviour
{
    public TextMeshProUGUI rankingText; // ��ŷ�� ǥ���� TextMeshProUGUI

    void Start()
    {
        // PlayerPrefs���� �г��Ӱ� ���� �ҷ�����
        string playerNickname = PlayerPrefs.GetString("PlayerNickname", "Unknown");
        int playerScore = PlayerPrefs.GetInt("PlayerScore", 0);

        // ��ŷ �ؽ�Ʈ�� �г��Ӱ� ���� ǥ��
        rankingText.text = $"�г���: {playerNickname}     ����: {playerScore}";
    }
}