using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class WaitingScene : MonoBehaviour
{
    [SerializeField] private Button WaitingPlay;  // WaitingScene���� ����� Play ��ư
    [SerializeField] private Button WaitingHome;  // WaitingScene���� ����� Home ��ư
  
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