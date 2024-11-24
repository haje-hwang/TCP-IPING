using UnityEngine;
using UnityEngine.SceneManagement;

public class ButtonHandler : MonoBehaviour
{
    private ConnectQuiz quizData;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        quizData = FindObjectOfType<ConnectQuiz>();
        if (quizData == null)
        {
            UnityEngine.Debug.LogError("ConnectQuiz�� ã�� �� �����ϴ�.");
        }
        else
        {
            UnityEngine.Debug.Log("ConnectQuiz�� ���������� �����߽��ϴ�.");
        }
    }

    public void ChangeCategory(int category)
    {
        if (quizData != null)
        {
            quizData.ChangeCategory(category);
            UnityEngine.Debug.Log($"ī�װ��� {category}�� �����߽��ϴ�.");
        }
        else
        {
            UnityEngine.Debug.LogError("ConnectQuiz�� ã�� �� ���� ī�װ��� ������ �� �����ϴ�.");
        }
    }
}
