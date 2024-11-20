using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicToggleButton : MonoBehaviour
{
    public Button musicButton;          // UI ��ư
    public Sprite playSprite;           // ���� ��� ��ư �̹���
    public Sprite pauseSprite;          // ���� ���� ��ư �̹���
    public AudioSource audioSource;     // ��� ���� Audio Source

    private bool isPlaying = true;      // ���� ���� (true: ���, false: ����)

    void Start()
    {
        // ��ư Ŭ�� �̺�Ʈ�� �Լ� ���
        musicButton.onClick.AddListener(ToggleMusic);

        // �ʱ� ��ư �̹����� ���� (������ ��� ���̶�� ����)
        musicButton.image.sprite = playSprite;
    }

    void ToggleMusic()
    {
        if (isPlaying)
        {
            // ���� ����
            audioSource.Pause();
            musicButton.image.sprite = pauseSprite;
        }
        else
        {
            // ���� ���
            audioSource.Play();
            musicButton.image.sprite = playSprite;
        }

        // ���� ����
        isPlaying = !isPlaying;
    }
}
