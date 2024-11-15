using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Lobby
{
    public class ClientHandler
    {
        LobbyServer server;
        //
        public User user;
        public bool isRunning = true;
        private TcpClient client;
        private NetworkStream stream;

        public ClientHandler(TcpClient client, LobbyServer server)
        {
            this.client = client;
            this.server = server;
            stream = client.GetStream();
        }

        public async Task Start()
        {
            await Task.Run(() => HandleClient());
        }
        public void Close()
        {
            isRunning = false;
        }
        public void SendMessage(string message)
        {
            IPacket packet = new IPacket(PacketType.Message, message);
            SendPacket(packet);
        }
        public async void SendPacket(IPacket packet)
        {
            try
            {
                packet.id = 1;
                string jsonPacket = PacketHelper.Serialize(packet);
                byte[] buffer = Constants.Packet.encoding.GetBytes(jsonPacket);
                await stream.WriteAsync(buffer, 0, buffer.Length);
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }


        private async Task HandleClient()
        {
            try
            {
                byte[] buffer = new byte[Constants.Packet.bufferLength];
                while (isRunning)
                {
                    //데이터를 수신 대기
                    int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                    if (bytesRead == 0) break;  // 클라이언트가 연결을 종료한 경우

                    string message = Constants.Packet.encoding.GetString(buffer, 0, bytesRead);
                    ProcessPacket(message);
                }
                _Close();
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.LogWarning(e);
                throw;
            }
        }
        private void _Close()
        {
            UnityEngine.Debug.Log("ClientHandler Closed");
            client.Close();
        }


        /// <summary>
        /// 받은 string을 패킷으로 Deserialize
        /// </summary>
        /// <param name="jsonPacket"></param>
        private void ProcessPacket(string jsonPacket)
        {
            try
            {
                IPacket packet = PacketHelper.Deserialize(jsonPacket);
                // 패킷 처리 로직 구현
                PacketHandler packetHandler = new PacketHandler(server, packet, this);
                UnityEngine.Debug.Log($"Packet: {packet.type}, {packet.data}");
            }
            catch (System.Exception)
            {
                
                throw;
            }  
        }
    }
}