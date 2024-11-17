using System.Collections;
using System.Net.Sockets;
using System;
using Lobby;
using TMPro;
using UnityEditor.PackageManager.Requests;
using UnityEngine;

public class _testClient : MonoBehaviour
{
    [SerializeField] string IP = "127.0.0.1";
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
        if (nickName != null)
            nickName.text = user.nickName;
        else
            Debug.LogWarning("nickName is not assigned.");

        if(uid != null)
        {
            string idText = user.id.ToString();
            uid.SetText(idText); 
        }
        else
            Debug.LogWarning("uid is not assigned.");


        if(user!= null)
            m_user = user;
        else
            Debug.LogWarning("user is null");
    }


    public void Connect2Server()
    {
        TcpClient tcpClient = new TcpClient(IP, PORT);
        handler = new RequestHandler(null, tcpClient);
        // handler = new Lobby.ClientHandler(tcpClient, testLobby);

        // handler.SendPacket($"hi I'm {gameObject.name}. started in 127.0.0.1, {PORT}");
        
        isTestRunning = true;
        // StartCoroutine(sendTestMsg());
    }
    // IEnumerator sendTestMsg()
    // {
    //     while (isTestRunning)
    //     {
    //         handler.SendMessage($"I'm {gameObject.name}, Time is:{Time.time}");
    //         yield return new WaitForSeconds(2.0f);
    //     }
    // }
    public RequestHandler GetHandler()
    {
        return handler;
    }
    public async void button4test()
    {
        SetUser(await handler.FirstJoin());
    }
    private void OnApplicationQuit()
    {
        handler.Disconnect();
        handler = null;
    }
}