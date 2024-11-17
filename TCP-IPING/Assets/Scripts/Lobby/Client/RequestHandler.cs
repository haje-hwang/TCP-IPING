using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using System.Diagnostics;

public class RequestHandler : IRequest
{
    public bool isRunning;
    static User m_user;
    TcpClient m_tcpClient;
    #nullable enable
    private StreamReader? _reader;
    private StreamWriter? _writer;
    #nullable disable
    private static ConcurrentQueue<IPacket> packetQueue = new ConcurrentQueue<IPacket>();
    private static SemaphoreSlim packetSemaphore = new SemaphoreSlim(0); // 초기화 시 0, 즉 대기 상태

    // private 생성자로 외부에서 직접 호출하지 못하도록 제한
    public RequestHandler(User? user, TcpClient tcpClient) 
    { 
        if(user != null)
        {
            m_user = user;
            DebugMsg($"RequestHandler Created, {user.id}: {user.nickName}");
        }
        else
            DebugMsg($"RequestHandler Created");
            
        m_tcpClient = tcpClient;
          _ = Task.Run(() => Start());  // Start는 백그라운드 스레드에서 실행됨
    }

    // // 비동기 초기화를 위한 팩토리 메서드
    // public static async Task<RequestHandler> CreateAsync(User user)
    // {
    //     m_user = user;
    //     var handler = new RequestHandler();
    //     await handler.Start();
    //     return handler;
    // }

    private void DebugMsg(string msg)
    {
        UnityEngine.Debug.Log(msg);
        // Console.WriteLine(msg);
    }

    private async Task Start()
    {
        DebugMsg("클라이언트 시작.");
        isRunning = true;

        try
        {
            using (NetworkStream stream = m_tcpClient.GetStream())
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
    public void Close()
    {
        isRunning = false;
    }

    /// <summary>
    /// 서버로 메시지를 전송하는 함수
    /// </summary>
    public async Task SendPacketAsync(IPacket Packet)
    {
        if (_writer != null)
        {
            try
            {
                // string jsonPacket = PacketHelper.Serialize(Packet);
                // byte[] data = Constants.Packet.encoding.GetBytes(jsonPacket);
                // int length = data.Length;

                // // 패킷 길이(4바이트)를 먼저 보냄
                // byte[] lengthBytes = BitConverter.GetBytes(length);
                // await _writer.BaseStream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

                // 메시지를 전송
                // await _writer.WriteAsync(jsonPacket);
                // await _writer.FlushAsync(); // 스트림 비우기

                //old
                string jsonPacket = PacketHelper.Serialize(Packet);
                await _writer.WriteLineAsync(jsonPacket);
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
    /// 서버로부터 패킷를 수신 후 큐에 저장 
    /// </summary>
    private async Task ReceivePacketAsync(StreamReader reader)
    {
        try
        {
            // byte[] lengthBuffer = new byte[4];
            while (isRunning)
            {
                /*
                // 1. 먼저 4바이트의 길이 정보를 읽어옴
                int bytesRead = 0;
                while (bytesRead < 4)   // 계속해서 4바이트를 읽을 때까지 시도
                {
                    int readResult = await reader.BaseStream.ReadAsync(lengthBuffer, bytesRead, 4 - bytesRead);
                    bytesRead += readResult;
                }

                int packetLength = BitConverter.ToInt32(lengthBuffer, 0);   //일반적으로 빅 엔디안 사용됨

                // 2. 패킷 길이에 따라 데이터를 읽음
                byte[] dataBuffer = new byte[packetLength];
                int totalBytesRead = 0;

                while (totalBytesRead < packetLength)
                {
                    int remainingBytes = packetLength - totalBytesRead;
                    bytesRead = await reader.BaseStream.ReadAsync(dataBuffer, totalBytesRead, remainingBytes);

                    if (bytesRead == 0)
                    {
                        DebugMsg($"패킷 데이터 읽기 실패: 연결이 끊어졌거나 잘못된 데이터. bytesRead should 0 but :{bytesRead}");
                        break;
                    }
                    
                    totalBytesRead += bytesRead;
                }

                if (totalBytesRead == packetLength)
                {
                    // 3. 데이터를 문자열로 변환 (JSON 디코딩)
                    string jsonPacket = Constants.Packet.encoding.GetString(dataBuffer);
                    IPacket packet = PacketHelper.Deserialize(jsonPacket);
                    if(packet != null)
                    {
                        if(PacketHelper.isBidirectional(packet.type))   //양방향 패킷이면 (응답을 기다리는)
                        {
                            packetQueue.Enqueue(packet);  // 큐에 패킷 저장
                            packetSemaphore.Release();  // 패킷 도착 시 Semaphore를 Release
                        }
                        ProcessBroadcastPacket(packet);
                    }
                }
                else
                {
                    DebugMsg("패킷 크기 불일치: 수신된 데이터가 예상보다 작습니다.");
                }
                */
                ///old one
                string? response = await reader.ReadLineAsync(); // 서버로부터 메시지 수신
                
                IPacket packet = PacketHelper.Deserialize(response);
                if(packet != null)
                {
                    if(PacketHelper.isBidirectional(packet.type))
                    {
                        packetQueue.Enqueue(packet);  // 큐에 패킷 저장
                        packetSemaphore.Release();  // 패킷 도착 시 Semaphore를 Release
                    }
                    ProcessBroadcastPacket(packet);
                }
            }
        }
        catch (Exception ex)
        {
            DebugMsg($"수신 중 오류 발생: {ex.Message}");
        }
    }
    /// <summary>
    /// 응답 패킷이 필요한 요청패킷을 전송 
    /// </summary>
    /// <param name="senderPacket">Request packet</param>
    /// <param name="timeoutSeconds">n초 안에 적절한 Respond 패킷이 오지 않으면 timeout</param>
    /// <returns></returns>
    public async Task<object> SendAndWaitPacketAsync(IPacket senderPacket, int timeoutSeconds = 3)
    {
        DebugMsg($"Client Sent packet:{senderPacket.type}");
        using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(timeoutSeconds));
        IPacket responsePacket = null;  // 응답 패킷을 저장할 변수

        try
        {
            // 패킷 전송
            if (_writer == null) 
                throw new InvalidOperationException("Writer is not initialized");

            string jsonPacket = PacketHelper.Serialize(senderPacket);
            await _writer.WriteLineAsync(jsonPacket);

            // 패킷 수신 및 응답 확인
            while (!cts.IsCancellationRequested)
            {
                // 큐가 비어 있으면 기다림
                if (packetQueue.IsEmpty)
                {
                    await packetSemaphore.WaitAsync(cts.Token); //패킷을 받을 때까지 
                    continue;
                }

                // 큐에서 패킷을 꺼냄
                if (!packetQueue.TryDequeue(out responsePacket))
                    continue;

                // 응답 타입이 일치하면 루프 종료
                if (responsePacket?.type == senderPacket.type)
                    break;
            }
        }
        catch (OperationCanceledException ex)
        {
            // 타임아웃 발생
            DebugMsg($"Timeout occurred: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            // 기타 예외 발생
            DebugMsg($"Error occurred: {ex.Message}");
            throw;
        }

        return responsePacket?.data; // 응답 패킷의 데이터 반환 (응답이 없으면 null 반환)
    }


    /// <summary>
    /// 서버에게 받은 패킷 처리 (요청-응답 패킷 제외)
    /// </summary>
    /// <param name="packet"></param>
    private void ProcessBroadcastPacket(IPacket packet)
    {
        //SendAndWaitPacketAsync()를 통해 요청-응답을 받는 경우는 제외
        try
        {
            switch(packet.type)
            {
                case PacketType.Booted:
                    break;
                case PacketType.LobbyMessege:
                    break;
                case PacketType.GameStarted:
                    break;
                case PacketType.Quiz:
                    break;
                case PacketType.Timer:
                    break;
                case PacketType.GameData:
                    break;
            }
        }
        catch (System.Exception e)
        {
            DebugMsg(e.Message);
            throw;
        }  
    }

    #region IRequest 구현
    public async Task<User> FirstJoin()
    {
        IPacket senderPacket = new IPacket(PacketType.__FirstJoin, null, Guid.Empty);
        return await SendAndWaitPacketAsync(senderPacket) as User;
    }
     public async Task<List<Lobby.LobbyData>> GetLobbyList()
    {
        IPacket senderPacket= new IPacket(PacketType.__LobbyList, null, m_user.id);
        return await SendAndWaitPacketAsync(senderPacket) as List<Lobby.LobbyData>; 
    }
    public async Task<Lobby.LobbyData> GetLobbyData()
    {
        IPacket senderPacket= new IPacket(PacketType.__LobbyData, null, m_user.id);
        return await SendAndWaitPacketAsync(senderPacket) as Lobby.LobbyData; 
    }
    public async void StartJoin()
    {
        IPacket packet = new IPacket(PacketType._StartJoin, null, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void EndJoin()
    {
        IPacket packet = new IPacket(PacketType._EndJoin, null, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void UpdateUser(User user)
    {
        IPacket packet = new IPacket(PacketType._UpdateUserData, user, m_user.id);
        await SendPacketAsync(packet);
    }
    //Lobby
    public async void CreateLobby(string LobbyName)
    {
        IPacket packet = new IPacket(PacketType._CreateLobby, LobbyName, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void JoinLobby(Guid lobbyID)
    {
        IPacket packet = new IPacket(PacketType._UpdateUserData, lobbyID, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void LeaveLobby()
    {
        IPacket packet = new IPacket(PacketType._UpdateUserData, null, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void LobbyReady(bool ReadyState)
    {
        IPacket packet = new IPacket(PacketType._UpdateUserData, ReadyState, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void SendLobbyMessege(string messege)
    {
        IPacket packet = new IPacket(PacketType._UpdateUserData, messege, m_user.id);
        await SendPacketAsync(packet);
    }
    //In Game
    public async void SendAnswer(int answer)
    {
        IPacket packet = new IPacket(PacketType._Answer, answer, m_user.id);
        await SendPacketAsync(packet);
    }
    #endregion
}
