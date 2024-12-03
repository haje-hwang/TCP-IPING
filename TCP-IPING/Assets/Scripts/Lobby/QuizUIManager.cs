using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizUIManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;  // ������ ǥ���� TextMeshProUGUI
    public Button[] optionButtons;  // ������ ��ư �迭

    private ConnectQuiz connectQuiz;  // ConnectQuiz ����
    private int currentQuestionIndex = 0;  // ���� ������ �ε���

    void Start()
    {
        connectQuiz = FindObjectOfType<ConnectQuiz>();  // ConnectQuiz ã��

        if (connectQuiz != null && connectQuiz.quiz.questions.Count > 0)
        {
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]);  // ù ��° ���� ǥ��
        }
        else
        {
            Debug.LogError("ConnectQuiz�� ã�� �� ���ų� ������ �����ϴ�.");
        }
    }

    // ������ �������� TextMeshPro�� ��ư�� ǥ���ϴ� �Լ�
    void DisplayQuestion(Question question)
    {
        questionText.text = question.question;  // ������ TextMeshPro�� ǥ��

        // �� �������� ��ư�� ǥ��
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < question.options.Count)  // ������ ������ŭ ��ư ����
            {
                int optionIndex = i;  // ������ ��ư�� �ش��ϴ� �ε���

                // ��ư�� �ؽ�Ʈ ����
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.options[i];

                // ��ư Ŭ�� ������ ���� (���� Ȯ��)
                optionButtons[i].onClick.RemoveAllListeners();  // ���� ������ ����
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionIndex, question.answer));
            }
            else
            {
                // �������� �����ϸ� ��ư �����
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // ����ڰ� ������ �亯 Ȯ��
    void OnOptionSelected(int optionIndex, int correctAnswerIndex)
    {
        if (optionIndex == correctAnswerIndex)
        {
            Debug.Log("����!");
        }
        else
        {
            Debug.Log("����!");
        }

        // ���� �������� �̵�
        currentQuestionIndex++;

        if (currentQuestionIndex < connectQuiz.quiz.questions.Count)
        {
            // ���� ������ ǥ��
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]);
        }
        else
        {
            // ��� ������ ���� ���
            Debug.Log("���� �Ϸ�!");
        }
    }
}
