using TMPro;
using UnityEngine;

public class NickName : MonoBehaviour
{
    public TMP_Text[] nicknameTexts; // 닉네임 텍스트 배열
    public GameObject[] foxImages;  // 여우 이미지 배열

    private bool[] slotOccupied; // 슬롯이 점유되었는지 여부를 저장하는 배열

    void Start()
    {
        // 슬롯 상태 초기화
        slotOccupied = new bool[nicknameTexts.Length];

        // 모든 슬롯 초기화 (닉네임 비활성화, 여우 색상 회색)
        for (int i = 0; i < nicknameTexts.Length; i++)
        {
            nicknameTexts[i].gameObject.SetActive(false); // 닉네임 UI 숨기기
            foxImages[i].GetComponent<UnityEngine.UI.Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f); // 회색 설정
        }

        // 플레이어 접속 테스트를 위한 샘플 호출 (이 부분은 서버 데이터와 연결되면 제거)
        string playerNickname = PlayerPrefs.GetString("PlayerNickname", "Guest");
        ActivateSlot(0, playerNickname); // 첫 번째 슬롯에 접속한 플레이어 표시
    }

    // 특정 슬롯을 활성화하고 닉네임과 이미지를 표시
    public void ActivateSlot(int playerIndex, string nickname)
    {
        Debug.Log($"ActivateSlot 호출됨 - 슬롯: {playerIndex}, 닉네임: {nickname}");

        if (playerIndex >= 0 && playerIndex < nicknameTexts.Length) // 유효한 인덱스인지 확인
        {
            if (!slotOccupied[playerIndex]) // 해당 슬롯이 비어있다면
            {
                slotOccupied[playerIndex] = true; // 슬롯 점유 상태로 설정
                nicknameTexts[playerIndex].gameObject.SetActive(true); // 닉네임 UI 활성화
                nicknameTexts[playerIndex].text = nickname; // 닉네임 설정
                foxImages[playerIndex].GetComponent<UnityEngine.UI.Image>().color = Color.white; // 여우 이미지를 하얀색으로 변경
                Debug.Log($"플레이어 {playerIndex + 1} 접속 성공: {nickname}");
            }
            else
            {
                Debug.LogWarning($"슬롯 {playerIndex + 1}은 이미 활성화됨");
            }
        }
        else
        {
            Debug.LogError($"잘못된 슬롯 인덱스: {playerIndex}");
        }
    }

    // 특정 슬롯을 비활성화 (테스트용으로 추가 가능)
    public void DeactivateSlot(int playerIndex)
    {
        Debug.Log($"DeactivateSlot 호출됨 - 슬롯: {playerIndex}");

        if (playerIndex >= 0 && playerIndex < nicknameTexts.Length)
        {
            if (slotOccupied[playerIndex]) // 슬롯이 점유된 상태라면
            {
                slotOccupied[playerIndex] = false; // 슬롯 점유 상태 해제
                nicknameTexts[playerIndex].gameObject.SetActive(false); // 닉네임 UI 숨기기
                foxImages[playerIndex].GetComponent<UnityEngine.UI.Image>().color = new Color(0.6f, 0.6f, 0.6f, 1f); // 여우 이미지를 회색으로 변경
                Debug.Log($"플레이어 {playerIndex + 1} 슬롯 비활성화");
            }
            else
            {
                Debug.LogWarning($"슬롯 {playerIndex + 1}은 이미 비활성화됨");
            }
        }
        else
        {
            Debug.LogError($"잘못된 슬롯 인덱스: {playerIndex}");
        }
    }
}
