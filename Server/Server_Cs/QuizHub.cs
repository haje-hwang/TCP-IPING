using Microsoft.AspNetCore.SignalR;
using System.Collections.Concurrent;
using System.Threading.Tasks;

public class QuizHub : Hub
{
    private static ConcurrentDictionary<string, Room> Rooms = new();
    private static ConcurrentDictionary<string, string> UserNicknames = new(); // ConnectionId와 닉네임 매핑

    // 닉네임 설정
    public async Task SetNickname(string nickname)
    {
        UserNicknames[Context.ConnectionId] = nickname;
        await Clients.Caller.SendAsync("NicknameSet", nickname);
    }

    public async Task CreateRoom(string roomCode)
    {
        if (!Rooms.ContainsKey(roomCode))
        {
            Rooms[roomCode] = new Room(roomCode);
            await Clients.Caller.SendAsync("RoomCreated", roomCode);
        }
        else
        {
            await Clients.Caller.SendAsync("Error", "Room already exists");
        }
    }

    public async Task JoinRoom(string roomCode, string username)
    {
        if (Rooms.TryGetValue(roomCode, out var room))
        {
            room.Players.Add(username);
            await Groups.AddToGroupAsync(Context.ConnectionId, roomCode);
            await Clients.Group(roomCode).SendAsync("PlayerJoined", username);
        }
        else
        {
            await Clients.Caller.SendAsync("Error", "Room not found");
        }
    }

    public async Task SubmitAnswer(string roomCode, string username, string answer)
    {
        if (Rooms.TryGetValue(roomCode, out var room))
        {
            bool isCorrect = answer == "42"; // Replace with actual logic
            if (isCorrect)
            {
                room.Scores[username] += 10;
                await Clients.Group(roomCode).SendAsync("ScoreUpdated", room.Scores);
            }
        }
    }
}

public class Room
{
    public string RoomCode { get; }
    public List<string> Players { get; } = new();
    public Dictionary<string, int> Scores { get; } = new();

    public Room(string roomCode)
    {
        RoomCode = roomCode;
    }
}
