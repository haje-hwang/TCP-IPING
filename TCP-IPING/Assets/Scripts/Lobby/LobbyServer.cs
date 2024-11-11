using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Linq;

namespace Lobby
{
    public class LobbyServer
    {
        public bool isRunning = true;
        private TcpListener listener;
        private List<ClientHandler> clients = new List<ClientHandler>();
        private List<GameLobby> lobbies = new List<GameLobby>();
        //Events
        public delegate void UserJoin(User joinedUser);
        public delegate void UserExit(User exitedUser);
        public event UserJoin OnUserJoined;
        public event UserExit OnUserExited;
        
        // public void UserJoin(User joinedUser)
        // {
        //     OnUserJoined?.Invoke(joinedUser);
        // }


        public async void Start(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            // Console.WriteLine($"서버가 {port}번 포트에서 시작되었습니다.");
            UnityEngine.Debug.Log($"서버가 {port}번 포트에서 시작되었습니다.");

            while (isRunning)
            {
                try
                {
                    // 비동기 방식으로 클라이언트 연결을 기다림
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    UnityEngine.Debug.Log("클라이언트가 연결되었습니다.");
                    
                    ClientHandler handler = new ClientHandler(client);
                    clients.Add(handler);
                    await handler.Start();
                }
                catch (System.Exception)
                {
                    
                    throw;
                }
            }
        }

        public void BroadcastMessage(string message)
        {
            foreach (var client in clients)
            {
                client.SendMessage(message);
            }
        }

        public GameLobby CreateLobby(string name, int maxPlayers)
        {
            UID lobby_uid = UID.NewUID();
            GameLobby lobby = new GameLobby(lobby_uid, name, maxPlayers);
            lobbies.Add(lobby);
            return lobby;
        }

        // 기타 로비 관리 메서드들...
        GameLobby FindLobby(UID uid)
        {
            return lobbies.FirstOrDefault(lobby => lobby.uid == uid);
        } 
    }
}