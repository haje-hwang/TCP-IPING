using System.Collections.Generic;

public interface IResponse
{
    public IPacket DefineUser();    
    //Lobby
    public IPacket SendLobbyList(List<Lobby.LobbyData> lobbyList);
    public IPacket SendLobbyData(Lobby.LobbyData lobbyData);
    public IPacket Booted();
    public IPacket LobbyMessege(string who, string message);
    //In Game
    public IPacket GameStarted();
    public IPacket ReceivedAnswer();
    public IPacket LastTimer();
}
