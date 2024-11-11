using UnityEngine;
using UnityEngine.UI;

public class RoomTimer : MonoBehaviour
{
    public float timerDuration = 30f; // Ÿ�̸� ���� �ð� (��)
    private float timer;
    public Text timerText; // Ÿ�̸� ǥ���� Text UI

    private bool timerActive = false;

    void Start()
    {
        // Ÿ�̸� �ʱ�ȭ
        timer = timerDuration;
        UpdateTimerText();
        StartTimer(); // Ÿ�̸� ����
    }

    void Update()
    {
        // Ÿ�̸Ӱ� Ȱ��ȭ�� ���
        if (timerActive)
        {
            if (timer > 0)
            {
                timer -= Time.deltaTime; // �� �����Ӹ��� �ð� ����
                UpdateTimerText();
            }
            else
            {
                // Ÿ�̸Ӱ� ������ ��
                timer = 0;
                timerActive = false;
                OnTimerEnd(); // Ÿ�̸� ���� �� ȣ���� �Լ�
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
        // ���� �ð��� �ؽ�Ʈ�� ǥ��
        timerText.text = Mathf.Ceil(timer).ToString() + "s";
    }

    private void OnTimerEnd()
    {
        // Ÿ�̸Ӱ� ������ �� ������ �۾�
        Debug.Log("Timer ended!");
        // ���⼭ ���ϴ� �ൿ�� �߰��ϼ���
    }
}