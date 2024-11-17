using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server_TCP;

namespace Lobby
{
    public class ClientHandler : IResponse
    {
        LobbyServer m_server;
        private TcpClient m_client;
        private NetworkStream m_stream;
#nullable enable
        User? m_user;
#nullable disable

        public ClientHandler(TcpClient client, LobbyServer server)
        {
            this.m_client = client;
            this.m_server = server;
            m_stream = m_client.GetStream();
        }
        public void Disconnect()
        {
            if (m_stream != null)
                m_stream.Close();

            if (m_client != null)
                m_client.Close();

            m_server.ServerExit(this);
            Debug.Log($"Client {m_user?.id} disconnected.");
        }
        public async Task Start()
        {
            try
            {
                // 수신 작업 실행
                await ReceiveMessegesync(m_stream);
            }
            catch (Exception ex)
            {
                Debug.Log($"오류 발생: {ex.Message}");
            }
        }
        public async Task ReceiveMessegesync(NetworkStream stream)
        {
            try
            {
                byte[] buffer;
                byte[] lengthBuffer = new byte[4]; // 길이 정보를 저장할 버퍼
                while (m_client.Connected)
                {
                    // 1. 패킷 길이 읽기
                    int bytesRead = 0;
                    while (bytesRead < 4)
                    {
                        int readResult = await m_stream.ReadAsync(lengthBuffer, bytesRead, 4 - bytesRead);
                        if (readResult == 0)
                        {
                            return; // 연결 종료 처리
                        }
                        bytesRead += readResult;
                    }

                    // 2. 길이 정보 변환
                    int packetLength = BitConverter.ToInt32(lengthBuffer, 0); // 기본 Little Endian 사용
                    if (packetLength <= 0)
                    {
                        Debug.LogWarning($"Invalid packet length from : {packetLength}");
                        continue; // 비정상 패킷 무시
                    }

                    // 3. 패킷 데이터 읽기
                    buffer = new byte[packetLength];
                    int totalBytesRead = 0;

                    while (totalBytesRead < packetLength)
                    {
                        int remainingBytes = packetLength - totalBytesRead;
                        bytesRead = await m_stream.ReadAsync(buffer, totalBytesRead, remainingBytes);

                        if (bytesRead == 0)
                        {
                            Debug.Log($"disconnected during packet read.");
                            return; // 연결 종료 처리
                        }

                        totalBytesRead += bytesRead;
                    }

                    // 4. 데이터 처리
                    string message = Encoding.UTF8.GetString(buffer);
                    ReceivedPacket(message);
                    Debug.Log($"Received from {m_user?.id}: {packetLength}, {message}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Client {m_user?.id} error: {ex.Message}");
            }
            finally
            {
                Disconnect();
            }
        }

        public async Task SendMessegeAsync(string message)
        {
            if (m_client == null || !m_client.Connected)
            {
                Debug.LogWarning("Not connected to server!");
                return;
            }
            if (m_stream == null || !m_stream.CanWrite)
            {
                Debug.LogWarning("NetworkStream cannot write!");
                return;
            }

            try
            {
                byte[] data = System.Text.Encoding.UTF8.GetBytes(message);
                int length = data.Length;

                // 패킷 길이(4바이트)를 먼저 보냄
                byte[] lengthBytes = BitConverter.GetBytes(length);
                await m_stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

                // 메시지를 전송
                await m_stream.WriteAsync(data, 0, data.Length);
            }
            catch (ObjectDisposedException ex)
            {
                Debug.LogWarning("Stream has been closed: " + ex.Message);
                throw;
            }
            catch (OperationCanceledException ex)
            {
                Debug.LogWarning("Read operation was canceled: " + ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Error sending message: {ex.Message}");
                throw;
            }
        }
        private void ReceivedPacket(string jsonPacket)
        {
            try
            {
                IPacket Packet = JsonHelper<IPacket>.Deserialize(jsonPacket);
                ProcessPacket(Packet);
            }
            catch (Exception)
            {
                throw;
            }
        }
        public async void SendPacketAsync(IPacket packet)
        {
            try
            {
                string jsonPacket = JsonHelper<IPacket>.Serialize(packet);
                await SendMessegeAsync(jsonPacket);
            }
            catch (System.Exception)
            {
                throw;
            }
        }

        private void ProcessPacket(IPacket packet)
        {
            switch (packet.type)
            {
                case PacketType.__FirstJoin:
                    DefineUser();
                    break;
                case PacketType.__LobbyData:
                    break;
                case PacketType.__LobbyList:
                    break;
                case PacketType._StartJoin:
                    //서버에 접속할 때
                    if (packet.id == Guid.Empty)
                        DefineUser();
                    break;
                case PacketType._EndJoin:
                    //서버에서 퇴장
                    Disconnect();
                    break;
                case PacketType._CreateLobby:
                    //로비 생성
                    string roomName = (string)packet.data;
                    if (string.IsNullOrEmpty(roomName))
                        roomName = $"{m_server.userList.ReadUser(packet.id).nickName}'s Lobby";

                    SendLobbyData(m_server.CreateLobby(roomName, 4, packet.id).data);
                    break;
                case PacketType._JoinLobby:
                    //packet.data로 LobbyID를 받기
                    //이후 Lobby에 해당 유저 넣기
                    m_server.JoinLobby((Guid)packet.data, this);
                    break;
                case PacketType._LeaveLobby:
                    //packet.data로 LobbyID를 받기
                    //이후 Lobby에 해당 유저 삭제
                    m_server.LeaveLobby((Guid)packet.data, this);
                    break;
                case PacketType._Answer:
                    ReceivedAnswer();
                    break;
                case PacketType._SendLobbyMessege:
                    break;
                case PacketType._UpdateUserData:
                    User user = (User)packet.data;
                    m_server.userList.UpdateUser(user);
                    break;
                default:
                    break;
            }
        }


        #region IResponse구현
        //양방향
        public void DefineUser()
        {
            User newUser = m_server.userList.CreateNewUser();
            Guid id = newUser.id;
            IPacket packet = new(PacketType.__FirstJoin, id, Guid.Empty);
            SendPacketAsync(packet);
        }
        public void SendLobbyList(List<Lobby.LobbyData> lobbyList)
        {
            IPacket packet = new(PacketType.__FirstJoin, lobbyList, Guid.Empty);
            SendPacketAsync(packet);
        }
        public void SendLobbyData(LobbyData lobbyData)
        {
            IPacket packet = new(PacketType.__LobbyData, lobbyData, Guid.Empty);
            SendPacketAsync(packet);
        }
        //단방향 broadcasting
        //Lobby
        public void Booted()
        {

        }
        public void LobbyMessege(string who, string message)
        {

        }
        //In Game
        public void GameStarted()
        {

        }
        public void ReceivedAnswer()
        {

        }
        public void LastTimer()
        {

        }
        #endregion
    }
}