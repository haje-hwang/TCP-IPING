using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class UnityTcpClient : MonoBehaviour
{
    private TcpClient client;
    private NetworkStream stream;

    [Header("Server Configuration")]
    public string serverIP = "27.113.62.74"; // 서버 IP 주소 (예: "192.168.0.3" 또는 외부 IP)
    public int serverPort = 5000;         // 서버 포트 (예: 5000)

    private void Start()
    {
        // 서버 연결 시작
        ConnectToServer();
    }

    private async void ConnectToServer()
    {
        try
        {
            Debug.Log($"[Client] Connecting to server at {serverIP}:{serverPort}...");
            client = new TcpClient();
            await client.ConnectAsync(serverIP, serverPort);
            Debug.Log("[Client] Connected to server!");

            stream = client.GetStream();
            _ = ListenForMessages(); // 서버에서 오는 메시지 수신 시작
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Client] Connection failed: {ex.Message}");
        }
    }

    private async Task ListenForMessages()
    {
        byte[] buffer = new byte[1024];

        try
        {
            while (client.Connected)
            {
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                if (bytesRead > 0)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                    Debug.Log($"[Client] Received: {message}");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Client] Error reading from server: {ex.Message}");
        }
    }

    public async void SendMessageToServer(string message)
    {
        if (client == null || !client.Connected)
        {
            Debug.LogWarning("[Client] Not connected to server!");
            return;
        }

        try
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
            Debug.Log($"[Client] Sent: {message}");
        }
        catch (Exception ex)
        {
            Debug.LogError($"[Client] Error sending message: {ex.Message}");
        }
    }

    private void OnApplicationQuit()
    {
        DisconnectFromServer();
    }

    private void DisconnectFromServer()
    {
        if (stream != null) stream.Close();
        if (client != null) client.Close();
        Debug.Log("[Client] Disconnected from server.");
    }
}
