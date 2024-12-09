using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class QuizUIManager : MonoBehaviour
{
    public TextMeshProUGUI questionText; // 질문을 표시할 TextMeshProUGUI
    public Button[] optionButtons; // 선택지 버튼들

    public Sprite defaultSprite; // 기본 스프라이트
    public Sprite correctSprite; // 정답 스프라이트
    public Sprite incorrectSprite; // 오답 스프라이트

    public TextMeshProUGUI scoreText; // 점수를 표시할 TextMeshProUGUI

    private ConnectQuiz connectQuiz; // ConnectQuiz 참조
    private ConnectUser connectUser;
    private int currentQuestionIndex = 0;

    private bool isAnswered = false; // 현재 질문에 답변했는지 여부를 저장
    private int score = 0; // 현재 점수

    void Start()
    {
        connectQuiz = FindObjectOfType<ConnectQuiz>(); // ConnectQuiz 찾기
        connectUser = FindObjectOfType<ConnectUser>();

        if (connectQuiz != null)
        {
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]); // 첫 번째 질문 표시
        }
        else
        {
            Debug.LogError("ConnectQuiz를 찾을 수 없습니다.");
        }

        UpdateScoreUI(); // 시작 시 점수 UI 업데이트
        int playerScore = PlayerPrefs.GetInt("PlayerScore", 0);
    }

    // 질문과 선택지를 TextMeshPro와 버튼에 표시하는 함수
    void DisplayQuestion(Question question)
    {
        isAnswered = false; // 새로운 질문 표시 시, 다시 답변 가능하도록 설정

        // 질문 표시
        questionText.text = question.question;

        // 각 선택지를 버튼에 표시
        for (int i = 0; i < optionButtons.Length; i++)
        {
            if (i < question.options.Count)
            {
                int optionIndex = i; // 선택지 버튼에 해당하는 인덱스

                // 버튼의 텍스트 변경
                optionButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = question.options[optionIndex];

                // 버튼 클릭 리스너 설정 (정답 확인)
                optionButtons[i].onClick.RemoveAllListeners();
                optionButtons[i].onClick.AddListener(() => OnOptionSelected(optionButtons[optionIndex], optionIndex, question.answer));

                // 기본 스프라이트와 상호작용 가능 상태로 초기화
                optionButtons[i].GetComponent<Image>().sprite = defaultSprite;
                optionButtons[i].interactable = true;
                optionButtons[i].gameObject.SetActive(true);
            }
            else
            {
                // 옵션이 부족하면 버튼 숨기기
                optionButtons[i].gameObject.SetActive(false);
            }
        }
    }

    // 사용자가 선택한 답변 확인
    void OnOptionSelected(Button button, int optionIndex, int correctAnswerIndex)
    {
        if (isAnswered) return; // 이미 답변한 경우 아무 작업도 하지 않음

        isAnswered = true; // 답변 상태를 true로 변경

        // 정답 확인 및 스프라이트 변경
        if (optionIndex == correctAnswerIndex)
        {
            Debug.Log("정답!");
            button.GetComponent<Image>().sprite = correctSprite; // 정답 스프라이트로 변경
            score++; // 점수 증가
            UpdateScoreUI(); // 점수 UI 업데이트
        }
        else
        {
            Debug.Log("오답!");
            button.GetComponent<Image>().sprite = incorrectSprite; // 오답 스프라이트로 변경
        }

        // 모든 버튼을 비활성화
        DisableAllButtons();
    }

    // 점수 UI 업데이트 함수
    void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"score: {score}";
        }
        else
        {
            Debug.LogWarning("Score Text UI가 설정되지 않았습니다.");
        }
    }

    // 모든 버튼을 비활성화
    void DisableAllButtons()
    {
        foreach (var button in optionButtons)
        {
            button.interactable = false;
        }
    }

    // 다음 문제로 넘어가는 함수
    public void ShowNextQuestion()
    {
        if (currentQuestionIndex < connectQuiz.quiz.questions.Count - 1)
        {
            currentQuestionIndex++;

            // 새로운 질문 표시
            DisplayQuestion(connectQuiz.quiz.questions[currentQuestionIndex]);
        }
        else
        {
            Debug.Log("모든 문제가 끝났습니다.");
            SaveScore();
        }
    }
    public void SaveScore()
    {
        if (connectUser != null)
        {
            string nickName = PlayerPrefs.GetString("PlayerNickname", "Guest");

            connectUser.UpdateUserScore(nickName, score); // 사용자 점수 업데이트
            Debug.Log($"DB에 점수가 저장되었습니다: 사용자 {nickName}, 점수 {score}");
        }
        else
        {
            Debug.LogError("ConnectUser가 설정되지 않았습니다. 점수를 저장할 수 없습니다.");
        }

        PlayerPrefs.SetInt("PlayerScore", score); // 로컬에도 점수를 저장
        Debug.Log("로컬 점수 저장됨: " + score);
    }

}
