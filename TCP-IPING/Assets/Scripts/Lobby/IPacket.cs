using System;

[Serializable]
public enum PacketType
{
    Message,   // 메시지 패킷
    //클라->서버 패킷
    StartJoin, //서버 참가요청 패킷: UID로 UserID보내기, 미정이면 0전송
    EndJoin,    //서버 퇴장 패킷
    LobbyList,  //존재하는 로비 리스트 불러오기
    CreateLobby,    //로비 생성 패킷, data로 로비 이름 정의해서 보내기
    JoinLobby, // 로비 참가 패킷, data로 참가할 로비 이름 정의해서 보내기
    LobbyData,  //로비 데이터 불러오기
    LeaveLobby, // 로비 퇴장 패킷
    Answer, //문제 답변
    UpdateUserData, //유저 정보 업데이트
    GameData,   //게임 정보 (각 유저 점수 순위)

    //서버->클라 패킷
    _PARDON,    //네가 보냈던 패킷이 이상해서 수행하지 못했다. 다시 보내달라. (data로 Packet 반송)
    _DefineUser,    //userID정의해서 User클래스로 전송
    _Quiz,   //문제 제출 패킷
    _Timer,  //남은 타이머 전송 패킷
    _LobbyList, //로비 리스트 데이터 전송, (로비ID, 로비이름, 참여중인 인원)
    _LobbyData, //로비 데이터 전송
    _GameData,  //게임 정보
    _Booted,    //로비 강제퇴장
    

    //통신에 필요한 케이스가 너무 많은데요;;;
}

[Serializable]
public class IPacket    //상속해서 "data"부분을 override하고 무슨 데이터인지 PacketType으로 표시해서 전송하기
{
    public PacketType type { get; set; } // 패킷의 유형
    public UID id { get; set; } //보내는 유저의(혹은 lobby)의 id. 0은 미정일때. 1은 서버에서 보냄
    public object data { get; set; }     // 전송할 데이터
    public IPacket(PacketType type, object data, UID id)
    {
        this.type = type;
        this.data = data;
        this.id = id;
    }
}
