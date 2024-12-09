using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizUIManager : MonoBehaviour
{
    public TextMeshProUGUI questionText; // ������ ǥ���� TextMeshProUGUI
    public Button[] optionButtons; // ������ ��ư��

    public Sprite defaultSprite; // �⺻ ��������Ʈ
    public Sprite correctSprite; // ���� ��������Ʈ
    public Sprite incorrectSprite; // ���� ��������Ʈ

    public TextMeshProUGUI scoreText; // ������ ǥ���� TextMeshProUGUI

    private ConnectQuiz connectQuiz; // ConnectQuiz ����
    private ConnectUser connectUser;
    private int currentQuestionIndex = 0;

    private bool isAnswered = false; // ���� ������ �亯�ߴ��� ���θ� ����
    private int score = 0; // ���� ����

    void Start()
    {
        connectQuiz = FindObjectOfType<ConnectQuiz>(); // ConnectQuiz ã��
        connectUser = FindObjectOfType<ConnectUser>();

        if (connectQuiz != null)
        {
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]); // ù ��° ���� ǥ��
        }
        else
        {
            Debug.LogError("ConnectQuiz�� ã�� �� �����ϴ�.");
        }

        UpdateScoreUI(); // ���� �� ���� UI ������Ʈ
        int playerScore = PlayerPrefs.GetInt("PlayerScore", 0);
    }

    // ������ �������� TextMeshPro�� ��ư�� ǥ���ϴ� �Լ�
    void DisplayQuestion(Question question)
    {
        isAnswered = false; // ���ο� ���� ǥ�� ��, �ٽ� �亯 �����ϵ��� ����

        // ���� ǥ��
        questionText.text = question.question;

        // �� �������� ��ư�� ǥ��
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < question.options.Count)
            {
                int optionIndex = i; // ������ ��ư�� �ش��ϴ� �ε���

                // ��ư�� �ؽ�Ʈ ����
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.options[optionIndex];

                // ��ư Ŭ�� ������ ���� (���� Ȯ��)
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionButtons[optionIndex], optionIndex, question.answer));

                // �⺻ ��������Ʈ�� ��ȣ�ۿ� ���� ���·� �ʱ�ȭ
                optionButtons[i].GetComponent<Image>().sprite = defaultSprite;
                optionButtons[i].interactable = true;
                optionButtons[i].gameObject.SetActive(true);
            }
            else
            {
                // �ɼ��� �����ϸ� ��ư �����
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // ����ڰ� ������ �亯 Ȯ��
    void OnOptionSelected(Button button, int optionIndex, int correctAnswerIndex)
    {
        if (isAnswered) return; // �̹� �亯�� ��� �ƹ� �۾��� ���� ����

        isAnswered = true; // �亯 ���¸� true�� ����

        // ���� Ȯ�� �� ��������Ʈ ����
        if (optionIndex == correctAnswerIndex)
        {
            Debug.Log("����!");
            button.GetComponent<Image>().sprite = correctSprite; // ���� ��������Ʈ�� ����
            score++; // ���� ����
            UpdateScoreUI(); // ���� UI ������Ʈ
        }
        else
        {
            Debug.Log("����!");
            button.GetComponent<Image>().sprite = incorrectSprite; // ���� ��������Ʈ�� ����
        }

        // ��� ��ư�� ��Ȱ��ȭ
        DisableAllButtons();
    }

    // ���� UI ������Ʈ �Լ�
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"score: {score}";
        }
        else
        {
            Debug.LogWarning("Score Text UI�� �������� �ʾҽ��ϴ�.");
        }
    }

    // ��� ��ư�� ��Ȱ��ȭ
    void DisableAllButtons()
    {
        foreach (var button in optionButtons)
        {
            button.interactable = false;
        }
    }

    // ���� ������ �Ѿ�� �Լ�
    public void ShowNextQuestion()
    {
        if (currentQuestionIndex < connectQuiz.quiz.questions.Count - 1)
        {
            currentQuestionIndex++;

            // ���ο� ���� ǥ��
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]);
        }
        else
        {
            Debug.Log("��� ������ �������ϴ�.");
            SaveScore();
        }
    }
    public void SaveScore()
    {
        if (connectUser != null)
        {
            string nickName = PlayerPrefs.GetString("PlayerNickname", "Guest");

            connectUser.UpdateUserScore(nickName, score); // ����� ���� ������Ʈ
            Debug.Log($"DB�� ������ ����Ǿ����ϴ�: ����� {nickName}, ���� {score}");
        }
        else
        {
            Debug.LogError("ConnectUser�� �������� �ʾҽ��ϴ�. ������ ������ �� �����ϴ�.");
        }

        PlayerPrefs.SetInt("PlayerScore", score); // ���ÿ��� ������ ����
        Debug.Log("���� ���� �����: " + score);
    }

}
