using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class QuizUIManager : MonoBehaviour
{
    public TextMeshProUGUI questionText; // ������ ǥ���� TextMeshProUGUI
    public Button[] optionButtons; // ������ ��ư��

    private ConnectQuiz connectQuiz; // ConnectQuiz ����
    private int currentQuestionIndex = 0;

    private RoomTimer roomTimer;  // RoomTimer ����

    void Start()
    {
        connectQuiz = FindObjectOfType<ConnectQuiz>();  // ConnectQuiz ã��
        roomTimer = FindObjectOfType<RoomTimer>(); // RoomTimer ã��

        if (connectQuiz != null)
        {
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]);  // ù ��° ���� ǥ��
        }
        else
        {
            Debug.LogError("ConnectQuiz�� ã�� �� �����ϴ�.");
        }
    }

    // ������ �������� TextMeshPro�� ��ư�� ǥ���ϴ� �Լ�
    void DisplayQuestion(Question question)
    {
        questionText.text = question.question;  // ������ TextMeshPro�� ǥ��

        // �� �������� ��ư�� ǥ��
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < question.options.Count)
            {
                int optionIndex = i;  // ������ ��ư�� �ش��ϴ� �ε���

                // ��ư�� �ؽ�Ʈ ����
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.options[optionIndex];

                // ��ư Ŭ�� ������ ���� (���� Ȯ��)
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionIndex, question.answer));
            }
            else
            {
                // �ɼ��� �����ϸ� ��ư �����
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // ����ڰ� ������ �亯 Ȯ��
    void OnOptionSelected(int optionIndex, int correctAnswerIndex)
    {
        // ���� Ȯ��
        if (optionIndex == correctAnswerIndex)
        {
            Debug.Log("����!");
        }
        else
        {
            Debug.Log("����!");
        }

     
    }

    // ���� ������ �Ѿ�� �Լ�
    public void ShowNextQuestion()
    {
        if (currentQuestionIndex < connectQuiz.quiz.questions.Count - 1)
        {
            currentQuestionIndex++;
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]); // ���� ���� ǥ��
        }
        else
        {
            Debug.Log("��� ������ �������ϴ�.");
            // ��ŷ ȭ������ �̵��ϴ� �� �ʿ��� ó���� ���⼭ �� �� �ֽ��ϴ�.
        }
    }
}
