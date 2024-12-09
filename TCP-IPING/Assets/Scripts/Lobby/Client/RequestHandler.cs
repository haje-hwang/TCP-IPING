using System;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Threading;
using System.Collections.Concurrent;
using Lobby;
using Newtonsoft.Json.Linq;

public class RequestHandler : IRequest
{
    public bool isRunning;
    public bool isSending;
    User m_user;
    TcpClient m_client;
    NetworkStream m_stream;
    private static ConcurrentQueue<IPacket> rcvPacketQueue = new ConcurrentQueue<IPacket>();
    private static ConcurrentQueue<string> sendMessegeQueue = new ConcurrentQueue<string>();
    private static SemaphoreSlim packetSemaphore = new SemaphoreSlim(0); // 초기화 시 0, 즉 대기 상태

    //Event
    public event EventHandler<Lobby.LobbyData> OnLobbyUpdate;

    // private 생성자로 외부에서 직접 호출하지 못하도록 제한
    public RequestHandler(User user, TcpClient tcpClient) 
    { 
        if(user != null)
        {
            m_user = user;
            DebugMsg($"RequestHandler Created, {user.id}: {user.nickName}");
        }
        else
            DebugMsg($"RequestHandler Created");

        m_client = tcpClient;
         _ = Task.Run(() => Start());  // Start는 백그라운드 스레드에서 실행됨
    }
    ~RequestHandler()
    {
        Disconnect();
    }
    public User GetUser() {return m_user; }
    private void DebugMsg(string msg)
    {
        UnityEngine.Debug.Log("[Client] "+msg);
        // Console.WriteLine(msg);
    }
    private void DebugWaringMsg(string msg)
    {
        UnityEngine.Debug.LogWarning("[Client] "+msg);
        // Console.WriteLine(msg);
    }
    public void Disconnect()
    {
        isRunning = false;
        if (m_stream != null)
            m_stream.Close();

        if (m_client != null)
            m_client.Close();

        DebugMsg("Disconnected from the server.");
    }
    
#region 클라이언트 측 통신 구현
    private async Task Start()
    {
        DebugMsg("클라이언트 시작.");
        isRunning = true;
        isSending = false;
        try
        {
            m_stream = m_client.GetStream();
            // 수신 작업 실행
            _ = Task.Run(() => ReceivePacketAsync(m_stream));

            //User가 정의되지 않은 첫 접속이면, User정보 받기
            if(m_user == null)
            {
                Guid id = await FirstJoin();
                m_user = new User(id, "Annonymous");
                DebugMsg($"User Confirmed : {m_user?.id}, {m_user.nickName}");
            }
            else
            {
                StartJoin();
            }
        }
        catch (Exception ex)
        {
            DebugWaringMsg($"Error in RequestHandler.Start(): {ex.Message}");
        }
    }
    public async Task ReceivePacketAsync(NetworkStream stream)
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
                    int readResult = await stream.ReadAsync(lengthBuffer, bytesRead, 4 - bytesRead);
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
                    DebugWaringMsg($"Invalid packet length from : {packetLength}");
                    continue; // 비정상 패킷 무시
                }

                // 3. 패킷 데이터 읽기
                buffer = new byte[packetLength];
                int totalBytesRead = 0;

                while (totalBytesRead < packetLength)
                {
                    int remainingBytes = packetLength - totalBytesRead;
                    bytesRead = await stream.ReadAsync(buffer, totalBytesRead, remainingBytes);

                    if (bytesRead == 0)
                    {
                        DebugWaringMsg($"disconnected during packet read.");
                        return; // 연결 종료 처리
                    }
                    totalBytesRead += bytesRead;
                }

                // 4. 데이터 처리
                string message = Constants.Packet.encoding.GetString(buffer);
                ReceivedPacket(message);
                DebugMsg($"Received : {packetLength}, {message}");
            }
        }
        catch (Exception ex)
        {
            DebugWaringMsg($"Error reading from server: {ex.Message}");
        }
        Disconnect();
    }
    private async Task SendMessegeAsync(string message)
    {
        if (m_client == null || !m_client.Connected)
        {
            DebugWaringMsg("Not connected to server!");
            return;
        }
        if (m_stream == null || !m_stream.CanWrite)
        {
            DebugWaringMsg("NetworkStream cannot write!");
            return;
        }

        try
        {
            byte[] data = Constants.Packet.encoding.GetBytes(message);
            int length = data.Length;
            DebugMsg($"SendMessegeAsync: {length}/{message}");

            // 패킷 길이(4바이트)를 먼저 보냄
            byte[] lengthBytes = BitConverter.GetBytes(length);
            await m_stream.WriteAsync(lengthBytes, 0, lengthBytes.Length);

            // 메시지를 전송
            await m_stream.WriteAsync(data, 0, data.Length);
        }
        catch (ObjectDisposedException ex)
        {
            DebugWaringMsg("Stream has been closed: " + ex.Message);
            throw;
        }
        catch (OperationCanceledException ex)
        {
            DebugWaringMsg("Read operation was canceled: " + ex.Message);
            throw;
        }
        catch (Exception ex)
        {
            DebugWaringMsg($"Error sending message: {ex.Message}");
            throw;
        }
    }
    private void ReceivedPacket(string jsonPacket)
    {
        try
        {
            IPacket packet = JsonHelper<IPacket>.Deserialize(jsonPacket);
            if(packet != null)
            {
                if(IPacket.isBidirectional(packet.type))
                {
                    rcvPacketQueue.Enqueue(packet);  // 큐에 패킷 저장
                    packetSemaphore.Release();  // 패킷 도착 시 Semaphore를 Release
                }
                ProcessBroadcastPacket(packet);
            }
        }
        catch (Exception ex)
        {
            DebugMsg($"수신 중 오류 발생: {ex.Message}");
        }
    }

    public async Task SendPacketAsync(IPacket Packet)
    {
        try
        {
            string jsonPacket = JsonHelper<IPacket>.Serialize(Packet);
            sendMessegeQueue.Enqueue(jsonPacket);
            await SendQueueAsync();
        }
        catch (System.Exception)
        {
            throw;
        }
    }
    private async Task SendQueueAsync()
    {
        if(isSending)
            return;

        isSending = true;
        string messege;
        while(sendMessegeQueue.Count > 0)
        {
            sendMessegeQueue.TryDequeue(out messege);
            await SendMessegeAsync(messege);
        }
        isSending = false;
    }

    /// <summary>
    /// 응답이 필요한 요청패킷을 전송 
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
            await SendPacketAsync(senderPacket);

            // 패킷 수신 및 응답 확인
            while (!cts.IsCancellationRequested)
            {
                // 큐가 비어 있으면 기다림
                if (rcvPacketQueue.IsEmpty)
                {
                    await packetSemaphore.WaitAsync(cts.Token); //패킷을 받을 때까지 
                    continue;
                }

                // 큐에서 패킷을 꺼냄
                if (!rcvPacketQueue.TryDequeue(out responsePacket) || responsePacket == null)
                {
                    continue;
                }

                // 응답 타입이 일치하면 루프 종료
                if (senderPacket.type == (PacketType)responsePacket.type)
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
        DebugMsg($"## responsePacket?.data: {responsePacket.data}");
        return responsePacket.data; // 응답 패킷의 데이터 반환 (응답이 없으면 null 반환)
    }
#endregion

#region 서버에서 받은 패킷 처리
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
                case PacketType.LobbyUpdate:
                    try
                    {
                        JObject jObject = (Newtonsoft.Json.Linq.JObject)packet.data;
                        LobbyData Ldata = jObject.ToObject<LobbyData>();
                        OnLobbyUpdate?.Invoke(this, Ldata);
                    }
                    catch (System.Exception e)
                    {
                        DebugWaringMsg($"In PacketType.LobbyUpdate: {e}");
                    }
                    break;
            }
        }
        catch (System.Exception e)
        {
            DebugMsg(e.Message);
            throw;
        }  
    }
#endregion

#region IRequest 구현
    public async Task<Guid> FirstJoin()
    {
        IPacket senderPacket = new IPacket(PacketType.__FirstJoin, null, Guid.Empty);
        object receivedData = await SendAndWaitPacketAsync(senderPacket);
            DebugMsg($"FirstJoin(), before Deserialize: {receivedData}");
        // Guid id = JsonHelper<Guid>.Deserialize((string)receivedData);
        //     DebugMsg($"FirstJoin(), Deserialize: {id}");
        return new Guid((string)receivedData);
    }
     public async Task<List<Lobby.LobbyData>> GetLobbyList()
    {
        IPacket senderPacket= new IPacket(PacketType.__LobbyList, null, m_user.id);
        object receivedData = await SendAndWaitPacketAsync(senderPacket); 
        return JsonHelper<List<Lobby.LobbyData>>.Deserialize((string)receivedData); 
    }
    public async Task<Lobby.LobbyData> GetLobbyData()
    {
        IPacket senderPacket= new IPacket(PacketType.__LobbyData, null, m_user.id);
        object receivedData = await SendAndWaitPacketAsync(senderPacket); 
        return JsonHelper<Lobby.LobbyData>.Deserialize((string)receivedData);
    }
    public async void StartJoin()
    {
        IPacket packet = new IPacket(PacketType._StartJoin, m_user, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void EndJoin()
    {
        IPacket packet = new IPacket(PacketType._EndJoin, null, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void UpdateUser(User user)
    {
        m_user = user;
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
        IPacket packet = new IPacket(PacketType._JoinLobby, lobbyID, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void LeaveLobby()
    {
        IPacket packet = new IPacket(PacketType._LeaveLobby, null, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void LobbyReady(bool ReadyState)
    {
        IPacket packet = new IPacket(PacketType._LobbyReady, ReadyState, m_user.id);
        await SendPacketAsync(packet);
    }
    public async void SendLobbyMessege(string messege)
    {
        IPacket packet = new IPacket(PacketType._SendLobbyMessege, messege, m_user.id);
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