using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Server_TCP;
using Server_TCP.Quiz;

namespace Server_TCP.Lobby
{
    /// <summary>
    /// 게임 전체 서버, 게임 내 모든 로비들을 관리한다고 LobbyServer라 지음
    /// 홀펀칭 추가해서 host클라가 각 게임 로비를 p2p로 하는게 아니면
    /// 게임 관련 통신도 담당함
    /// </summary>
    public class LobbyServer    
    {
        public bool isRunning ;
        

        private int _PORT;
        private TcpListener listener;
        private List<ClientHandler> clients = new List<ClientHandler>();
        private UserList userList = new UserList();
        private ConcurrentDictionary<Guid, GameLobby> lobbyMap = new ConcurrentDictionary<Guid, GameLobby>();
        private ConcurrentDictionary<string, Guid> roomIdMap = new ConcurrentDictionary<string, Guid>();
        private PacketEventDispatcher eventDispatcher;

        public LobbyServer(int PORT)
        {
            listener = new TcpListener(IPAddress.Any, PORT);
            eventDispatcher = new PacketEventDispatcher(this);
            listener.Start();

            _PORT = PORT;
            isRunning = true;
        }
        ~LobbyServer()
        {
            CloseServer();
        }
        public void CloseServer()
        {
            isRunning = false;
            foreach (var gameLobby in lobbyMap.Values)
            {
                gameLobby.CloseLobby();
            }
            foreach (var client in clients)
            {
                client.Disconnect();
            }
            listener.Stop();
        }
        public async Task Start()
        {
            try
            {
                Debug.Log($"Listening on port {_PORT}...");
                while (isRunning)
                {
                    // 비동기 방식으로 클라이언트 연결을 기다림
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Debug.Log("Client connected!");

                    ClientHandler handler = new ClientHandler(client, eventDispatcher);
                    ServerEnter(handler);
                    eventDispatcher.OnUserExited += ServerExit;

                    _ = handler.Start(); // 클라이언트 처리 시작
                }
            }
            catch (System.Exception)
            {
                throw;
            }
            Debug.Log($"Stop Listning...");
        }

        public void BroadcastPacket(IPacket packet)
        {
            foreach (var client in clients)
            {
                client.SendPacketAsync(packet);
            }
        }
        //eventListen
        public void ServerEnter(ClientHandler client)
        {
            clients.Add(client);
        }
        public void ServerExit(ClientHandler client)
        {
            clients.Remove(client);
        }
        // 로비 관리 메서드...
        public GameLobby CreateLobby(string name, int maxPlayers, Guid host)
        {
            Guid lobby_uid = Guid.NewGuid();
            LobbyData data = new LobbyData(lobby_uid, name, maxPlayers, host);
            GameLobby lobby = new GameLobby(data, eventDispatcher);
            lobbyMap.TryAdd(lobby_uid, lobby);
            roomIdMap.TryAdd(name, lobby_uid);

            lobby.OnLobbyDestroyed -= DestroyLobby;
            lobby.OnLobbyDestroyed += DestroyLobby;
            return lobby;
        }
        public void DestroyLobby(Guid lobbyId)
        {
            roomIdMap.TryRemove(lobbyMap[lobbyId].data.name, out Guid g);
            lobbyMap.TryRemove(lobbyId, out GameLobby? l);
        }
        public GameLobby FindLobby(Guid uid)
        {
            //return lobbies.FirstOrDefault(lobby => lobby.data.uid == uid);
            return lobbyMap[uid];
        }
        public bool JoinLobby(Guid lobbyCode, ClientHandler client, User user)
        {
            return FindLobby(lobbyCode)?.AddPlayer(client, user) ?? false;
        }
        public bool JoinLobby(string lobbyName,  ClientHandler client, User user)
        {
            Guid id = roomIdMap[lobbyName];
            return JoinLobby(id, client, user);
        }
        public bool LeaveLobby(Guid lobbyCode, ClientHandler client, User user)
        {
            return FindLobby(lobbyCode)?.RemovePlayer(client, user) ?? false;
        }
        public List<LobbyData> GetALLLobbyList()
        {
            List<LobbyData> result = new List<LobbyData>();
            result.Capacity = lobbyMap.Count;
            foreach (var gameLobby in lobbyMap.Values)
            {
                result.Add(gameLobby.data);
            }
            return result;
        }

        //
        public User NewUser()
        {
            return userList.CreateNewUser();
        }
        public void UpdateUser(User user)
        {
            userList.UpdateUser(user);
        }
    }
}