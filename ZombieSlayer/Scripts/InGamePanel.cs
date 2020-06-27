using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class InGamePanel : BasePanel
{
    [SerializeField] Image reloadBar;       // 재장전 시간 표시
    [SerializeField] Image hpBar;           // hp 표시
    [SerializeField] Text hpText;           // hp 텍스트 표시
    [SerializeField] Image expBar;          // 획득 경험치 표시

    /// <summary>
    /// 초기화
    /// </summary>
    public override void InitializePanel()
    {
        base.InitializePanel();

        ActivatedReloadBar();
        UpdateExpBar();
    }

    /// <summary>
    /// 재장전 UI 노출/숨김
    /// </summary>
    public void ActivatedReloadBar()
    {
        reloadBar.gameObject.SetActive(!reloadBar.gameObject.activeSelf);
    }

    /// <summary>
    /// 업데이트 처리
    /// </summary>
    public override void UpdatePanel()
    {
        // 게임매니저가 없다면 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        // 플레이어가 없다며 리턴
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player == null)
            return;

        base.UpdatePanel();

        UpdateHpBar();
    }

    /// <summary>
    /// HP 업데이트
    /// </summary>
    void UpdateHpBar()
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player;

        hpBar.fillAmount = player.CurrentHP / player.MaxHP;

        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("{0} / {1}", player.CurrentHP.ToString("N0"), player.MaxHP.ToString("N0"));
        hpText.text = sb.ToString();
    }

    /// <summary>
    /// 경험치바 업데이트
    /// </summary>
    public void UpdateExpBar()
    {
        GameAccumulator ga = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().GameAccumulator;

        expBar.fillAmount = (float)ga.Exp / (float)ga.MaxExp;
    }

    /// <summary>
    /// 재장전 UI 업데이트
    /// </summary>
    /// <param name="currentReloadTime">진행된 시간</param>
    /// <param name="reloadTime">최종 재장전 시간</param>
    public void UpdateReloadBar(float currentReloadTime, float reloadTime)
    {
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player.IsDead)
            return;

        reloadBar.fillAmount = currentReloadTime / (reloadTime + Player.RELOAD_DELAY_TIME);
    }

    /// <summary>
    /// 설정 버튼이 눌렸을 때의 처리
    /// </summary>
    public void OnSettingsBtn()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        SettingsPanel settingsPanel = PanelManager.GetPanel(typeof(SettingsPanel)) as SettingsPanel;
        settingsPanel.Show();

        Time.timeScale = 0;
    }
}
