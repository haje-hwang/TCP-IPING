using UnityEngine;
using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TcpClientUnity : MonoBehaviour
{
    public string serverAddress = "27.113.62.74"; // 서버 주소
    public int port = 5000; // 서버 포트
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;
    private bool isConnected = false;

    void Start()
    {
        ConnectToServer();
    }

    void Update()
    {
        // Escape 키로 종료
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Disconnect();
        }
    }

    void OnApplicationQuit()
    {
        Disconnect();
    }

    private void ConnectToServer()
    {
        try
        {
            client = new TcpClient();
            client.Connect(serverAddress, port);
            stream = client.GetStream();
            isConnected = true;

            Debug.Log("[Client] Connected to the server.");

            // 수신 스레드 시작
            receiveThread = new Thread(ReceiveData);
            receiveThread.IsBackground = true;
            receiveThread.Start();
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Client] Connection error: {ex.Message}");
        }
    }

    private void ReceiveData()
    {
        try
        {
            byte[] buffer = new byte[1024];
            while (isConnected)
            {
                if (stream.CanRead)
                {
                    int bytesRead = stream.Read(buffer, 0, buffer.Length);
                    if (bytesRead > 0)
                    {
                        string receivedMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                        Debug.Log($"[Client] Received: {receivedMessage}");
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Client] Receive error: {ex.Message}");
        }
    }

    public void SendMessageToServer(string message)
    {
        if (!isConnected)
        {
            Debug.LogWarning("[Client] Not connected to the server.");
            return;
        }

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            stream.Write(data, 0, data.Length);
            Debug.Log($"[Client] Sent: {message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Client] Send error: {ex.Message}");
        }
    }

    private void Disconnect()
    {
        isConnected = false;

        if (receiveThread != null && receiveThread.IsAlive)
        {
            receiveThread.Abort();
        }

        if (stream != null)
        {
            stream.Close();
        }

        if (client != null)
        {
            client.Close();
        }

        Debug.Log("[Client] Disconnected from the server.");
    }

    // Example UI interaction
    public void OnSendMessageButton(string message)
    {
        SendMessageToServer(message);
    }
}
