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
        public Guid host {get; set;}
        public Guid uid { get; private set; }    
        public string name { get; private set; }
        public int maxPlayers { get; private set; }
        public LobbyMode state { get; private set; }
        public List<User> players { get; private set; }
        public LobbyData(Guid uid, string name, int maxPlayers, Guid host, LobbyMode mode = LobbyMode.PUBLIC)
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
        // public delegate void LobbyJoin(User joinedUser);
        // public delegate void LobbyExit(User exitedUser);
        // public event LobbyJoin OnLobbyJoined;
        // public event LobbyExit OnLobbyExited;

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
        public bool RemovePlayer(ClientHandler player)
        {
            // OnLobbyExited?.Invoke(player.user);
            return clients.Remove(player);
        }
        //
        public void BroadcastPacket(IPacket packet)
        {
            foreach (var client in clients)
            {
                client.SendPacketAsync(packet);
            }
        }
        // 기타 로비 관련 메서드들...
    }
}