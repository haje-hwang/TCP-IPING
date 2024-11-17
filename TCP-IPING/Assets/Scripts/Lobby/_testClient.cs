using System.Collections;
using System.Net.Sockets;
using System;
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
    RequestHandler handler;

    User m_user;
    public void SetUser(User user)
    {
        m_user = user;
        string idText = user.id.ToString();
        uid.SetText(idText);
        nickName.text = user.nickName;
    }


    public void Connect2Server(Lobby.LobbyServer testLobby)
    {
        SetUser(new User(Guid.Empty, "aaa"));
        TcpClient tcpClient = new TcpClient("127.0.0.1", PORT);
        handler = new RequestHandler(m_user, tcpClient);
        // handler = new Lobby.ClientHandler(tcpClient, testLobby);

        // handler.SendPacket($"hi I'm {gameObject.name}. started in 127.0.0.1, {PORT}");
        
        isTestRunning = true;
        // StartCoroutine(sendTestMsg());
    }
    IEnumerator sendTestMsg()
    {
        while (isTestRunning)
        {
            // handler.SendMessage($"I'm {gameObject.name}, Time is:{Time.time}");
            yield return new WaitForSeconds(2.0f);
        }
    }
    public RequestHandler GetHandler()
    {
        return handler;
    }
    public async void button4test()
    {
        SetUser(await handler.FirstJoin());
    }
}