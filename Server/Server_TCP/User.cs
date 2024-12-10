using System;

[Serializable]
public class User
{
    public Guid id {get; private set;}
    public string nickName;
    public bool isHost { get; private set; } // ȣ��Ʈ ���� ����
    public int score;
    public User(Guid uid, string name)
    {
        id = uid;
        nickName = name;
        isHost = false;
        score = 0;
    }
    public User Empty()
    {
        return new User(Guid.Empty, "Anonnymous");
    }
}
