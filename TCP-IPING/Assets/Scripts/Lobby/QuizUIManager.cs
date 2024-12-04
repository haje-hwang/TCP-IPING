using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class QuizUIManager : MonoBehaviour
{
    public TextMeshProUGUI questionText; // 질문을 표시할 TextMeshProUGUI
    public Button[] optionButtons; // 선택지 버튼들

    private ConnectQuiz connectQuiz; // ConnectQuiz 참조
    private int currentQuestionIndex = 0;

    private RoomTimer roomTimer;  // RoomTimer 참조

    void Start()
    {
        connectQuiz = FindObjectOfType<ConnectQuiz>();  // ConnectQuiz 찾기
        roomTimer = FindObjectOfType<RoomTimer>(); // RoomTimer 찾기

        if (connectQuiz != null)
        {
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]);  // 첫 번째 질문 표시
        }
        else
        {
            Debug.LogError("ConnectQuiz를 찾을 수 없습니다.");
        }
    }

    // 질문과 선택지를 TextMeshPro와 버튼에 표시하는 함수
    void DisplayQuestion(Question question)
    {
        questionText.text = question.question;  // 질문을 TextMeshPro에 표시

        // 각 선택지를 버튼에 표시
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < question.options.Count)
            {
                int optionIndex = i;  // 선택지 버튼에 해당하는 인덱스

                // 버튼의 텍스트 변경
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.options[optionIndex];

                // 버튼 클릭 리스너 설정 (정답 확인)
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionIndex, question.answer));
            }
            else
            {
                // 옵션이 부족하면 버튼 숨기기
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 사용자가 선택한 답변 확인
    void OnOptionSelected(int optionIndex, int correctAnswerIndex)
    {
        // 정답 확인
        if (optionIndex == correctAnswerIndex)
        {
            Debug.Log("정답!");
        }
        else
        {
            Debug.Log("오답!");
        }

     
    }

    // 다음 문제로 넘어가는 함수
    public void ShowNextQuestion()
    {
        if (currentQuestionIndex < connectQuiz.quiz.questions.Count - 1)
        {
            currentQuestionIndex++;
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]); // 다음 질문 표시
        }
        else
        {
            Debug.Log("모든 문제가 끝났습니다.");
            // 랭킹 화면으로 이동하는 등 필요한 처리를 여기서 할 수 있습니다.
        }
    }
}
