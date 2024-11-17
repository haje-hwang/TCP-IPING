using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가

public class UserManager : MonoBehaviour
{
    public UserList userList; // UserList Singleton 연결
    /*
    public TMP_InputField nameInputField; // 유저 이름을 입력받는 TMP_InputField
    public TMP_Text feedbackText; // 결과 메시지를 표시할 TMP_Text 컴포넌트
    */
    // 버튼 클릭 시 호출될 메서드
    public void OnCreateUserButtonClick()
    {
        if (userList == null)
        {
            UnityEngine.Debug.LogError("UserList가 초기화되지 않았습니다!");
            return;
        }
        /*
        string userName = nameInputField.text; // 입력 필드에서 유저 이름 가져오기
        if (string.IsNullOrWhiteSpace(userName))
        {
            feedbackText.text = "이름을 입력하세요!";
            return;
        }
        */
        // UserList의 CreateNewUser 호출
        User newUser = userList.CreateNewUser();

        // 결과 메시지 표시
        //feedbackText.text = $"새 유저 생성 완료: {newUser.name} (ID: {newUser.id})";
        UnityEngine.Debug.Log($"새 유저 생성 완료: {newUser.nickName} (ID: {newUser.id})");
    }
}
