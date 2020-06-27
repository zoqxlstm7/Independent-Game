using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameOverPanel : BasePanel
{
    const float LERP_TIME_RATE = 3f;        // Lerp 타임 비율
    
    [SerializeField] Text stageText;        // 스테이지 텍스트
    [SerializeField] Text goldText;         // 골드 텍스트
    [SerializeField] Text killCountText;    // 좀비 사살 카운트 텍스트

    GameAccumulator gameAccumulator;        // 포인트계산기 객체
    SpawnManager spawnManager;              // 스폰매니저 객체
    Player player;                          // 플레이어 객체

    float stageCount;                       // 스테이지 카운트 변수
    float goldCount;                        // 골드 카운트 변수
    float killCount;                        // 킬 카운트 변수

    public override void InitializePanel()
    {
        base.InitializePanel();

        Close();
    }

    public override void Show()
    {
        base.Show();

        // 객체 캐싱
        gameAccumulator = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().GameAccumulator;
        spawnManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager;
        player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player;

        // 스테이지 보너스 골드 계산
        gameAccumulator.Gold += (ItemDropManager.GOLD_GET_MIN_VALUE * (spawnManager.Wave.stageCount - 1)) + player.KillCount;
        // 획득 골드 저장
        //GameManager.Instance.Gold += gameAccumulator.Gold;

        GameManager gm = GameManager.Instance;
        // 획득 골드 저장
        gm.DataBase.SaveData.gold += gameAccumulator.Gold;

        // 수류탄, tnt 초기화 
        SaveData saveData = gm.DataBase.SaveData;
        saveData.grenadeCount = 0;
        saveData.tntCount = 0;

        // 랭킹 데이터 저장
        saveData.rankDatas.Add(new RankData(
            spawnManager.Wave.stageCount - 1,
            gameAccumulator.Gold,
            player.KillCount
        ));
        // 랭킹 데이터 내림차순 정렬
        saveData.RankDataSort();
        // 데이터가 10개를 초과한 경우 가장 마지막 데이터 제거
        if (saveData.rankDatas.Count > 10)
            saveData.rankDatas.RemoveAt(saveData.rankDatas.Count - 1);

        // 구입 버프 초기화
        saveData.buffDatas.Clear();

        // 저장
        gm.DataBase.Save();

        // 적용 버프 초기화
        gm.BuffManager.ResetBuff();
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();

        UpdateInfo();
    }

    /// <summary>
    /// 게임 오버시 정보 업데이트
    /// </summary>
    void UpdateInfo()
    {
        // 클리어한 스테이지 표시
        stageCount = Mathf.Lerp(stageCount, spawnManager.Wave.stageCount - 1, Time.deltaTime * LERP_TIME_RATE);
        stageText.text = stageCount.ToString("N0");

        // 획득한 골드 표시
        goldCount = Mathf.Lerp(goldCount, gameAccumulator.Gold, Time.deltaTime * LERP_TIME_RATE);
        goldText.text = goldCount.ToString("N0");

        // 사살한 좀비 수 표시
        killCount = Mathf.Lerp(killCount, player.KillCount, Time.deltaTime * LERP_TIME_RATE);
        killCountText.text = killCount.ToString("N0");
    }

    /// <summary>
    /// 로비로 이동
    /// </summary>
    public void OnReturnButton()
    {
        GameManager gameManager = GameManager.Instance;

        // 버튼음 재생
        gameManager.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 메인 BGM 재생
        gameManager.SoundManager.StopBGM();
        gameManager.SoundManager.PlayBGM(AudioNameConstant.MAIN_SOUND);

        // 로비씬으로 이동
        gameManager.SceneController.LoadScene(SceneNameConstant.LOBBY_SCENE);
    }
}
