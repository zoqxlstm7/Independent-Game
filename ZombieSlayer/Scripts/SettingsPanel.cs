using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPanel : BasePanel
{
    [SerializeField] Sprite playSoundIcon;      // 음악 재생 이미지 스프라이트
    [SerializeField] Sprite stopSoundIcon;      // 음악 꺼짐 이미지 스프라이트

    [SerializeField] Image bgmImage;            // bgm 버튼
    [SerializeField] Image sfxImage;            // sfx 버튼

    SoundManager soundManager;

    public override void InitializePanel()
    {
        base.InitializePanel();

        soundManager = GameManager.Instance.SoundManager;

        Close();
    }

    public override void Show()
    {
        base.Show();

        // 설정에 따른 bgm 스프라이트 설정
        if (soundManager.IsBgmMute)
            bgmImage.sprite = stopSoundIcon;
        else
            bgmImage.sprite = playSoundIcon;

        // 설정에 따른 sfx 스프라이트 설정
        if (soundManager.IsSfxMute)
            sfxImage.sprite = stopSoundIcon;
        else
            sfxImage.sprite = playSoundIcon;
    }

    public override void Close()
    {
        base.Close();

        // 타임 스케일 정상화
        Time.timeScale = 1;
    }

    /// <summary>
    /// bgm 스프라이트 지정 및 사운드 on/off
    /// </summary>
    public void OnBgmSoundSet()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 뮤트 해제
        if (soundManager.IsBgmMute)
        {
            soundManager.IsBgmMute = false;
            bgmImage.sprite = playSoundIcon;

            // 씬에 맞는 음악 재생
            PlayBgmCurrentScene();
        }
        else // 뮤트 설정
        {
            soundManager.IsBgmMute = true;
            bgmImage.sprite = stopSoundIcon;
            soundManager.StopBGM();
        }
    }

    /// <summary>
    /// 씬에 맞는 음악 재생
    /// </summary>
    void PlayBgmCurrentScene()
    {
        GameManager gameManager = GameManager.Instance;
        Debug.Log(gameManager.SceneController.GetCurrentSceneName());
        // 로비 씬인 경우 메인 음악 재생
        if (gameManager.SceneController.GetCurrentSceneName().Equals(SceneNameConstant.LOBBY_SCENE))
            soundManager.PlayBGM(AudioNameConstant.MAIN_SOUND);
        // 인게임 씬인 경우 배틀 음악 재생
        else if (gameManager.SceneController.GetCurrentSceneName().Equals(SceneNameConstant.INGAME_SCENE))
            soundManager.PlayBGM(AudioNameConstant.BATTLE_SOUND);
    }


    /// <summary>
    /// sfx 스프라이트 지정 및 사운드 on/off
    /// </summary>
    public void OnSfxSoundSet()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 뮤트 해제
        if (soundManager.IsSfxMute)
        {
            soundManager.IsSfxMute = false;
            sfxImage.sprite = playSoundIcon;
        }
        else // 뮤트 설정
        {
            soundManager.IsSfxMute = true;
            sfxImage.sprite = stopSoundIcon;
        }
    }

    /// <summary>
    /// 포기하기 버튼 처리
    /// </summary>
    public void OnGiveUpBtn()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 강제 게임 오버 처리
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().IsGameOver = true;
        // 게임 종료 패널 노출
        GameOverPanel gameOverPanel = PanelManager.GetPanel(typeof(GameOverPanel)) as GameOverPanel;
        gameOverPanel.Show();
        Close();
    }

    /// <summary>
    /// 종료 버튼
    /// </summary>
    public void OnExitBtn()
    {
        Application.Quit();
    }

    /// <summary>
    /// 닫기 버튼
    /// </summary>
    public void OnCloseBtn()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        Close();
    }
}
