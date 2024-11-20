using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MusicToggleButton : MonoBehaviour
{
    public Button musicButton;          // UI 버튼
    public Sprite playSprite;           // 음악 재생 버튼 이미지
    public Sprite pauseSprite;          // 음악 중지 버튼 이미지
    public AudioSource audioSource;     // 배경 음악 Audio Source

    private bool isPlaying = true;      // 음악 상태 (true: 재생, false: 중지)

    void Start()
    {
        // 버튼 클릭 이벤트에 함수 등록
        musicButton.onClick.AddListener(ToggleMusic);

        // 초기 버튼 이미지를 설정 (음악이 재생 중이라고 가정)
        musicButton.image.sprite = playSprite;
    }

    void ToggleMusic()
    {
        if (isPlaying)
        {
            // 음악 중지
            audioSource.Pause();
            musicButton.image.sprite = pauseSprite;
        }
        else
        {
            // 음악 재생
            audioSource.Play();
            musicButton.image.sprite = playSprite;
        }

        // 상태 변경
        isPlaying = !isPlaying;
    }
}
