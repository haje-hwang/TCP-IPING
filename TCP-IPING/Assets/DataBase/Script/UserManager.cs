using UnityEngine;
using TMPro; // TextMeshPro 네임스페이스 추가

public class UserManager : MonoBehaviour
{
    public UserList userList; // UserList Singleton 연결
    
    public TMP_InputField nameInputField; // 유저 이름을 입력받는 TMP_InputField
    public TMP_InputField RoomInputField; // 방 이름을 입력받는 TMP_InputField
    private string roomName;
    private string userName;
    
    // 버튼 클릭 시 호출될 메서드
    //방 생성하기 눌렀을 때 
    public void OnCreateUserButtonClick()
    {
        if (userList == null)
        {
            UnityEngine.Debug.LogError("UserList가 초기화되지 않았습니다!");
            return;
        }
        
        userName = nameInputField.text; // 입력 필드에서 유저 이름 가져오기
        if (string.IsNullOrWhiteSpace(userName))
        {
            return;
        }
        
        // UserList의 CreateNewUser 호출
        User newUser = userList.CreateNewUser(userName);

        // 결과 메시지 표시
        UnityEngine.Debug.Log($"새 유저 생성 완료: {newUser.nickName} (ID: {newUser.id})");
    }
    //방 입장하기 눌렀을 때
    public void OnJoinUserButtonClick()
    {
        if (userList == null)
        {
            UnityEngine.Debug.LogError("UserList가 초기화되지 않았습니다!");
            return;
        }

        userName = nameInputField.text; // 입력 필드에서 유저 이름 가져오기
        if (string.IsNullOrWhiteSpace(userName))
        {
            return;
        }
    }
    public void OnJoinButtonCllick()
    {
        roomName= RoomInputField.text;
        userList.ConnectToDatabase(roomName);
        User newUser = userList.CreateNewUser(userName);
        UnityEngine.Debug.Log($"새 유저 생성 완료: {newUser.nickName} (ID: {newUser.id})");
    }
    public void OnDeleteButtonClick()
    {
        if (userList == null)
        {
            UnityEngine.Debug.LogError("UserList가 초기화되지 않았습니다!");
            return;
        }
        userList.DeleteAllUsers();

        // 결과 메시지 표시
        //feedbackText.text = $"모든 유저정보가 제거 되었습니다.";
        UnityEngine.Debug.Log($"모든 유저정보가 제거 되었습니다.");
    }
}
