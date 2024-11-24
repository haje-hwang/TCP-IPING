using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server_TCP.Lobby
{
    public enum LobbyMode
    {
        PUBLIC,
        PRIVATE
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
        public bool AddPlayer(ClientHandler player, User user)
        {
            if (clients.Count < data.maxPlayers)
            {
                clients.Add(player);
                OnLobbyJoined?.Invoke(user);
                return true;
            }
            return false;
        }
        public bool RemovePlayer(ClientHandler player, User user)
        {
            OnLobbyExited?.Invoke(user);
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