using System.Collections;
using System.Collections.Generic;

/// <summary>
/// 랭킹 데이터 클래스
/// </summary>
[System.Serializable]
public class RankData
{
    public int stageCount;     // 진행한 스테이지
    public int goldCount;      // 획득한 골드
    public int killCount;      // 사살한 좀비 수

    public RankData(int stageCount, int goldCount, int killCount)
    {
        this.stageCount = stageCount;
        this.goldCount = goldCount;
        this.killCount = killCount;
    }
}

[System.Serializable]
public class SaveData
{
    public int modelIndex;      // 착용중인 모델 인덱스
    public int subGunIndex;     // 착용중인 보조무기 인덱스
    public int mainGunIndex;    // 착용중인 주무기 인덱스
    public int grenadeCount;    // 소지한 수류탄 갯수
    public int tntCount;        // 소지한 tnt 갯수
    public int gold;            // 소지한 골드

    public List<RankData> rankDatas = new List<RankData>(); // 랭킹 데이터

    public List<BuffData> buffDatas = new List<BuffData>(); // 버프 데이터
    public List<int> buyableGuns = new List<int>();         // 구매가능한 총기 리스트
    public List<int> buyableModels = new List<int>();       // 구매가능한 모델 리스트

    public SaveData()
    {
        ///---------------------------
        /// 구매한 아이템은 리스트에서 인덱스를 제거한다.
        ///---------------------------

        // 구매 가능한 총기 인덱스 초기화
        for (int i = 0; i < 18; i++)
        {
            buyableGuns.Add(i);
        }
        // 시작 시 주어지는 총 인덱스 제거
        buyableGuns.Remove(0);

        // 구매 가능한 모델 인덱스 초기화
        for (int i = 0; i < 19; i++)
        {
            buyableModels.Add(i);
        }
        // 시작 시 주어지는 모델 인덱스 제거
        buyableModels.Remove(0);
    }

    /// <summary>
    /// 랭킹데이터 정렬
    /// </summary>
    public void RankDataSort()
    {
        // 킬카운트를 기준으로 내림차순으로 정렬
        rankDatas.Sort(delegate (RankData a, RankData b) { return b.killCount - a.killCount; });
    }
}
