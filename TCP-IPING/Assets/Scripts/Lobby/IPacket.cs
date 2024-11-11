using System;

[Serializable]
public enum PacketType
{
    Message,   // 메시지 패킷
    //클라->서버 패킷
    StartJoin, //서버 참가요청 패킷: uint로 UserID보내기, 미정이면 0전송
    JoinLobby, // 로비 참가 패킷
    LeaveLobby, // 로비 퇴장 패킷
    Answer, //문제 답변
    UpdateUserData, //유저 정보 업데이트
    
    //서버->클라 패킷
    _DefineUser,    //userID정의해서 User클래스로 전송
    _Quiz,   //문제 제출 패킷
    Timer,  //남은 타이머 전송 패킷

    // 추가적인 패킷 유형을 여기에 정의
}

[Serializable]
public class IPacket    //상속해서 "data"부분을 override하고 무슨 데이터인지 PacketType으로 표시해서 전송하기
{
    public PacketType type { get; set; } // 패킷의 유형
    public object data { get; set; }     // 전송할 데이터
    public IPacket(PacketType _type, object _data)
    {
        type = _type;
        data = _data;
    }
}
