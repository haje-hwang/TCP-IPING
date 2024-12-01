using Server_TCP.Lobby;
using System;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Server_TCP.Lobby
{
    [Serializable]
    public enum LobbyMode
    {
        PUBLIC,
        PRIVATE
    }
    [Serializable]
    public class LobbyData
    {
        public Guid host { get; set; }
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
}
