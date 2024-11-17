using System.Collections.Generic;
using Lobby;
using UnityEngine;
using System.Threading;
using System.Linq;

public class _testServerManager : MonoBehaviour
{
    Lobby.LobbyServer testServer;
    // Start is called before the first frame update
    private void Awake() 
    {
        testServer = new Lobby.LobbyServer();
        Thread serverThread = new Thread(() => testServer.Start(8080));
        serverThread.Start();

        // GameLobby testLobby = testServer.CreateLobby("testLobby", 4, UID.Empty());
        // foreach(_testClient client in testClients)
        // {
        //     if(!testLobby.AddPlayer(client.GetHandler())) 
        //     { 
        //         Debug.LogWarning("Failled to add player.");
        //     }
        //     else
        //     {
        //         Debug.Log($"{client.name} added in testLobby, UID:{testLobby.data.uid}");
        //     }
        // }
    }
    public Lobby.LobbyServer GetLobbyServer()
    {
        return testServer;
    }
}
