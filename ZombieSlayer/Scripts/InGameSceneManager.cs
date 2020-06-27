using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InGameSceneManager : BaseSceneManager
{
    const float GAME_OVER_DELAY_TIME = 2f;  // 게임오버 딜레이 시간

    // 플레이어 객체 반환
    [SerializeField] Player player;
    public Player Player
    {
        get => player;
        set => player = value;
    }

    // 스폰 관리 객체 반환
    [SerializeField] SpawnManager spawnManager;
    public SpawnManager SpawnManager
    {
        get => spawnManager;
    }

    // 총알 관리 객체 반환
    [SerializeField] BulletManager bulletManager;
    public BulletManager BulletManager
    {
        get => bulletManager;
    }

    // 에너미 관리 객체 반환
    [SerializeField] EnemyManager enemyManager;
    public EnemyManager EnemyManager
    {
        get => enemyManager;
    }

    // 보스 관리 객체 반환
    [SerializeField] BossManager bossManager;
    public BossManager BossManager
    {
        get => bossManager;
    }

    // 탄피 관리 객체 반환
    [SerializeField] EmptyShellManager emptyShellManager;
    public EmptyShellManager EmptyShellManager
    {
        get => emptyShellManager;
    }

    // 이펙트 관리 객체 반환
    [SerializeField] EffectManager effectManager;
    public EffectManager EffectManager
    {
        get => effectManager;
    }

    // 던지기 관리 객체 반환
    [SerializeField] ThrowManager throwManager;
    public ThrowManager ThrowManager
    {
        get => throwManager;
    }

    // tnt 관리 객체 반환
    [SerializeField] TnTManager tnTManager;
    public TnTManager TnTManager
    {
        get => tnTManager;
    }

    // 획득 UI 관리 객체 반환
    [SerializeField] AcquireUIManager acquireUIManager;
    public AcquireUIManager AcquireUIManager
    {
        get => acquireUIManager;
    }

    // 아이템 드랍 관리 객체 반환
    [SerializeField] ItemDropManager itemDropManager;
    public ItemDropManager ItemDropManager
    {
        get => itemDropManager;
    }

    // 게임 진행 사항 계산 관리 객체 반환
    [SerializeField] GameAccumulator gameAccumulator;
    public GameAccumulator GameAccumulator
    {
        get => gameAccumulator;
    }

    // 에너미 캐시 관리 객체 반환
    CacheManager enemyCacheManager = new CacheManager();
    public CacheManager EnemyCacheManager
    {
        get => enemyCacheManager;
    }

    // 탄알 캐시 관리 객체 반환
    CacheManager bulletCacheManager = new CacheManager();
    public CacheManager BulletCacheManager
    {
        get => bulletCacheManager;
    }

    // 탄피 캐시 관리 객체 반환
    CacheManager emptyShellCacheManager = new CacheManager();
    public CacheManager EmptyShellCacheManager
    {
        get => emptyShellCacheManager;
    }

    // 이펙트 캐시 관리 객체 반환
    CacheManager effectCacheManager = new CacheManager();
    public CacheManager EffectCacheManager
    {
        get => effectCacheManager;
    }

    // tnt 캐시 관리 객체 반환
    CacheManager tntCacheManager = new CacheManager();
    public CacheManager TntCacheManager
    {
        get => tntCacheManager;
    }

    // 보스 캐시 관리 객체 반환
    CacheManager bossCacheManager = new CacheManager();
    public CacheManager BossCacheManager
    {
        get => bossCacheManager;
    }

    public bool IsGameOver { get; set; } = false;     // 게임오버 플래그

    private void Start()
    {
        // 게임 매니저 객체가 없으면 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        // 배틀 BGM 재생
        GameManager.Instance.SoundManager.StopBGM();
        GameManager.Instance.SoundManager.PlayBGM(AudioNameConstant.BATTLE_SOUND);

        // 알림 문구 출력 후 스폰 시작
        WavePanel wavePanel = PanelManager.GetPanel(typeof(WavePanel)) as WavePanel;
        wavePanel.ShowNotic(WavePanel.SPAWN_MESSAGE);

        // 게임시작
        StartCoroutine(spawnManager.SpawnDelay());
    }

    public override void UpdateManager()
    {
        base.UpdateManager();

        GameOverCheck();
    }

    /// <summary>
    /// 스폰 시작 함수
    /// </summary>
    public void SpawnStart()
    {
        // 스폰매니저에 스폰 명령 실행
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager.SpawnStart();
    }

    /// <summary>
    /// 게임이 종료되는 조건인지 검사
    /// </summary>
    void GameOverCheck()
    {
        // 게임 오버 시 리턴
        if (IsGameOver)
            return;

        // 플레이어가 죽었다면 게임오버 처리
        if (player.IsDead)
            StartCoroutine(GameOverDelay());
    }

    /// <summary>
    /// 게임 종료 패널을 보여주기까지 딜레이 시간을 줌
    /// </summary>
    /// <returns></returns>
    public IEnumerator GameOverDelay()
    {
        // 게임 오버 처리
        IsGameOver = true;

        yield return new WaitForSeconds(GAME_OVER_DELAY_TIME);
        // 게임 종료 패널 노출
        GameOverPanel gameOverPanel = PanelManager.GetPanel(typeof(GameOverPanel)) as GameOverPanel;
        gameOverPanel.Show();
    }
}
