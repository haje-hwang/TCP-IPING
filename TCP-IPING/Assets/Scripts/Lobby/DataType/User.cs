using System;

[Serializable]
public class User
{
    public Guid id { get; private set; }
    public string nickName;
    public bool isHost { get; private set; } // ȣ��Ʈ ���� ����
    public int score;

    public User(Guid uid, string name, bool isHost = false)
    {
        id = uid;
        nickName = name;
        this.isHost = isHost;
        score = 0;
    }

    public User Empty()
    {
        return new User(Guid.Empty, "Anonymous", false);
    }

    // ȣ��Ʈ ���� �ο� �޼���
    public void MakeHost()
    {
        isHost = true;
    }

    // ȣ��Ʈ ���� ���� �޼���
    public void RemoveHost()
    {
        isHost = false;
    }

    public override string ToString()
    {
        return $"User: {nickName} (ID: {id}, Host: {isHost})";
    }
}