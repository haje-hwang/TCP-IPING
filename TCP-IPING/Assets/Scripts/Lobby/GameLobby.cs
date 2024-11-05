using System;
using System.Collections.Generic;

namespace Lobby
{
    public enum LobbyMode
    {
        PUBLIC,
        PRIVATE
    }
    public class GameLobby
    {
        public System.Guid uuid { get; private set; }    
        public string Name { get; private set; }
        public int MaxPlayers { get; private set; }
        public List<ClientHandler> Players { get; private set; }
        public LobbyMode State { get; private set; }
        public bool isGameStarted { get; private set; }

        public GameLobby(string name, int maxPlayers)
        {
            uuid = Guid.NewGuid();
            Name = name;
            MaxPlayers = maxPlayers;
            Players = new List<ClientHandler>();

            State = LobbyMode.PUBLIC;
            isGameStarted = false;
        }

        public bool AddPlayer(ClientHandler player)
        {
            if (Players.Count < MaxPlayers)
            {
                Players.Add(player);
                return true;
            }
            return false;
        }

        public void RemovePlayer(ClientHandler player)
        {
            Players.Remove(player);
        }

        // 기타 로비 관련 메서드들...
    }
}