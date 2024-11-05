using System.Collections;
using System.Net.Sockets;
using Lobby;
using UnityEngine;

public class _testClient : MonoBehaviour
{
    Lobby.ClientHandler handler;
    // Start is called before the first frame update
    public void Connect2Server(Lobby.LobbyServer testLobby)
    {
        TcpClient tcpClient = new TcpClient("127.0.0.1", 8080);
        handler = new Lobby.ClientHandler(tcpClient, testLobby);
    }
    void Start()
    {
        handler.SendMessage($"hi I'm {gameObject.name}. started in 127.0.0.1, 8080");
        StartCoroutine(sendTestMsg());
    }

    IEnumerator sendTestMsg()
    {
        handler.SendMessage($"hi I'm {gameObject.name}, {Time.time}");
        yield return new WaitForSeconds(1.0f);
    }
    public ClientHandler GetHandler()
    {
        return handler;
    }
}