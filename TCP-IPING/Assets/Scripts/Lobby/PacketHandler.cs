using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Lobby;
using UnityEngine.PlayerLoop;

public class PacketHandler
{
    LobbyServer server;
    
    public PacketHandler(LobbyServer server, IPacket packet, ClientHandler clientHandler)
    {
        this.server = server;
        HandlePacketType(packet, clientHandler);
    }
    private void HandlePacketType(IPacket packet, ClientHandler clientHandler)
    {
        switch (packet.type)
        {
            case PacketType.StartJoin:
                //서버에 접속할 때
                if(packet.id == 0)
                    DefineUser(clientHandler);
                break;
            case PacketType.EndJoin:
                //서버에서 퇴장
                clientHandler.Close();
                break;
            case PacketType.CreateLobby:
                //로비 생성
                string roomName = (string)packet.data;
                if(string.IsNullOrEmpty(roomName))
                    roomName = $"{UserList.Instance.ReadUser(packet.id).nickName}'s Lobby";

                SendLobbyData(server.CreateLobby(roomName, 4), clientHandler);
                break;
            case PacketType.JoinLobby:
                //packet.data로 LobbyID를 받기
                //이후 Lobby에 해당 유저 넣기
                server.JoinLobby((UID)packet.data, clientHandler);
                break;
            case PacketType.LeaveLobby:
                //packet.data로 LobbyID를 받기
                //이후 Lobby에 해당 유저 삭제
                server.LeaveLobby((UID)packet.data, clientHandler);
                break;
            case PacketType.Answer:
                ReceivedAnswer();
                break;
            case PacketType.Message:
                break;
            case PacketType.UpdateUserData:
                User user = (User)packet.data;
                UserList.Instance.UpdateUser(user);
                break;
            default:
                break;
        }
    }

    void DefineUser(ClientHandler clientHandler)
    {
        User newUser = UserList.Instance.CreateNewUser();
        IPacket packet = new IPacket(PacketType._DefineUser, newUser);
        clientHandler.SendPacket(packet);
    }
    void SendLobbyData(GameLobby lobby, ClientHandler clientHandler)
    {
        IPacket packet = new IPacket(PacketType._LobbyData, lobby);
        clientHandler.SendPacket(packet);
    }

    void ReceivedAnswer()
    {
        
    }

    void LeaveLobby(ClientHandler clientHandler)
    {
        //
    }
}
