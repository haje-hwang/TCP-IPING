using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;

public class Program
{
    static void Main()
    {
        const int port = 5000;
        LobbyServer lobbyServer = new LobbyServer();

        try
        {
            Console.WriteLine($"[Server] Starting on port {port}...");
            lobbyServer.Start(port).Wait(); // 서버 시작 (비동기 실행)
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Error: {ex.Message}");
        }
    }
}

public class LobbyServer
{
    public bool isRunning = true;
    private TcpListener listener;
    private ConcurrentDictionary<Guid, ClientHandler> clientMap = new();
    private List<ClientHandler> clients = new List<ClientHandler>();
    private List<GameLobby> lobbies = new List<GameLobby>();

    public async Task Start(int port)
    {
        listener = new TcpListener(IPAddress.Any, port);
        listener.Start();
        Console.WriteLine($"[Server] Listening on port {port}...");

        while (isRunning)
        {
            try
            {
                // 비동기 방식으로 클라이언트 연결을 기다림
                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("[Server] Client connected!");

                ClientHandler handler = new ClientHandler(client, this);
                clients.Add(handler);
                _ = handler.Start(); // 클라이언트 처리 시작
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Server] Client error: {ex.Message}");
            }
        }
    }

    public void BroadcastMessage(string message)
    {
        foreach (var client in clients)
        {
            client.SendMessage(message);
        }
    }

    public GameLobby CreateLobby(string name, int maxPlayers, Guid host)
    {
        Guid lobbyUid = Guid.NewGuid();
        LobbyData data = new LobbyData(lobbyUid, name, maxPlayers, host);
        GameLobby lobby = new GameLobby(data);
        lobbies.Add(lobby);
        return lobby;
    }

    private GameLobby FindLobby(Guid uid)
    {
        return lobbies.FirstOrDefault(lobby => lobby.Data.Uid == uid);
    }

    public bool JoinLobby(Guid lobbyCode, ClientHandler client)
    {
        GameLobby lobby = FindLobby(lobbyCode);
        return lobby?.AddPlayer(client) ?? false;
    }

    public bool LeaveLobby(Guid lobbyCode, ClientHandler client)
    {
        GameLobby lobby = FindLobby(lobbyCode);
        return lobby?.RemovePlayer(client) ?? false;
    }
}

public class ClientHandler
{
    private TcpClient client;
    private NetworkStream stream;
    private LobbyServer server;
    private Guid clientId;

    public ClientHandler(TcpClient client, LobbyServer server)
    {
        this.client = client;
        this.server = server;
        this.clientId = Guid.NewGuid();
    }

    public async Task Start()
    {
        stream = client.GetStream();
        byte[] buffer = new byte[1024];
        int bytesRead;

        Console.WriteLine($"[Server] Client {clientId} connected.");

        try
        {
            while ((bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[Server] Received from {clientId}:{message}");

                // 처리 로직 예시: Echo 또는 Broadcast
                if (message.StartsWith("LOBBY:"))
                {
                    // 간단한 로비 예시 명령 처리
                    string[] parts = message.Split(':');
                    if (parts.Length > 1)
                    {
                        string action = parts[1];
                        if (action == "CREATE")
                        {
                            var lobby = server.CreateLobby("TestLobby", 4, clientId);
                            SendMessage($"Lobby created with ID: {lobby.Data.Uid}");
                        }
                    }
                }
                else
                {
                    server.BroadcastMessage($"[Broadcast] {clientId}: {message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Client {clientId} error: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine($"[Server] Client {clientId} disconnected.");
        }
    }

    public void SendMessage(string message)
    {
        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Error sending to {clientId}: {ex.Message}");
        }
    }
}

public class GameLobby
{
    public LobbyData Data { get; private set; }
    private List<ClientHandler> players = new List<ClientHandler>();

    public GameLobby(LobbyData data)
    {
        Data = data;
    }

    public bool AddPlayer(ClientHandler client)
    {
        if (players.Count >= Data.MaxPlayers)
            return false;

        players.Add(client);
        return true;
    }

    public bool RemovePlayer(ClientHandler client)
    {
        return players.Remove(client);
    }
}

public class LobbyData
{
    public Guid Uid { get; private set; }
    public string Name { get; private set; }
    public int MaxPlayers { get; private set; }
    public Guid Host { get; private set; }

    public LobbyData(Guid uid, string name, int maxPlayers, Guid host)
    {
        Uid = uid;
        Name = name;
        MaxPlayers = maxPlayers;
        Host = host;
    }
}
