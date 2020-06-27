using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : BasePanel
{
    [SerializeField] GunShopPanel gunShopPanel;     // 총기 상점 패널
    [SerializeField] ItemShopPanel itemShopPanel;   // 아이템 상점 패널
    [SerializeField] ModelShopPanel modelShopPanel; // 모델 상점 패널
    [SerializeField] Text goldText;                 // 골드 텍스트

    public override void UpdatePanel()
    {
        base.UpdatePanel();

        UpdateGoldText();
    }

    /// <summary>
    /// 골드 텍스트 업데이트
    /// </summary>
    void UpdateGoldText()
    {
        goldText.text = GameManager.Instance.DataBase.SaveData.gold.ToString();//GameManager.Instance.Gold.ToString();
    }

    /// <summary>
    /// 패널 활성/비활성 처리
    /// </summary>
    /// <param name="panel">처리할 패널 객체</param>
    void ActivatedPanel(BasePanel panel)
    {
        // 패널이 비활성화라면 활성화
        if (panel.gameObject.activeSelf == false)
            panel.Show();
        else
            panel.Close();
    }

    /// <summary>
    /// 총기 상점 패널 열기
    /// </summary>
    public void OnOpenGunShopPanel()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        ActivatedPanel(gunShopPanel);
    }

    /// <summary>
    /// 아이템 상점 패널 열기
    /// </summary>
    public void OnOpenItemShopPanel()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        ActivatedPanel(itemShopPanel);
    }

    /// <summary>
    /// 모델 상점 패널 열기
    /// </summary>
    public void OnOpenModelShoPanel()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        ActivatedPanel(modelShopPanel);
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

    /// <summary>
    /// 랭킹 버튼이 눌렸을 때의 처리
    /// </summary>
    public void OnRankBtn()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        RankPanel rankPanel = PanelManager.GetPanel(typeof(RankPanel)) as RankPanel;
        rankPanel.Show();

        Time.timeScale = 0;
    }
}
