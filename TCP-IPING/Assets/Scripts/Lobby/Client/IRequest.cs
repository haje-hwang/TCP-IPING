using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public interface IRequest
{
    public Task<Guid> FirstJoin();
    public void StartJoin();
    public void EndJoin();
    public void UpdateUser(User user);
    //Lobby
    public Task<List<Lobby.LobbyData>> GetLobbyList();
    public Task<Lobby.LobbyData> GetLobbyData();
    public void CreateLobby(string LobbyName);
    public void JoinLobby(Guid lobbyID);
    public void LeaveLobby();
    public void LobbyReady(bool ReadyState);
    public void SendLobbyMessege(string messege);
    //InGame
    public void SendAnswer(int answer);
}
