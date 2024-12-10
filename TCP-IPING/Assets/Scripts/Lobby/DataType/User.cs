using System;

//2024-12-10. 
//유니티에서 User클래스 변경 시 서버의 User클래스와 달라져서
//json 직렬화-역직렬화가 안되니까 매개변수 추가, 삭제, 변경을 자제할 것
//추가가 필요하다면 id를 키값으로하는 테이블을 새로 만들어 데이터베이스 정규화하기
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