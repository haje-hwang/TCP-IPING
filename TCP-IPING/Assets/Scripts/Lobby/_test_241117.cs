using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class _test_241117 : MonoBehaviour
{
    [SerializeField] TMP_InputField inputField;
    [SerializeField] Button sendButton;
    [SerializeField] TcpClientUnity minwoo_client;
    [SerializeField] _testClient client;
    public void test_SendMessege()
    {
        Debug.Log(inputField.text);
        minwoo_client.SendMessageToServer(inputField.text);
        // await client.GetHandler().SendMessegeAsync(inputField.text);
        inputField.text = "";
    }
}
