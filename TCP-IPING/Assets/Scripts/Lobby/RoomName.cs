using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class RoomName : MonoBehaviour
{
    public TMP_Text roomNameText;
    // Start is called before the first frame update
    void Start()
    {
        string roomName = PlayerPrefs.GetString("RoomName", "None");
        roomNameText.text = "¹æ ¹øÈ£: " + roomName;
    }

 
}
