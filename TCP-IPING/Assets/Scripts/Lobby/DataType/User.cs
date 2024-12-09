using System;

[Serializable]
public class User
{
    public Guid id { get; private set; }
    public string nickName;
    public bool isHost { get; private set; } // 호스트 여부 관리
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

    // 호스트 권한 부여 메서드
    public void MakeHost()
    {
        isHost = true;
    }

    // 호스트 권한 해제 메서드
    public void RemoveHost()
    {
        isHost = false;
    }

    public override string ToString()
    {
        return $"User: {nickName} (ID: {id}, Host: {isHost})";
    }
}