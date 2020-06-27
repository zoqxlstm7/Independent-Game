using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RankPanel : BasePanel
{
    [SerializeField] RankSlot slotPrefabs;  // 랭킹 슬롯 프리팹
    [SerializeField] Transform content;     // 스크롤 뷰 컨텐트

    public override void InitializePanel()
    {
        base.InitializePanel();

        Close();
    }

    public override void Show()
    {
        base.Show();

        // 저장된 랭킹데이터를 기반으로 랭킹슬롯 생성 및 초기화
        List<RankData> rankDatas = GameManager.Instance.DataBase.SaveData.rankDatas;
        for (int i = 0; i < rankDatas.Count; i++)
        {
            RankSlot rankSlot = Instantiate(slotPrefabs, content);
            rankSlot.UpdateRankText(i + 1, rankDatas[i].stageCount, rankDatas[i].goldCount, rankDatas[i].killCount);
        }
    }

    public override void Close()
    {
        base.Close();

        // 타임 스케일 정상화
        Time.timeScale = 1;

        // 랭킹 슬롯 제거
        for (int i = 0; i < content.childCount; i++)
        {
            Destroy(content.GetChild(i).gameObject);
        }
    }

    public void OnClose()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        Close();
    }
}
