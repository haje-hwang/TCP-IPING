
public interface IResponse
{
    public IPacket DefineUser();
    public IPacket SendLobbyData(Lobby.GameLobby lobby);
    public IPacket ReceivedAnswer();
    public IPacket LeaveLobby();
}
