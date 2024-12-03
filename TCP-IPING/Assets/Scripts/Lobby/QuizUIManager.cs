using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizUIManager : MonoBehaviour
{
    public TextMeshProUGUI questionText;  // 질문을 표시할 TextMeshProUGUI
    public Button[] optionButtons;  // 선택지 버튼 배열

    private ConnectQuiz connectQuiz;  // ConnectQuiz 참조
    private int currentQuestionIndex = 0;  // 현재 질문의 인덱스

    void Start()
    {
        connectQuiz = FindObjectOfType<ConnectQuiz>();  // ConnectQuiz 찾기

        if (connectQuiz != null && connectQuiz.quiz.questions.Count > 0)
        {
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]);  // 첫 번째 질문 표시
        }
        else
        {
            Debug.LogError("ConnectQuiz를 찾을 수 없거나 질문이 없습니다.");
        }
    }

    // 질문과 선택지를 TextMeshPro와 버튼에 표시하는 함수
    void DisplayQuestion(Question question)
    {
        questionText.text = question.question;  // 질문을 TextMeshPro에 표시

        // 각 선택지를 버튼에 표시
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < question.options.Count)  // 선택지 개수만큼 버튼 설정
            {
                int optionIndex = i;  // 선택지 버튼에 해당하는 인덱스

                // 버튼의 텍스트 변경
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.options[i];

                // 버튼 클릭 리스너 설정 (정답 확인)
                optionButtons[i].onClick.RemoveAllListeners();  // 기존 리스너 제거
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionIndex, question.answer));
            }
            else
            {
                // 선택지가 부족하면 버튼 숨기기
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 사용자가 선택한 답변 확인
    void OnOptionSelected(int optionIndex, int correctAnswerIndex)
    {
        if (optionIndex == correctAnswerIndex)
        {
            Debug.Log("정답!");
        }
        else
        {
            Debug.Log("오답!");
        }

        // 다음 질문으로 이동
        currentQuestionIndex++;

        if (currentQuestionIndex < connectQuiz.quiz.questions.Count)
        {
            // 다음 질문을 표시
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]);
        }
        else
        {
            // 모든 질문이 끝난 경우
            Debug.Log("퀴즈 완료!");
        }
    }
}
