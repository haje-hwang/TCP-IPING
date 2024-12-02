using Server_TCP.Quiz;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Server_TCP.Lobby
{
    public class GameLobby
    {
        public LobbyData data;
        public bool isRunning {get; private set;}

        private List<ClientHandler> clients;
        private QuizServer quizManager = new QuizServer();
        private PacketEventDispatcher eventDispatcher;

        public delegate void LobbyJoin(ClientHandler joinedClient);
        public delegate void LobbyExit(ClientHandler exitedClient);
        public delegate void LobbyDestroy(Guid lobbyId);
        public event LobbyJoin OnLobbyJoined = delegate { };
        public event LobbyExit OnLobbyExited = delegate { };
        public event LobbyDestroy OnLobbyDestroyed = delegate { };

        public GameLobby(LobbyData data, PacketEventDispatcher eventDispatcher)
        {
            this.data = data;
            this.eventDispatcher = eventDispatcher;
            isRunning = true;
            clients = new List<ClientHandler>();
        }
        ~GameLobby()
        {
            clients.Clear(); 
        }
        public bool AddPlayer(ClientHandler player, User user)
        {
            if (clients.Count < data.maxPlayers)
            {
                clients.Add(player);
                data.players.Add(player.GetUser());
                player.joindLobbyId = data.uid;

                IPacket joinedPacket = new IPacket(PacketType.LobbyUpdate, data, Guid.Empty);
                BroadcastPacket(joinedPacket);

                return true;
            }
            return false;
        }
        public bool RemovePlayer(ClientHandler player, User user)
        {
            data.players.Remove(player.GetUser());
            if(user.id == data.host)// || data.players.Count == 0)  //어차피 host나가면 방 터지니까.
            {
                //방 삭제
                OnLobbyDestroyed?.Invoke(data.uid);
                CloseLobby();
            }
            return clients.Remove(player);
        }
        private void CloseLobby()
        {
            foreach (var client in clients)
            {
                if (!client.GetUser().id.Equals(data.host))
                {
                    //로비 퇴장 패킷 전송
                    IPacket bootPacket = new IPacket(PacketType.Booted, data.uid, Guid.Empty);
                    SendPacket(bootPacket, client);
                }
            }
        }
        //
        public void BroadcastPacket(IPacket packet)
        {
            foreach (var client in clients)
            {
                SendPacket(packet, client);
            }
        }
        public void SendPacket(IPacket packet, ClientHandler client)
        {
            client.SendPacketAsync(packet);
        }
        // 기타 로비 관련 메서드들...
    }
}