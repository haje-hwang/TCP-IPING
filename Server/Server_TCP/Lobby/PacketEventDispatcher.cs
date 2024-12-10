using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Server_TCP.Lobby
{
    public class PacketEventDispatcher
    {
        private LobbyServer _server;
        //Delegate
        public delegate void UserJoin(ClientHandler joinedClient);
        public delegate void UserExit(ClientHandler exitedClient);
        public delegate void LobbyCreate(Guid lobbyId, User host);
        public delegate void LobbyJoin(Guid lobbyId,  User joinedUser);
        public delegate void LobbyExit(Guid lobbyId,  User exitedUser);
        public delegate void LobbyDestroy(Guid lobbyId);
        //Events
        public event UserJoin OnUserJoined = delegate { };
        public event UserExit OnUserExited = delegate { };
        private PacketEventDispatcher() { }
        public PacketEventDispatcher(LobbyServer server)
        {
            _server = server;
        }

        public void ProcessPacket(IPacket packet, ClientHandler client)
        {
            try
            {
                switch (packet.type)
                {
                    case PacketType.__FirstJoin:
                        DefineUser(client);
                        break;
                    case PacketType.__LobbyData:
                        SendLobbyData(client);
                        break;
                    case PacketType.__LobbyList:
                        SendLobbyList(client);
                        break;
                    case PacketType._StartJoin:
                        //서버에 접속할 때
                        if (packet.id == Guid.Empty)
                            DefineUser(client);
                        else
                        {
                            client.SetUser(Object2User(packet.data));
                        }
                        //ClientHandler가 생성될 때 대부분 초기화 하기 때문에 ID체크만
                        break;
                    case PacketType._EndJoin:
                        //서버에서 퇴장
                        OnUserExited?.Invoke(client);
                        client.Disconnect();
                        break;
                    case PacketType._CreateLobby:
                        //로비 생성
                        string roomName = (string)packet.data;
                        Debug.Log($"In _CreateLobby Packet, Received Lobby name is :{roomName}");
                        if (string.IsNullOrEmpty(roomName))
                            roomName = $"{client.GetUser().nickName}'s Lobby";

                        CreateLobby(client, roomName);  
                        break;
                    case PacketType._JoinLobby:
                        //packet.data로 LobbyID를 받기
                        //이후 Lobby에 해당 유저 넣기
                        Guid roomID = Guid.Parse((string)packet.data);
                        _server.JoinLobby(roomID, client, client.GetUser());
                        break;
                    case PacketType._JoinLobbyByName:
                        //packet.data로 LobbyName를 받기
                        //이후 Lobby에 해당 유저 넣기
                        _server.JoinLobby((string)packet.data, client, client.GetUser());
                        break;
                    case PacketType._LeaveLobby:
                        //packet.data로 LobbyID를 받기
                        //이후 Lobby에 해당 유저 삭제
                        _server.LeaveLobby((Guid)packet.data, client, client.GetUser());
                        break;
                    case PacketType._Answer:
                        ReceivedAnswer();
                        break;
                    case PacketType._SendLobbyMessege:
                        break;
                    case PacketType._UpdateUserData:
                        UpdateUserData(Object2User(packet.data));
                        break;
                    default:
                        break;
                }
            }
            catch(Exception e)
            {
                Debug.LogWarning($"While Processing Packet ({client.GetUser()?.nickName}, {client.GetUser()?.id}): {e.Message}");
                throw;
            }
        }
        private User? Object2User (object obj)
        {
            JObject jObject = (Newtonsoft.Json.Linq.JObject) obj;
            User? user = jObject.ToObject<User>();
            return user;
        }
        //
        public void ServerExit(ClientHandler client)
        {
            OnUserExited?.Invoke(client);
        }
        public void CreateLobby(ClientHandler client, string roomName)
        {
            User user = client.GetUser();
            GameLobby gameLobby = _server.CreateLobby(roomName, 4, user.id);
            gameLobby.AddPlayer(client, user);
        }

        public void UpdateUserData(User user)
        {
            _server.UpdateUser(user);
        }
        #region IResponse구현
        //양방향
        public void DefineUser(ClientHandler client)
        {
            User newUser = _server.NewUser();
            client.SetUser(newUser);
            IPacket packet = new(PacketType.__FirstJoin, newUser.id, Guid.Empty);
            client.SendPacketAsync(packet);
        }
        public void SendLobbyList(ClientHandler client)
        {
            List<LobbyData> lobbyList = _server.GetALLLobbyList();
            IPacket packet = new(PacketType.__FirstJoin, lobbyList, Guid.Empty);
            client.SendPacketAsync(packet);
        }
        public void SendLobbyData(ClientHandler client)
        {
            LobbyData lobbyData = _server.FindLobby(client.joindLobbyId).data;
            IPacket packet = new(PacketType.__LobbyData, lobbyData, Guid.Empty);
            client.SendPacketAsync(packet);
        }
        //단방향 broadcasting
        //Lobby
        public void Booted(ClientHandler client)
        {
            LobbyData lobbyData = _server.FindLobby(client.joindLobbyId).data;
            IPacket packet = new(PacketType.Booted, lobbyData, Guid.Empty);
            client.SendPacketAsync(packet);
        }
        public void LobbyMessege(string whoSpeaks, string message)
        {

        }
        //In Game
        public void GameStarted()
        {

        }
        public void ReceivedAnswer()
        {

        }
        public void LastTimer()
        {

        }
        #endregion
    }
}
