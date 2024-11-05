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

        private async void HandleClient()
        {
            try
            {
                byte[] buffer = new byte[Constants.Packet.bufferLength];
                while (isRunning)
                {
                    //데이터를 보낼 때까지 대기하고 처리
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;

                    string message = Constants.Packet.encoding.GetString(buffer, 0, bytesRead);
                    ProcessPacket(message);
                }
                UnityEngine.Debug.Log("ClientHandler Closed");
                client.Close();
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }

        public void SendMessage(string message)
        {
            IPacket packet = new IPacket("message", message);
            SendPacket(packet);
        }

        private void ProcessPacket(string jsonPacket)
        {
            try
            {
                IPacket packet = PacketHelper.Deserialize(jsonPacket);
                // 패킷 처리 로직 구현
                UnityEngine.Debug.Log($"Packet: {packet.PacketType}, {packet.Data}");
            }
            catch (System.Exception)
            {
                
                throw;
            }  
        }

        public async void SendPacket(IPacket packet)
        {
            try
            {
                string jsonPacket = PacketHelper.Serialize(packet);
                byte[] buffer = Constants.Packet.encoding.GetBytes(jsonPacket);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
    }
}