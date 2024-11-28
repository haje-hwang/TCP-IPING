using TMPro;
using UnityEngine;

public class RoomName : MonoBehaviour
{
    public TMP_Text roomNameText;

    private void Start()
    {
        UpdateRoomNameUI(); // �ʱ� UI ����
    }

    public void UpdateRoomNameUI()
    {
        // PlayerPrefs���� �� ��ȣ ��������
        string roomName = PlayerPrefs.GetString("RoomName", "None");
        roomNameText.text = "�� ��ȣ: " + roomName; // �� ��ȣ UI ������Ʈ

        Debug.Log($"RoomName UI Updated: {roomName}");
    }
}
