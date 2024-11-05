using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        public void Start(int port)
        {
            listener = new TcpListener(IPAddress.Any, port);
            listener.Start();
            // Console.WriteLine($"서버가 {port}번 포트에서 시작되었습니다.");
            UnityEngine.Debug.Log($"서버가 {port}번 포트에서 시작되었습니다.");

            while (isRunning)
            {
                // 비동기 방식으로 클라이언트 연결을 기다림
                TcpClient client = listener.AcceptTcpClient();
                UnityEngine.Debug.Log("클라이언트가 연결되었습니다.");

                ClientHandler handler = new ClientHandler(client, this);
                clients.Add(handler);
                handler.Start();
                
                NetworkStream stream = client.GetStream();
                byte[] buffer = new byte[1024];
                //bytesRead: 클라이언트로부터 실제로 읽은 데이터의 크기(바이트 수)
                int bytesRead = stream.Read(buffer, 0, buffer.Length);

                //Encoding.ASCII.GetString(): 바이트 배열을 문자열로 변환하는 메서드
                string receivedMessage = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                UnityEngine.Debug.Log("Received: " + receivedMessage);
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
            GameLobby lobby = new GameLobby(name, maxPlayers);
            lobbies.Add(lobby);
            return lobby;
        }

        // 기타 로비 관리 메서드들...
        GameLobby FindLobby(Guid guid)
        {
            return lobbies.FirstOrDefault(lobby => lobby.uuid == guid);
        }
    }
}