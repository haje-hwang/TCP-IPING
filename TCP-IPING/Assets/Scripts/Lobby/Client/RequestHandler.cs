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
    static User m_user;
    private bool isRunning;
    private string serverAddress = "127.0.0.1"; // 서버 IP 주소 (로컬호스트)
    private int port = 8080; // 서버 포트 번호
    #nullable enable
    private StreamReader? _reader;
    private StreamWriter? _writer;
    #nullable disable
    private static ConcurrentQueue<IPacket> packetQueue = new ConcurrentQueue<IPacket>();
    private static SemaphoreSlim packetSemaphore = new SemaphoreSlim(0); // 초기화 시 0, 즉 대기 상태

    // private 생성자로 외부에서 직접 호출하지 못하도록 제한
    private RequestHandler() { }

    // 비동기 초기화를 위한 팩토리 메서드
    public static async Task<RequestHandler> CreateAsync(User user)
    {
        m_user = user;
        var handler = new RequestHandler();
        await handler.Start();
        return handler;
    }

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
            using (TcpClient client = new TcpClient())
            {
                // 서버에 연결
                await client.ConnectAsync(serverAddress, port);
                DebugMsg($"서버에 연결됨: {serverAddress}:{port}");

                using (NetworkStream stream = client.GetStream())
                {
                    _reader = new StreamReader(stream, Constants.Packet.encoding);
                    _writer = new StreamWriter(stream, Constants.Packet.encoding) { AutoFlush = true };

                    // 수신 작업 실행
                    await ReceivePacketAsync();
                }
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
    private async Task ReceivePacketAsync()
    {
        while (isRunning)
        {
            try
            {
                if (_reader != null)
                {
                    string? response = await _reader.ReadLineAsync(); // 서버로부터 메시지 수신
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
                isRunning = false; // 오류 발생 시 루프 종료
                break;
            }
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
    public async void FirstJoin()
    {
        IPacket packet = new IPacket(PacketType.__FirstJoin, null, UID.Empty());
        await SendPacketAsync(packet);
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
    public async void CreateLobby(string LobbyName)
    {
        IPacket packet = new IPacket(PacketType._CreateLobby, LobbyName, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void JoinLobby(UID lobbyID)
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
