using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RankSlot : MonoBehaviour
{
    [SerializeField] Text rankText;     // 랭킹 텍스트
    [SerializeField] Text stageText;    // 스테이지 텍스트
    [SerializeField] Text goldText;     // 골드 텍스트
    [SerializeField] Text killText;     // 킬카운트 텍스트

    /// <summary>
    /// 랭킹 정보 텍스트 표시 함수
    /// </summary>
    /// <param name="rankCount">랭킹 순위</param>
    /// <param name="stageCount">도달한 스테이지</param>
    /// <param name="goldCount">획득 골드</param>
    /// <param name="killCount">사살한 좀비 수</param>
    public void UpdateRankText(int rankCount, int stageCount, int goldCount, int killCount)
    {
        rankText.text = rankCount.ToString();
        stageText.text = stageCount.ToString();
        goldText.text = goldCount.ToString();
        killText.text = killCount.ToString();
    }
}
