using System.Collections.Generic;

public interface IResponse
{
    public void DefineUser();    
    //Lobby
    public void SendLobbyList(List<Lobby.LobbyData> lobbyList);
    public void SendLobbyData(Lobby.LobbyData lobbyData);
    public void Booted();
    public void LobbyMessege(string who, string message);
    //In Game
    public void GameStarted();
    public void ReceivedAnswer();
    public void LastTimer();
}
