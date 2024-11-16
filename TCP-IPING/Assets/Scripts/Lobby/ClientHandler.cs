using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;

namespace Lobby
{
    public class ClientHandler //: IResponse
    {
        LobbyServer server;
        //
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
            IPacket packet = new IPacket(PacketType.Message, message, UID.Empty());
            SendPacket(packet);
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
                    if(packet.id == 0)
                        DefineUser();
                    break;
                case PacketType.EndJoin:
                    //서버에서 퇴장
                    this.Close();
                    break;
                case PacketType.CreateLobby:
                    //로비 생성
                    string roomName = (string)packet.data;
                    if(string.IsNullOrEmpty(roomName))
                        roomName = $"{UserList.Instance.ReadUser(packet.id).nickName}'s Lobby";

                    SendLobbyData(server.CreateLobby(roomName, 4, packet.id));
                    break;
                case PacketType.JoinLobby:
                    //packet.data로 LobbyID를 받기
                    //이후 Lobby에 해당 유저 넣기
                    server.JoinLobby((UID)packet.data, this);
                    break;
                case PacketType.LeaveLobby:
                    //packet.data로 LobbyID를 받기
                    //이후 Lobby에 해당 유저 삭제
                    server.LeaveLobby((UID)packet.data, this);
                    break;
                case PacketType.Answer:
                    ReceivedAnswer();
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

        void DefineUser()
        {
            User newUser = UserList.Instance.CreateNewUser();
            IPacket packet = new(PacketType._DefineUser, newUser, UID.Empty());
            SendPacket(packet);
        }
        void SendLobbyData(GameLobby lobby)
        {
            IPacket packet = new(PacketType._LobbyData, lobby, UID.Empty());
            SendPacket(packet);
        }

        void ReceivedAnswer()
        {
            
        }

        void LeaveLobby()
        {
            //
        }
    }
}