using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Lobby;
using System.Text;

public class DisplayLobbyData : MonoBehaviour
{
    [SerializeField] TMP_Text hostID;
    [SerializeField] TMP_Text roomID;
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text MaxPlayer;
    [SerializeField] TMP_Text Players;
    public void DisplayData(LobbyData data)
    {
        hostID.text = data.host.ToString();
        roomID.text = data.uid.ToString();
        roomName.text = data.name.ToString();
        MaxPlayer.text = data.maxPlayers.ToString();
        StringBuilder sb = new StringBuilder();
        List<User> users = data.players;
        foreach(User u in users)
        {
            sb.Append(u.nickName.ToString());
            sb.Append(" / ");
            sb.AppendLine(u.id.ToString());
        }
        Players.text = sb.ToString();
    }
}
