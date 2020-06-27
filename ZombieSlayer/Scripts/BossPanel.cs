using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BossPanel : BasePanel
{
    [SerializeField] Vector3 offset;        // 월드에 표시될 네임텍스트 포지션 오프셋

    [SerializeField] Image hpBar;           // 보스 hpBar
    [SerializeField] Text bossNameText;     // 보스 이름 텍스트
    [SerializeField] Text followNameText;   // 보스 팔로우 이름 텍스트

    Boss boss;                              // 현재 생성된 보스 객체

    public override void InitializePanel()
    {
        base.InitializePanel();

        Close();
    }

    public override void Show()
    {
        base.Show();

        boss = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BossManager.BossInScene;
        // 보스 이름 지정
        bossNameText.text = boss.BossName;
        followNameText.text = boss.BossName;
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();

        UpdateHpBar();
        UpdateFollowName();
    }

    /// <summary>
    /// 체력바 업데이트 처리
    /// </summary>
    void UpdateHpBar()
    {
        hpBar.fillAmount = boss.CurrentHP / boss.MaxHP;
    }

    /// <summary>
    /// 보스 이름 따라다니도록 위치 업데이트 처리
    /// </summary>
    void UpdateFollowName()
    {
        Vector3 pos = Camera.main.WorldToScreenPoint(boss.transform.position + offset);
        pos.z = 0;

        followNameText.transform.position = pos;
    }
}
