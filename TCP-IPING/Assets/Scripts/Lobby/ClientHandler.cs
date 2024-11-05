using System;
using System.Net.Sockets;
using System.Threading;

namespace Lobby
{
    public class ClientHandler
    {
        public bool isRunning = true;
        private TcpClient client;
        private LobbyServer server;
        private NetworkStream stream;

        public ClientHandler(TcpClient client, LobbyServer server)
        {
            this.client = client;
            this.server = server;
            stream = client.GetStream();
        }

        public void Start()
        {
            Thread thread = new Thread(HandleClient);
            thread.Start();
        }

        private void HandleClient()
        {
            byte[] buffer = new byte[1024];
            while (isRunning)
            {
                int bytesRead = stream.Read(buffer, 0, buffer.Length);
                if (bytesRead == 0) break;

                string message = System.Text.Encoding.ASCII.GetString(buffer, 0, bytesRead);
                ProcessMessage(message);
            }
            UnityEngine.Debug.Log("ClientHandler Closed");
            client.Close();
        }

        private void ProcessMessage(string message)
        {
            // 메시지 처리 로직 구현
        }

        public void SendMessage(string message)
        {
            byte[] buffer = System.Text.Encoding.ASCII.GetBytes(message);
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}