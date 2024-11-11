using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Lobby
{
    public class ClientHandler
    {
        public User user;
        public bool isRunning = true;
        private TcpClient client;
        private NetworkStream stream;

        public ClientHandler(TcpClient client)
        {
            this.client = client;
            stream = client.GetStream();
        }

        public async Task Start()
        {
            await Task.Run(() => HandleClient());
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
                UnityEngine.Debug.Log("ClientHandler Closed");
                Close();
            }
            catch (System.Exception)
            {
                
                throw;
            }
        }
        public void Close()
        {
            isRunning = false;
            client.Close();
        }

        public void SendMessage(string message)
        {
            IPacket packet = new IPacket(PacketType.Message, message);
            SendPacket(packet);
        }

        private void ProcessPacket(string jsonPacket)
        {
            try
            {
                IPacket packet = PacketHelper.Deserialize(jsonPacket);
                // 패킷 처리 로직 구현
                HandlePacketType(packet);
                UnityEngine.Debug.Log($"Packet: {packet.type}, {packet.data}");
            }
            catch (System.Exception)
            {
                
                throw;
            }  
        }

        private void HandlePacketType(IPacket packet)
        {
            switch (packet.type)
            {
                case PacketType.StartJoin:
                    //서버에 접속할 때
                    UID uid = (UID)packet.data;
                    if(uid == 0)
                        DefineUser();
                    break;
                case PacketType.JoinLobby:
                    //packet.data로 LobbyID를 받기
                    //이후 Lobby에 해당 유저 넣기
                    break;
                case PacketType.LeaveLobby:
                    //packet.data로 LobbyID를 받기
                    //이후 Lobby에 해당 유저 삭제
                    break;
                case PacketType.Answer:
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

        private void DefineUser()
        {
            User newUser = UserList.Instance.CreateNewUser();
            IPacket packet = new IPacket(PacketType._DefineUser, newUser);
            SendPacket(packet);
        }
    }
}