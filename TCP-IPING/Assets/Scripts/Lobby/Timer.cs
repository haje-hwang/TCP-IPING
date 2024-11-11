using UnityEngine;
using UnityEngine.UI;

public class RoomTimer : MonoBehaviour
{
    public float timerDuration = 30f; // 타이머 지속 시간 (초)
    private float timer;
    public Text timerText; // 타이머 표시할 Text UI

    private bool timerActive = false;

    void Start()
    {
        // 타이머 초기화
        timer = timerDuration;
        UpdateTimerText();
        StartTimer(); // 타이머 시작
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
                OnTimerEnd(); // 타이머 종료 시 호출할 함수
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
        timerText.text = Mathf.Ceil(timer).ToString() + "s";
    }

    private void OnTimerEnd()
    {
        // 타이머가 끝났을 때 수행할 작업
        Debug.Log("Timer ended!");
        // 여기서 원하는 행동을 추가하세요
    }
}