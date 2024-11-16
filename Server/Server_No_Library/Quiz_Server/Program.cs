using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

public class QuizServer
{
    private static Dictionary<string, Room> rooms = new();
    private static Dictionary<string, string> userNicknames = new();

    public static void Main(string[] args)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://192.168.0.3/");
        listener.Start();
        Console.WriteLine("Server started on http://192.168.0.3/");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            ProcessRequest(context);
        }
    }

    private static void ProcessRequest(HttpListenerContext context)
    {
        string responseString = "";
        string clientIp = context.Request.RemoteEndPoint.ToString();

        try
        {
            string method = context.Request.HttpMethod;
            string endpoint = context.Request.Url.AbsolutePath;

            Console.WriteLine($"[{DateTime.Now}] {method} request from {clientIp} to {endpoint}");

            if (method == "POST" && endpoint == "/SetNickname")
            {
                var requestData = ReadRequestBody(context.Request);
                var nickname = requestData["nickname"];
                userNicknames[clientIp] = nickname;
                responseString = JsonSerializer.Serialize(new { success = true, nickname });

                Console.WriteLine($"Set nickname: {nickname} for {clientIp}");
            }
            else if (method == "POST" && endpoint == "/CreateRoom")
            {
                var requestData = ReadRequestBody(context.Request);
                var roomCode = requestData["roomCode"];
                if (!rooms.ContainsKey(roomCode))
                {
                    rooms[roomCode] = new Room(roomCode, userNicknames[clientIp]);
                    responseString = JsonSerializer.Serialize(new { success = true, roomCode, host = userNicknames[clientIp] });

                    Console.WriteLine($"Room created: {roomCode} by host {userNicknames[clientIp]}");
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { success = false, message = "Room already exists" });
                    Console.WriteLine($"Failed to create room: {roomCode} (already exists)");
                }
            }
            else if (method == "POST" && endpoint == "/StartGame")
            {
                var requestData = ReadRequestBody(context.Request);
                var roomCode = requestData["roomCode"];
                if (rooms.TryGetValue(roomCode, out var room))
                {
                    if (room.Host == userNicknames[clientIp])
                    {
                        room.StartGame();
                        responseString = JsonSerializer.Serialize(new { success = true, questions = room.Questions });

                        Console.WriteLine($"Game started in room {roomCode} by host {room.Host}");
                    }
                    else
                    {
                        responseString = JsonSerializer.Serialize(new { success = false, message = "Only the host can start the game" });
                        Console.WriteLine($"Unauthorized game start attempt by {userNicknames[clientIp]}");
                    }
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { success = false, message = "Room not found" });
                    Console.WriteLine($"Failed to start game: Room {roomCode} not found");
                }
            }
            else if (method == "POST" && endpoint == "/SubmitAnswer")
            {
                var requestData = ReadRequestBody(context.Request);
                var roomCode = requestData["roomCode"];
                var answer = requestData["answer"];

                if (rooms.TryGetValue(roomCode, out var room))
                {
                    var nickname = userNicknames[clientIp];
                    bool isCorrect = room.CheckAnswer(answer);
                    if (isCorrect)
                    {
                        room.UpdateScore(nickname, 10);
                        Console.WriteLine($"{nickname} in room {roomCode} answered correctly! New score: {room.Scores[nickname]}");
                    }
                    else
                    {
                        Console.WriteLine($"{nickname} in room {roomCode} answered incorrectly");
                    }

                    responseString = JsonSerializer.Serialize(new { success = true, correct = isCorrect, score = room.Scores[nickname] });
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { success = false, message = "Room not found" });
                    Console.WriteLine($"Failed to submit answer: Room {roomCode} not found");
                }
            }
            else if (method == "POST" && endpoint == "/EndGame")
            {
                var requestData = ReadRequestBody(context.Request);
                var roomCode = requestData["roomCode"];

                if (rooms.TryGetValue(roomCode, out var room))
                {
                    var rankings = room.GetRankings();
                    rooms.Remove(roomCode);

                    Console.WriteLine($"Game ended in room {roomCode}. Rankings: {string.Join(", ", rankings.Select(r => $"{r.Key} ({r.Value} points)"))}");

                    responseString = JsonSerializer.Serialize(new { success = true, rankings });
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { success = false, message = "Room not found" });
                    Console.WriteLine($"Failed to end game: Room {roomCode} not found");
                }
            }
            else
            {
                responseString = JsonSerializer.Serialize(new { success = false, message = "Invalid endpoint" });
                Console.WriteLine($"Invalid endpoint: {endpoint}");
            }
        }
        catch (Exception ex)
        {
            responseString = JsonSerializer.Serialize(new { success = false, error = ex.Message });
            Console.WriteLine($"Error processing request: {ex.Message}");
        }

        byte[] buffer = Encoding.UTF8.GetBytes(responseString);
        context.Response.ContentType = "application/json";
        context.Response.ContentLength64 = buffer.Length;
        context.Response.OutputStream.Write(buffer, 0, buffer.Length);
        context.Response.Close();
    }

    private static Dictionary<string, string> ReadRequestBody(HttpListenerRequest request)
    {
        using (StreamReader reader = new StreamReader(request.InputStream, Encoding.UTF8))
        {
            string body = reader.ReadToEnd();
            return JsonSerializer.Deserialize<Dictionary<string, string>>(body);
        }
    }
}


public class Room
{
    public string RoomCode { get; }
    public string Host { get; } // 방장
    public List<string> Players { get; } = new();
    public Dictionary<string, int> Scores { get; } = new(); // 닉네임 -> 점수
    public List<Question> Questions { get; } = new(); // 문제 목록
    private int currentQuestionIndex = 0;

    public Room(string roomCode, string host)
    {
        RoomCode = roomCode;
        Host = host;
        LoadQuestions();
    }

    public void StartGame()
    {
        currentQuestionIndex = 0;
    }

    public bool CheckAnswer(string answer)
    {
        if (currentQuestionIndex >= Questions.Count) return false;
        return Questions[currentQuestionIndex++].Answer == answer;
    }

    public void UpdateScore(string nickname, int points)
    {
        if (!Scores.ContainsKey(nickname))
            Scores[nickname] = 0;

        Scores[nickname] += points;
    }

    public Dictionary<string, int> GetRankings()
    {
        return Scores.OrderByDescending(s => s.Value)
                     .ToDictionary(s => s.Key, s => s.Value);
    }

    private void LoadQuestions()
    {
        Questions.Add(new Question("What is 2+2?", "4"));
        Questions.Add(new Question("What is the capital of France?", "Paris"));
        Questions.Add(new Question("What is the answer to life, the universe, and everything?", "42"));
    }
}

public class Question
{
    public string Text { get; }
    public string Answer { get; }

    public Question(string text, string answer)
    {
        Text = text;
        Answer = answer;
    }
}
