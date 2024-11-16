using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

class Program
{
    static void Main()
    {
        const int port = 5000;
        TcpListener server = new TcpListener(IPAddress.Any, port);
        
        try
        {
            server.Start();
            Console.WriteLine($"[Server] Listening on port {port}...");
            
            while (true)
            {
                Console.WriteLine("[Server] Waiting for a connection...");
                TcpClient client = server.AcceptTcpClient();
                Console.WriteLine("[Server] Client connected!");
                
                // Start a new thread to handle the client
                Thread clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Error: {ex.Message}");
        }
        finally
        {
            server.Stop();
        }
    }

    static void HandleClient(object obj)
    {
        TcpClient client = (TcpClient)obj;

        try
        {
            NetworkStream stream = client.GetStream();
            byte[] buffer = new byte[1024];
            int bytesRead;

            while ((bytesRead = stream.Read(buffer, 0, buffer.Length)) != 0)
            {
                string request = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                Console.WriteLine($"[Server] Received: {request}");

                // Echo back the received message
                string response = $"Echo: {request}";
                byte[] responseBytes = Encoding.UTF8.GetBytes(response);
                stream.Write(responseBytes, 0, responseBytes.Length);
                Console.WriteLine($"[Server] Sent: {response}");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Server] Client error: {ex.Message}");
        }
        finally
        {
            client.Close();
            Console.WriteLine("[Server] Client disconnected.");
        }
    }
}
