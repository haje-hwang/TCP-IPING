using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Lobby
{
    public enum LobbyMode
    {
        PUBLIC,
        PRIVATE
    }
    public class LobbyData
    {
        public UID host {get; set;}
        public UID uid { get; private set; }    
        public string name { get; private set; }
        public int maxPlayers { get; private set; }
        public LobbyMode state { get; private set; }
        public List<User> players { get; private set; }
        public LobbyData(UID uid, string name, int maxPlayers, UID host, LobbyMode mode = LobbyMode.PUBLIC)
        {
            this.uid = uid;
            this.name = name;
            this.maxPlayers = maxPlayers;
            this.host = host;
            players = new List<User>();
            state = mode;
        }
    }
    public class GameLobby
    {
        public LobbyData data;
        public bool isRunning {get; private set;}

        public List<ClientHandler> clients { get; private set; }

        //Events
        public delegate void LobbyJoin(User joinedUser);
        public delegate void LobbyExit(User exitedUser);
        public event LobbyJoin OnLobbyJoined;
        public event LobbyExit OnLobbyExited;

        public GameLobby(LobbyData data)
        {
            this.data = data;
            isRunning = true;
            clients = new List<ClientHandler>();
        }
        public bool AddPlayer(ClientHandler player)
        {
            if (clients.Count < data.maxPlayers)
            {
                clients.Add(player);
                // OnLobbyJoined?.Invoke();
                return true;
            }
            return false;
        }
        public void RemovePlayer(ClientHandler player)
        {
            // OnLobbyExited?.Invoke(player.user);
            clients.Remove(player);
        }
        //
        public void BroadcastMessage(string message)
        {
            foreach (var client in clients)
            {
                client.SendMessage(message);
            }
        }
        // 기타 로비 관련 메서드들...
    }
}