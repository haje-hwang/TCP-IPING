using System;
using System.Buffers;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Server_TCP;
using Server_TCP.Lobby;

namespace Server_TCP
{
    public class ClientHandler
    {
        Server_TCP.Lobby.PacketEventDispatcher eventDispatcher;
        private TcpClient m_client;
        private NetworkStream m_stream;
#nullable enable
        User? m_user;
        public Guid joindLobbyId;
#nullable disable
        private readonly ArrayPool<byte> _bufferPool = ArrayPool<byte>.Shared;

        public ClientHandler(TcpClient client, PacketEventDispatcher eventDispatcher)
        {
            this.m_client = client;
            this.eventDispatcher = eventDispatcher;
            m_stream = m_client.GetStream();
            joindLobbyId = Guid.Empty;
        }
        public void Disconnect()
        {
            if (m_stream != null)
                m_stream.Close();

            if (m_client != null)
                m_client.Close();

            eventDispatcher.ServerExit(this);
            Debug.Log($"Client {m_user?.id} disconnected.");
        }
        public User GetUser() { return m_user; }
        public void SetUser(User user) { m_user = user; }

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
                    buffer = _bufferPool.Rent(packetLength + 1);
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
                    Debug.Log($"Received from ({m_user?.nickName}, {m_user?.id}): {packetLength}, {message}");
                    ReceivedPacket(message);
                }
            }
            catch (Exception ex)
            {
                Debug.LogWarning($"Client {m_user?.id} error: {ex.Message}");
                throw;
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

                //Debug.Log($"Send to {m_user?.nickName}, {m_user?.id}: {length}, {message}");

                // 패킷 길이(4바이트)를 먼저 보냄
                byte[] lengthBytes = BitConverter.GetBytes(length);
                await m_stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

                // 메시지를 전송
                await m_stream.WriteAsync(data, 0, data.Length);
                Debug.Log($"Sent to {m_user?.nickName}, {m_user?.id}: {length}, {message}");
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
                eventDispatcher.ProcessPacket(Packet, this);
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
    }
}