using System.Collections.Generic;
using Lobby;
using UnityEngine;
using System.Threading;

public class _testServerManager : MonoBehaviour
{
    [SerializeField] List<_testClient> testClients = new List<_testClient>();

    Lobby.LobbyServer testServer;
    // Start is called before the first frame update
    private void Awake() 
    {
        testServer = new Lobby.LobbyServer();
        Thread serverThread = new Thread(() => testServer.Start(8080));
        serverThread.Start();
        foreach(_testClient client in testClients)
        {
            client.Connect2Server(testServer);
        }


        GameLobby testLobby = testServer.CreateLobby("testLobby", 4);
        foreach(_testClient client in testClients)
        {
            if(!testLobby.AddPlayer(client.GetHandler())) 
            { 
                Debug.LogWarning("Failled to add player.");
            }
            else
            {
                Debug.Log($"{client.name} added in testLobby, UID:{testLobby.data.uid}");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.anyKeyDown)
        {
            testServer.isRunning = false;
            foreach(_testClient client in testClients)
            {
                client.isTestRunning = false;
                client.GetHandler().isRunning = false;
            }
            Debug.Log("Test End");
        }
    }
}
