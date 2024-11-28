using TMPro;
using UnityEngine;

public class RoomName : MonoBehaviour
{
    public TMP_Text roomNameText;

    private void Start()
    {
        UpdateRoomNameUI(); // 초기 UI 세팅
    }

    public void UpdateRoomNameUI()
    {
        // PlayerPrefs에서 방 번호 가져오기
        string roomName = PlayerPrefs.GetString("RoomName", "None");
        roomNameText.text = "방 번호: " + roomName; // 방 번호 UI 업데이트

        Debug.Log($"RoomName UI Updated: {roomName}");
    }
}
