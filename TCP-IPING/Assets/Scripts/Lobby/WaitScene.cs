using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingScene : MonoBehaviour
{
    [SerializeField] private Button WaitingPlay;  // WaitingScene에서 사용할 Play 버튼
    [SerializeField] private Button WaitingHome;  // WaitingScene에서 사용할 Home 버튼
  
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
}