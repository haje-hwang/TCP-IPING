using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public Button again;
    public Button Rankinghome;

    private void Start()
    {

        if (again != null)
        {
            again.onClick.AddListener(() => SceneManager.LoadScene("WaitingScene"));
        }

        if (Rankinghome != null)
        {
            Rankinghome.onClick.AddListener(() => SceneManager.LoadScene("LoginScene"));
        }
    }
    public void LoadLoginScene()
    {
        SceneManager.LoadScene("LoginScene"); // LoginScene으로 이동
    }
    public void LoadWaitingRoomScene()
    {
        SceneManager.LoadScene("WaitingScene"); // WaitingScene으로 이동
    }
    public void PlayScene()
    {
        SceneManager.LoadScene("PlayScene"); // PlayScene으로 이동
    }
}