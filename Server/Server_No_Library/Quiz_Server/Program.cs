using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;

public class QuizServer
{
    private static Dictionary<string, Room> rooms = new(); // RoomCode -> Room object
    private static Dictionary<string, string> userNicknames = new(); // ConnectionId -> Nickname

    public static void Main(string[] args)
    {
        HttpListener listener = new HttpListener();
        listener.Prefixes.Add("http://localhost:5000/");
        listener.Start();
        Console.WriteLine("Server is running on http://localhost:5000/");

        while (true)
        {
            HttpListenerContext context = listener.GetContext();
            ProcessRequest(context);
        }
    }

    private static void ProcessRequest(HttpListenerContext context)
    {
        string responseString = "";
        try
        {
            string method = context.Request.HttpMethod;
            string endpoint = context.Request.Url.AbsolutePath;

            if (method == "POST" && endpoint == "/SetNickname")
            {
                var requestData = ReadRequestBody(context.Request);
                var nickname = requestData["nickname"];
                userNicknames[context.Request.RemoteEndPoint.ToString()] = nickname;
                responseString = JsonSerializer.Serialize(new { success = true, nickname });
            }
            else if (method == "POST" && endpoint == "/CreateRoom")
            {
                var requestData = ReadRequestBody(context.Request);
                var roomCode = requestData["roomCode"];
                var connectionId = context.Request.RemoteEndPoint.ToString();
                if (!rooms.ContainsKey(roomCode))
                {
                    var room = new Room(roomCode, userNicknames[connectionId]); // 방 생성 시 방장 지정
                    rooms[roomCode] = room;
                    responseString = JsonSerializer.Serialize(new { success = true, roomCode, host = room.Host });
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { success = false, message = "Room already exists" });
                }
            }
            else if (method == "POST" && endpoint == "/StartGame")
            {
                var requestData = ReadRequestBody(context.Request);
                var roomCode = requestData["roomCode"];
                if (rooms.TryGetValue(roomCode, out var room))
                {
                    if (room.Host == userNicknames[context.Request.RemoteEndPoint.ToString()])
                    {
                        room.StartGame();
                        responseString = JsonSerializer.Serialize(new { success = true, questions = room.Questions });
                    }
                    else
                    {
                        responseString = JsonSerializer.Serialize(new { success = false, message = "Only the host can start the game" });
                    }
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { success = false, message = "Room not found" });
                }
            }
            else if (method == "POST" && endpoint == "/SubmitAnswer")
            {
                var requestData = ReadRequestBody(context.Request);
                var roomCode = requestData["roomCode"];
                var answer = requestData["answer"];
                var connectionId = context.Request.RemoteEndPoint.ToString();

                if (rooms.TryGetValue(roomCode, out var room))
                {
                    var nickname = userNicknames[connectionId];
                    bool isCorrect = room.CheckAnswer(answer);
                    if (isCorrect)
                    {
                        room.UpdateScore(nickname, 10); // 정답 시 10점 추가
                    }

                    responseString = JsonSerializer.Serialize(new
                    {
                        success = true,
                        correct = isCorrect,
                        score = room.Scores[nickname]
                    });
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { success = false, message = "Room not found" });
                }
            }
            else if (method == "POST" && endpoint == "/EndGame")
            {
                var requestData = ReadRequestBody(context.Request);
                var roomCode = requestData["roomCode"];

                if (rooms.TryGetValue(roomCode, out var room))
                {
                    var rankings = room.GetRankings();
                    rooms.Remove(roomCode); // 게임 종료 후 방 삭제
                    responseString = JsonSerializer.Serialize(new { success = true, rankings });
                }
                else
                {
                    responseString = JsonSerializer.Serialize(new { success = false, message = "Room not found" });
                }
            }
            else
            {
                responseString = JsonSerializer.Serialize(new { success = false, message = "Invalid endpoint" });
            }
        }
        catch (Exception ex)
        {
            responseString = JsonSerializer.Serialize(new { success = false, error = ex.Message });
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
