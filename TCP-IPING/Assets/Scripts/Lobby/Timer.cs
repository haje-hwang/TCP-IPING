using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomTimer : MonoBehaviour
{
    public float timerDuration = 10f; // 타이머 지속 시간 (초)
    private float timer;
    public Text timerText; // 타이머 표시할 Text UI

    private bool timerActive = false;
    private int timerCount = 0;

    public GameObject questionPanel; // 질문 패널
    public GameObject[] answerButtons; // 답변 버튼들
    public GameObject rankingPanel; //랭킹 패널

    void Start()
    {
        // 타이머 초기화
        timer = timerDuration;
        UpdateTimerText();
        StartTimer(); // 타이머 시작

        rankingPanel.SetActive(false);
    }

    void Update()
    {
        // 타이머가 활성화된 경우
        if (timerActive)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime; // 매 프레임마다 시간 감소
                UpdateTimerText();
            }
            else
            {
                // 타이머가 끝났을 때
                timer = 0;
                timerActive = false;
                StartCoroutine(HandleTimerEnd()); // 타이머 종료 시 호출할 함수
            }
        }
    }

    public void StartTimer()
    {
        timerActive = true;
    }

    public void ResetTimer()
    {
        timer = timerDuration;
        timerActive = true;
        UpdateTimerText();
    }

    private void UpdateTimerText()
    {
        // 남은 시간을 텍스트에 표시
        timerText.text = Mathf.Ceil(timer).ToString();
    }

    private IEnumerator HandleTimerEnd()
    {
        Debug.Log("Timer ended!");

        // 질문 패널과 버튼들을 숨김
        questionPanel.SetActive(false);
        foreach (var button in answerButtons)
        {
            button.SetActive(false);
        }

        // 1초 대기
        yield return new WaitForSeconds(1f);

        // 질문 패널과 버튼들을 다시 활성화
        questionPanel.SetActive(true);
        foreach (var button in answerButtons)
        {
            button.SetActive(true);
        }

        // 타이머 재설정 및 시작
        ResetTimer();

        timerCount++;
        if (timerCount >= 5)
        {
            rankingPanel.SetActive(true);  // 랭킹 패널을 보이게 함
        }
    }
}