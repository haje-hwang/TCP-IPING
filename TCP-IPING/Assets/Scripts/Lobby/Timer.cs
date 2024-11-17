using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class RoomTimer : MonoBehaviour
{
    public float timerDuration = 10f; // Ÿ�̸� ���� �ð� (��)
    private float timer;
    public Text timerText; // Ÿ�̸� ǥ���� Text UI

    private bool timerActive = false;
    private int timerCount = 0;

    public GameObject questionPanel; // ���� �г�
    public GameObject[] answerButtons; // �亯 ��ư��
    public GameObject rankingPanel; //��ŷ �г�

    void Start()
    {
        // Ÿ�̸� �ʱ�ȭ
        timer = timerDuration;
        UpdateTimerText();
        StartTimer(); // Ÿ�̸� ����

        rankingPanel.SetActive(false);
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
                StartCoroutine(HandleTimerEnd()); // Ÿ�̸� ���� �� ȣ���� �Լ�
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
        timerText.text = Mathf.Ceil(timer).ToString();
    }

    private IEnumerator HandleTimerEnd()
    {
        Debug.Log("Timer ended!");

        // ���� �гΰ� ��ư���� ����
        questionPanel.SetActive(false);
        foreach (var button in answerButtons)
        {
            button.SetActive(false);
        }

        // 1�� ���
        yield return new WaitForSeconds(1f);

        // ���� �гΰ� ��ư���� �ٽ� Ȱ��ȭ
        questionPanel.SetActive(true);
        foreach (var button in answerButtons)
        {
            button.SetActive(true);
        }

        // Ÿ�̸� �缳�� �� ����
        ResetTimer();

        timerCount++;
        if (timerCount >= 5)
        {
            rankingPanel.SetActive(true);  // ��ŷ �г��� ���̰� ��
        }
    }
}