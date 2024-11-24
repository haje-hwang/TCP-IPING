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
            UnityEngine.Debug.LogError("ConnectQuiz를 찾을 수 없습니다.");
        }
        else
        {
            UnityEngine.Debug.Log("ConnectQuiz를 성공적으로 참조했습니다.");
        }
    }

    public void ChangeCategory(int category)
    {
        if (quizData != null)
        {
            quizData.ChangeCategory(category);
            UnityEngine.Debug.Log($"카테고리를 {category}로 변경했습니다.");
        }
        else
        {
            UnityEngine.Debug.LogError("ConnectQuiz를 찾을 수 없어 카테고리를 변경할 수 없습니다.");
        }
    }
}
