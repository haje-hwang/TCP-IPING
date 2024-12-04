using TMPro;
using UnityEngine;

public class NickName : MonoBehaviour
{
    public TMP_Text[] nicknameTexts; // �г��� �ؽ�Ʈ �迭
    public GameObject[] foxImages;  // ���� �̹��� �迭

    private bool[] slotOccupied; // ������ �����Ǿ����� ���θ� �����ϴ� �迭

    void Start()
    {
        // ���� ���� �ʱ�ȭ
        slotOccupied = new bool[nicknameTexts.Length];

        // ��� ���� �ʱ�ȭ (�г��� ��Ȱ��ȭ, ���� ���� ȸ��)
        for (int i = 0; i < nicknameTexts.Length; i++)
        {
            nicknameTexts[i].gameObject.SetActive(false); // �г��� UI �����
            foxImages[i].GetComponent<UnityEngine.UI.Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f); // ȸ�� ����
        }

        // �÷��̾� ���� �׽�Ʈ�� ���� ���� ȣ�� (�� �κ��� ���� �����Ϳ� ����Ǹ� ����)
        string playerNickname = PlayerPrefs.GetString("PlayerNickname", "Guest");
        ActivateSlot(0, playerNickname); // ù ��° ���Կ� ������ �÷��̾� ǥ��
    }

    // Ư�� ������ Ȱ��ȭ�ϰ� �г��Ӱ� �̹����� ǥ��
    public void ActivateSlot(int playerIndex, string nickname)
    {
        Debug.Log($"ActivateSlot ȣ��� - ����: {playerIndex}, �г���: {nickname}");

        if (playerIndex >= 0 && playerIndex < nicknameTexts.Length) // ��ȿ�� �ε������� Ȯ��
        {
            if (!slotOccupied[playerIndex]) // �ش� ������ ����ִٸ�
            {
                slotOccupied[playerIndex] = true; // ���� ���� ���·� ����
                nicknameTexts[playerIndex].gameObject.SetActive(true); // �г��� UI Ȱ��ȭ
                nicknameTexts[playerIndex].text = nickname; // �г��� ����
                foxImages[playerIndex].GetComponent<UnityEngine.UI.Image>().color = Color.white; // ���� �̹����� �Ͼ������ ����
                Debug.Log($"�÷��̾� {playerIndex + 1} ���� ����: {nickname}");
            }
            else
            {
                Debug.LogWarning($"���� {playerIndex + 1}�� �̹� Ȱ��ȭ��");
            }
        }
        else
        {
            Debug.LogError($"�߸��� ���� �ε���: {playerIndex}");
        }
    }

    // Ư�� ������ ��Ȱ��ȭ (�׽�Ʈ������ �߰� ����)
    public void DeactivateSlot(int playerIndex)
    {
        Debug.Log($"DeactivateSlot ȣ��� - ����: {playerIndex}");

        if (playerIndex >= 0 && playerIndex < nicknameTexts.Length)
        {
            if (slotOccupied[playerIndex]) // ������ ������ ���¶��
            {
                slotOccupied[playerIndex] = false; // ���� ���� ���� ����
                nicknameTexts[playerIndex].gameObject.SetActive(false); // �г��� UI �����
                foxImages[playerIndex].GetComponent<UnityEngine.UI.Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f); // ���� �̹����� ȸ������ ����
                Debug.Log($"�÷��̾� {playerIndex + 1} ���� ��Ȱ��ȭ");
            }
            else
            {
                Debug.LogWarning($"���� {playerIndex + 1}�� �̹� ��Ȱ��ȭ��");
            }
        }
        else
        {
            Debug.LogError($"�߸��� ���� �ε���: {playerIndex}");
        }
    }
}
