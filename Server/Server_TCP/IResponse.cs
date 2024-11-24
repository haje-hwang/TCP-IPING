using System.Collections.Generic;
using Server_TCP.Lobby;

public interface IResponse
{
    public void DefineUser();    
    //Lobby
    public void SendLobbyList(List<LobbyData> lobbyList);
    public void SendLobbyData(LobbyData lobbyData);
    public void Booted();
    public void LobbyMessege(string who, string message);
    //In Game
    public void GameStarted();
    public void ReceivedAnswer();
    public void LastTimer();
}
