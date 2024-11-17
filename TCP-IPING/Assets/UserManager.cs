using UnityEngine;
using TMPro; // TextMeshPro ���ӽ����̽� �߰�

public class UserManager : MonoBehaviour
{
    public UserList userList; // UserList Singleton ����
    
    public TMP_InputField nameInputField; // ���� �̸��� �Է¹޴� TMP_InputField
    public TMP_Text feedbackText; // ��� �޽����� ǥ���� TMP_Text ������Ʈ
    
    // ��ư Ŭ�� �� ȣ��� �޼���
    public void OnCreateUserButtonClick()
    {
        if (userList == null)
        {
            UnityEngine.Debug.LogError("UserList�� �ʱ�ȭ���� �ʾҽ��ϴ�!");
            return;
        }
        
        string userName = nameInputField.text; // �Է� �ʵ忡�� ���� �̸� ��������
        if (string.IsNullOrWhiteSpace(userName))
        {
            feedbackText.text = "�̸��� �Է��ϼ���!";
            return;
        }
        
        // UserList�� CreateNewUser ȣ��
        User newUser = userList.CreateNewUser(userName);

        // ��� �޽��� ǥ��
        feedbackText.text = $"�� ���� ���� �Ϸ�: {newUser.nickName} (ID: {newUser.id})";
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
        feedbackText.text = $"��� ���������� ���� �Ǿ����ϴ�.";
        UnityEngine.Debug.Log($"��� ���������� ���� �Ǿ����ϴ�.");
    }
}
