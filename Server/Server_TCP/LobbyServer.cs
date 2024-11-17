using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;
using System.Collections.Concurrent;
using Server_TCP;

namespace Lobby
{
    public class LobbyServer
    {
        public bool isRunning ;

        private int _PORT;
        private TcpListener listener;
        private ConcurrentDictionary<Guid, ClientHandler> clientMap = new();
        private List<ClientHandler> clients = new();

        private ConcurrentDictionary<Guid, GameLobby> lobbyMap = new();
        public UserList userList = new();
        //Events
        // public delegate void UserJoin(User joinedUser);
        // public delegate void UserExit(User exitedUser);
        // public event UserJoin OnUserJoined;
        // public event UserExit OnUserExited;
        public LobbyServer(int PORT)
        {
            listener = new TcpListener(IPAddress.Any, PORT);
            listener.Start();

            _PORT = PORT;
            isRunning = true;
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

                    ClientHandler handler = new ClientHandler(client, this);
                    clients.Add(handler);
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

        // 로비 관리 메서드...
        public GameLobby CreateLobby(string name, int maxPlayers, Guid host)
        {
            Guid lobby_uid = Guid.NewGuid();
            LobbyData data = new LobbyData(lobby_uid, name, maxPlayers, host);
            GameLobby lobby = new GameLobby(data);
            //lobbies.Add(lobby);
            lobbyMap.TryAdd(lobby_uid, lobby);
            return lobby;
        }
        GameLobby? FindLobby(Guid uid)
        {
            //return lobbies.FirstOrDefault(lobby => lobby.data.uid == uid);
            return lobbyMap[uid];
        } 
        public bool JoinLobby(Guid lobbyCode, ClientHandler client)
        {
            return FindLobby(lobbyCode)?.AddPlayer(client) ?? false;
        }
        public bool LeaveLobby(Guid lobbyCode, ClientHandler client)
        {
            return FindLobby(lobbyCode)?.RemovePlayer(client) ?? false;
        }
    }
}