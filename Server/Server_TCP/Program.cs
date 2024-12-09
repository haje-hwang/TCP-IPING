using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Server_TCP;
using Server_TCP.Lobby;

public class Program
{
    static void Main()
    {
        const int port = 5000;
        LobbyServer lobbyServer = new LobbyServer(port);
        try
        {
            Debug.Log($"Starting on port {port}...");
            lobbyServer.Start().Wait(); // 서버 시작 (비동기 실행)
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Error: {ex.Message}");
        }
        Debug.Log($"Server Closed");
        lobbyServer.CloseServer();
    }
}