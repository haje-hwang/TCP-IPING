using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingScene : MonoBehaviour
{
    [SerializeField] private Button WaitingPlay;  // WaitingScene���� Play ��ư
    [SerializeField] private Button WaitingHome;  // WaitingScene���� Home ��ư
    private RequestHandler requestHandler;
    private UserManager UserManager;

    private void Start()
    {
        if (WaitingPlay != null)
        {
            WaitingPlay.onClick.AddListener(() => SceneManager.LoadScene("PlayScene"));
        }

        if (WaitingHome != null)
        {
            WaitingHome.onClick.AddListener(() => SceneManager.LoadScene("LoginScene"));
        }
    }
    public void GameStart()
    {
        if (requestHandler != null)
        {
            // ������ ���� ���� ��û
            requestHandler.GameStart(UserManager.roomName);

            Debug.Log("Game start request sent to server.");

            // �߰����� ���� ���� ���� (��: �� ��ȯ)
            //UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning("RequestHandler is not initialized!");
        }
    }
}
