using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class WavePanel : BasePanel
{
    public const float NOTIC_SHOW_TIME = 5f;                // 알림 노출 시간
    public const string SPAWN_MESSAGE = "Wave !";           // 웨이브 시작 시 나타날 메세지
    public const string BOSS_SPAWN_MESSAGE = "Boss !";      // 보스 출현 시 나타날 메세지
    //public const string BOSS_SPAWN_MESSAGE = "도끼장인 척";  // 보스 스폰 시 나타날 메세지

    [SerializeField] NoticMessage noticMessage;             // 알림 UI

    [SerializeField] Image waveBar;                         // 웨이브 진행 프로그레스 바
    [SerializeField] Text stageText;                        // 진행 스테이지 텍스트

    SpawnManager spawnManager;                              // 스폰매니저

    public override void InitializePanel()
    {
        base.InitializePanel();

        // 스폰 매니저 캐싱
        spawnManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager;
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();

        UpdateWaveBar();
        UpdateStateText();
    }

    /// <summary>
    /// 웨이브 진행바 업데이트
    /// </summary>
    void UpdateWaveBar()
    {
        waveBar.fillAmount = (float)spawnManager.Wave.stageInKillCount / (float)(spawnManager.Wave.spawnCount * 3);
    }

    /// <summary>
    /// 스테이지 텍스트 업데이트
    /// </summary>
    void UpdateStateText()
    {
        StringBuilder sb = new StringBuilder();
        sb.AppendFormat("Stage {0}", spawnManager.Wave.stageCount);

        stageText.text = sb.ToString();
    }

    /// <summary>
    /// 알림 UI 노출 함수
    /// </summary>
    /// <param name="msg">노출할 메세지</param>
    public void ShowNotic(string msg)
    {
        // 알림 UI 노출
        noticMessage.ShowNotic(msg);
    }
}
