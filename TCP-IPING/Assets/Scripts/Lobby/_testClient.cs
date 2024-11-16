using System.Collections;
using System.Net.Sockets;
using Lobby;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class _testClient : MonoBehaviour
{
    [SerializeField] int PORT;
    public bool isTestRunning = true;
    //
    [Space(5)]
    public TMP_Text uid;
    public TMP_Text nickName;
    //
    // RequestHandler handler;
    ClientHandler handler;

    User m_user;
    public void SetUser(User user)
    {
        m_user = user;
        uid.text = user.id.ToString();
        nickName.text = user.nickName;
    }


    public void Connect2Server(Lobby.LobbyServer testLobby)
    {
        TcpClient tcpClient = new TcpClient("127.0.0.1", PORT);
        // handler = RequestHandler.CreateAsync(m_user);
        handler = new Lobby.ClientHandler(tcpClient, testLobby);

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
            // handler.SendMessage($"I'm {gameObject.name}, Time is:{Time.time}");
            yield return new WaitForSeconds(2.0f);
        }
    }
    public ClientHandler GetHandler()
    {
        return handler;
    }
}