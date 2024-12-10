using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingScene : MonoBehaviour
{
    [SerializeField] private Button WaitingPlay;  // WaitingScene에서 Play 버튼
    [SerializeField] private Button WaitingHome;  // WaitingScene에서 Home 버튼
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
            // 서버에 게임 시작 요청
            requestHandler.GameStart(UserManager.roomName);

            Debug.Log("Game start request sent to server.");

            // 추가적인 게임 시작 로직 (예: 씬 전환)
            //UnityEngine.SceneManagement.SceneManager.LoadScene("GameScene");
        }
        else
        {
            Debug.LogWarning("RequestHandler is not initialized!");
        }
    }
}
