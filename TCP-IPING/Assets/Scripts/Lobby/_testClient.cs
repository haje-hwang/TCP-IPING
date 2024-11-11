using System.Collections;
using System.Net.Sockets;
using Lobby;
using UnityEngine;

public class _testClient : MonoBehaviour
{
    [SerializeField] int PORT;
    public bool isTestRunning = true;
    Lobby.ClientHandler handler;
    // Start is called before the first frame update
    public void Connect2Server(Lobby.LobbyServer testLobby)
    {
        TcpClient tcpClient = new TcpClient("127.0.0.1", PORT);
        handler = new Lobby.ClientHandler(tcpClient);

        handler.SendMessage($"hi I'm {gameObject.name}. started in 127.0.0.1, {PORT}");
        isTestRunning = true;
        StartCoroutine(sendTestMsg());
    }
    void Start()
    {
        
    }

    IEnumerator sendTestMsg()
    {
        while (isTestRunning)
        {
            handler.SendMessage($"I'm {gameObject.name}, Time is:{Time.time}");
            yield return new WaitForSeconds(2.0f);
        }
    }
    public ClientHandler GetHandler()
    {
        return handler;
    }
}