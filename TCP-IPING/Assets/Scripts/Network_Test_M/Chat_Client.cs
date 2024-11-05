using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using TMPro;

public class UdpChatClient:MonoBehaviour
{
    private static UdpClient udpClient;
    private static string serverIp = "127.0.0.1";  // 서버 IP
    private static int serverPort = 8080;          // 서버 포트
    private static int localPort = 0;           // 자동포트 할당
    private static string nickname;
    public TMP_InputField Text;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            SendMessagesAsync(Text);
        }
    }

    public static async Task Main(string[] args)
    {
        // 닉네임 설정
        Console.Write("Enter your nickname: ");
        nickname = Console.ReadLine();
        // 닉네임을 치지 않았다면 Anonymous로 설정
        if (string.IsNullOrEmpty(nickname))
        {
            nickname = "Anonymous";
        }

        // 로컬 포트로 클라이언트 바인딩
        udpClient = new UdpClient(localPort);
        Console.WriteLine($"UDP Chat Client started... Type messages to send to the server...");

        // 닉네임을 서버로 전송
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);
        byte[] nicknameBytes = Encoding.UTF8.GetBytes(nickname);
        await udpClient.SendAsync(nicknameBytes, nicknameBytes.Length, serverEndPoint);

        // 비동기로 서버로부터 메시지 수신
        Task receiveTask = ReceiveMessagesAsync();

        // 메시지 전송 루프
        //await SendMessagesAsync();
    }

    // 메시지를 비동기로 서버에 전송하는 메서드
    private static async Task SendMessagesAsync(TMP_InputField textInput)
    {
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Parse(serverIp), serverPort);

        while (true)
        {
            try
            {
                string message = textInput.text;
                if (message == "/exit")
                {
                    byte[] exitMessageBytes = Encoding.UTF8.GetBytes("/exit");
                    await udpClient.SendAsync(exitMessageBytes, exitMessageBytes.Length, serverEndPoint);
                    Console.WriteLine("Exiting the chat...");
                    udpClient.Close();
                    break;
                }

                if (!string.IsNullOrEmpty(message))
                {
                    byte[] messageBytes = Encoding.UTF8.GetBytes(message);
                    await udpClient.SendAsync(messageBytes, messageBytes.Length, serverEndPoint);
                    Console.WriteLine($"Sent to server: {message}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error sending message: " + ex.Message);
            }
        }
    }

    // 서버로부터 메시지를 비동기로 수신하는 메서드
    private static async Task ReceiveMessagesAsync()
    {
        IPEndPoint serverEndPoint = new IPEndPoint(IPAddress.Any, 0);

        while (true)
        {
            try
            {
                UdpReceiveResult receivedResult = await udpClient.ReceiveAsync();
                string receivedMessage = Encoding.UTF8.GetString(receivedResult.Buffer);
                Console.WriteLine(receivedMessage);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error receiving message: " + ex.Message);
            }
        }
    }
}
