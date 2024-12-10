using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Server_TCP.Lobby;

namespace Lobby
{
    public enum LobbyMode
    {
        PUBLIC,
        PRIVATE
    }
    public class GameLobby
    {
        public Server_TCP.Lobby.LobbyData data;
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