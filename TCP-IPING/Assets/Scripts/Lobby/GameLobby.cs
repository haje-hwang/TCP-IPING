using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lobby
{
    public enum LobbyMode
    {
        PUBLIC,
        PRIVATE
    }
    public class GameLobby
    {
        public UID uid { get; private set; }    
        public string name { get; private set; }
        public int maxPlayers { get; private set; }
        public LobbyMode state { get; private set; }
        public bool isRunning {get; private set;}

        public List<ClientHandler> players { get; private set; }
        private TcpListener listener;
        //Events
        public delegate void LobbyJoin(User joinedUser);
        public delegate void LobbyExit(User exitedUser);
        public event LobbyJoin OnLobbyJoined;
        public event LobbyExit OnLobbyExited;

        public GameLobby(UID uid, string name, int maxPlayers)
        {
            this.uid = uid;
            this.name = name;
            this.maxPlayers = maxPlayers;
            players = new List<ClientHandler>();

            state = LobbyMode.PUBLIC;
        }
        public async Task StartHosting(int port)
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
                    AddPlayer(handler);
                    await handler.Start();
                }
                catch (System.Exception)
                {
                    
                    throw;
                }
            }
        }
        ~GameLobby()
        {
            StopHosting();
        }
        public void StopHosting()
        {
            isRunning = false;
            listener.Stop();
        }
        //
        public bool AddPlayer(ClientHandler player)
        {
            if (players.Count < maxPlayers)
            {
                players.Add(player);
                OnLobbyJoined?.Invoke(player.user);
                return true;
            }
            return false;
        }

        public void RemovePlayer(ClientHandler player)
        {
            OnLobbyExited?.Invoke(player.user);
            players.Remove(player);
        }
        //
        public void BroadcastMessage(string message)
        {
            foreach (var player in players)
            {
                player.SendMessage(message);
            }
        }
        // 기타 로비 관련 메서드들...
    }
}