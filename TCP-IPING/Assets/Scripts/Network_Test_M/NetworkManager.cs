using UnityEngine;
using UnityEditor.Networking;

public class NetworkManager : MonoBehaviour
{
    private const string typeName = "QuizGame"; 
    private const string gameName = "RoomName"; 
    private void StartServer()
    {
        // Network.InitializeServer(4, 25000, !Network.HavePublicAddress()); 
        // MasterServer.RegisterHost(typeName, gameName);
    }
}
