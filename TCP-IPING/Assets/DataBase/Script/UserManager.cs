using UnityEngine;
using TMPro; // TextMeshPro ���ӽ����̽� �߰�

public class UserManager : MonoBehaviour
{
    public UserList userList; // UserList Singleton ����
    
    public TMP_InputField nameInputField; // ���� �̸��� �Է¹޴� TMP_InputField
    public TMP_InputField RoomInputField; // �� �̸��� �Է¹޴� TMP_InputField
    private string roomName;
    private string userName;
    
    // ��ư Ŭ�� �� ȣ��� �޼���
    //�� �����ϱ� ������ �� 
    public void OnCreateUserButtonClick()
    {
        if (userList == null)
        {
            UnityEngine.Debug.LogError("UserList�� �ʱ�ȭ���� �ʾҽ��ϴ�!");
            return;
        }
        
        userName = nameInputField.text; // �Է� �ʵ忡�� ���� �̸� ��������
        if (string.IsNullOrWhiteSpace(userName))
        {
            return;
        }
        
        // UserList�� CreateNewUser ȣ��
        User newUser = userList.CreateNewUser(userName);

        // ��� �޽��� ǥ��
        UnityEngine.Debug.Log($"�� ���� ���� �Ϸ�: {newUser.nickName} (ID: {newUser.id})");
    }
    //�� �����ϱ� ������ ��
    public void OnJoinUserButtonClick()
    {
        if (userList == null)
        {
            UnityEngine.Debug.LogError("UserList�� �ʱ�ȭ���� �ʾҽ��ϴ�!");
            return;
        }

        userName = nameInputField.text; // �Է� �ʵ忡�� ���� �̸� ��������
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
        UnityEngine.Debug.Log($"�� ���� ���� �Ϸ�: {newUser.nickName} (ID: {newUser.id})");
    }
    public void OnDeleteButtonClick()
    {
        if (userList == null)
        {
            UnityEngine.Debug.LogError("UserList�� �ʱ�ȭ���� �ʾҽ��ϴ�!");
            return;
        }
        userList.DeleteAllUsers();

        // ��� �޽��� ǥ��
        //feedbackText.text = $"��� ���������� ���� �Ǿ����ϴ�.";
        UnityEngine.Debug.Log($"��� ���������� ���� �Ǿ����ϴ�.");
    }
}
