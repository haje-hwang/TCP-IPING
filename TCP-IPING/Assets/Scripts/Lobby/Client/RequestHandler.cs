using System;
using System.IO;
using System.Net.Sockets;
using System.Threading.Tasks;

public class RequestHandler
{
    private bool isRunning;
    private string serverAddress = "127.0.0.1"; // 서버 IP 주소 (로컬호스트)
    private int port = 8080; // 서버 포트 번호
    private StreamReader? _reader;
    private StreamWriter? _writer;

    // private 생성자로 외부에서 직접 호출하지 못하도록 제한
    private RequestHandler() { }

    // 비동기 초기화를 위한 팩토리 메서드
    public static async Task<RequestHandler> CreateAsync()
    {
        var handler = new RequestHandler();
        await handler.Start();
        return handler;
    }

    private async Task Start()
    {
        Console.WriteLine("클라이언트 시작. 종료하려면 'exit' 입력.");
        isRunning = true;

        try
        {
            using (TcpClient client = new TcpClient())
            {
                // 서버에 연결
                await client.ConnectAsync(serverAddress, port);
                Console.WriteLine($"서버에 연결됨: {serverAddress}:{port}");

                using (NetworkStream stream = client.GetStream())
                {
                    _reader = new StreamReader(stream, Constants.Packet.encoding);
                    _writer = new StreamWriter(stream, Constants.Packet.encoding) { AutoFlush = true };

                    // 수신 작업 실행
                    await ReceiveMessagesAsync();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"오류 발생: {ex.Message}");
        }

        Console.WriteLine("클라이언트 종료.");
    }

    /// <summary>
    /// 서버로 메시지를 전송하는 함수
    /// </summary>
    private async Task SendMessagesAsync(string message)
    {
        if (_writer != null)
        {
            await _writer.WriteLineAsync(message); // 서버로 메시지 전송
        }
    }

    /// <summary>
    /// 서버로부터 메시지를 수신하는 함수
    /// </summary>
    private async Task ReceiveMessagesAsync()
    {
        while (isRunning)
        {
            try
            {
                if (_reader != null)
                {
                    string? response = await _reader.ReadLineAsync(); // 서버로부터 메시지 수신
                    IPacket packet = PacketHelper.Deserialize(response);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"수신 중 오류 발생: {ex.Message}");
                isRunning = false; // 오류 발생 시 루프 종료
                break;
            }
        }
    }

        /// <summary>
        /// 받은 string을 패킷으로 Deserialize
        /// </summary>
        /// <param name="jsonPacket"></param>
        private void ProcessPacket(IPacket packet)
        {
            try
            {
                
            }
            catch (System.Exception)
            {
                
                throw;
            }  
        }
    
}
