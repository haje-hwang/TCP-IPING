using System;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.IO;

namespace Lobby
{
    public class ClientHandler : IResponse
    {
        LobbyServer server;
        //
        public bool isRunning = true;
        private TcpClient client;
        private NetworkStream stream;
        #nullable enable
        private StreamReader? _reader;
        private StreamWriter? _writer;
        #nullable disable

        public ClientHandler(TcpClient client, LobbyServer server)
        {
            this.client = client;
            this.server = server;
        }

        private void DebugMsg(string msg)
        {
            UnityEngine.Debug.Log(msg);
            // Console.WriteLine(msg);
        }

        public async Task Start()
        {
            await Task.Run(() => HandleClient());
        }
        public void Close()
        {
            isRunning = false;
        }
        private async Task HandleClient()
        {
            isRunning = true;
            try
            {
                /* old one
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
                */
                using (NetworkStream stream = client.GetStream())
                {
                    _reader = new StreamReader(stream, Constants.Packet.encoding);
                    _writer = new StreamWriter(stream, Constants.Packet.encoding) { AutoFlush = true };

                    // 수신 작업 실행
                    await ReceivePacketAsync(_reader);
                }
            }
            catch (Exception ex)
            {
                DebugMsg($"오류 발생: {ex.Message}");
            }
            DebugMsg("클라이언트 종료.");
        }
        private async Task ReceivePacketAsync(StreamReader reader)
        {
            try
            {
                byte[] lengthBuffer = new byte[4];
                while (isRunning)
                {
                    // 1. 먼저 4바이트의 길이 정보를 읽어옴
                    int bytesRead = await reader.BaseStream.ReadAsync(lengthBuffer, 0, lengthBuffer.Length);
                    if (bytesRead != 4)
                    {
                        DebugMsg("패킷 길이 읽기 실패: 연결이 끊어졌거나 잘못된 데이터.");
                        break;
                    }

                    int packetLength = BitConverter.ToInt32(lengthBuffer, 0);

                    // 2. 패킷 길이에 따라 데이터를 읽음
                    byte[] dataBuffer = new byte[packetLength];
                    int totalBytesRead = 0;

                    while (totalBytesRead < packetLength)
                    {
                        int remainingBytes = packetLength - totalBytesRead;
                        bytesRead = await reader.BaseStream.ReadAsync(dataBuffer, totalBytesRead, remainingBytes);

                        if (bytesRead == 0)
                        {
                            DebugMsg("패킷 데이터 읽기 실패: 연결이 끊어졌거나 잘못된 데이터.");
                            break;
                        }
                        
                        totalBytesRead += bytesRead;
                    }

                    if (totalBytesRead == packetLength)
                    {
                        // 3. 데이터를 문자열로 변환 (JSON 디코딩)
                        string jsonPacket = Constants.Packet.encoding.GetString(dataBuffer);

                        // 4. 패킷 처리 로직 호출
                        ProcessPacket(jsonPacket);
                    }
                    else
                    {
                        DebugMsg("패킷 크기 불일치: 수신된 데이터가 예상보다 작습니다.");
                    }
                }
            }
            catch (Exception ex)
            {
                DebugMsg($"수신 중 오류 발생: {ex.Message}");
            }
        }
        public async void SendPacketAsync(IPacket packet)
        {
            if (_writer != null)
            {
                try
                {
                    /*old one
                    string jsonPacket = PacketHelper.Serialize(packet);
                    byte[] buffer = Constants.Packet.encoding.GetBytes(jsonPacket);
                    await stream.WriteAsync(buffer, 0, buffer.Length);
                    */

                    string jsonPacket = PacketHelper.Serialize(packet);
                    byte[] data = Constants.Packet.encoding.GetBytes(jsonPacket);
                    int length = data.Length;

                    // 패킷 길이(4바이트)를 먼저 보냄
                    byte[] lengthBytes = BitConverter.GetBytes(length);
                    await _writer.BaseStream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

                    // 메시지를 전송
                    await _writer.WriteAsync(jsonPacket);
                    await _writer.FlushAsync(); // 스트림 비우기
                }
                catch (System.Exception)
                {
                    
                    throw;
                }
            }
            else
            {
                DebugMsg("Do RequestHandler.CreateAsync(User) First!");
            }
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
                case PacketType.__FirstJoin:
                    break;
                case PacketType.__LobbyData:
                    break;
                case PacketType.__LobbyList:
                    break;
                case PacketType._StartJoin:
                    //서버에 접속할 때
                    if(packet.id == 0)
                        DefineUser();
                    break;
                case PacketType._EndJoin:
                    //서버에서 퇴장
                    this.Close();
                    break;
                case PacketType._CreateLobby:
                    //로비 생성
                    string roomName = (string)packet.data;
                    if(string.IsNullOrEmpty(roomName))
                        roomName = $"{UserList.Instance.ReadUser(packet.id).nickName}'s Lobby";

                    SendLobbyData(server.CreateLobby(roomName, 4, packet.id).data);
                    break;
                case PacketType._JoinLobby:
                    //packet.data로 LobbyID를 받기
                    //이후 Lobby에 해당 유저 넣기
                    server.JoinLobby((UID)packet.data, this);
                    break;
                case PacketType._LeaveLobby:
                    //packet.data로 LobbyID를 받기
                    //이후 Lobby에 해당 유저 삭제
                    server.LeaveLobby((UID)packet.data, this);
                    break;
                case PacketType._Answer:
                    ReceivedAnswer();
                    break;
                case PacketType._SendLobbyMessege:
                    break;
                case PacketType._UpdateUserData:
                    User user = (User)packet.data;
                    UserList.Instance.UpdateUser(user);
                    break;
                default:
                    break;
            }
        }


        #region IResponse구현
        //양방향
        public void DefineUser()
        {
            User newUser = UserList.Instance.CreateNewUser();
            IPacket packet = new(PacketType.__FirstJoin, newUser, UID.Empty());
            SendPacketAsync(packet);
        }
        public void SendLobbyList(List<Lobby.LobbyData> lobbyList)
        {
            IPacket packet = new(PacketType.__FirstJoin, lobbyList, UID.Empty());
            SendPacketAsync(packet);
        }
        public void SendLobbyData(LobbyData lobbyData)
        {
            IPacket packet = new(PacketType.__LobbyData, lobbyData, UID.Empty());
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