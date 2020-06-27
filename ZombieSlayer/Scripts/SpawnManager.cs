using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 등급에 따른 에너미 파일 경로 데이터
/// </summary>
[System.Serializable]
public class EnemyGradeData
{
    public string[] filePath;
}
/// <summary>
/// 웨이브 데이터
/// </summary>
[System.Serializable]
public class Wave
{
    public EnemyGradeData[] gradeEnemyData; // 등급에 따른 에너미 데이터
    public int spawnCount;                  // 한번에 스폰될 양
    public int waveCount;                   // 진행될 웨이브 카운트
    public int addSpawnCount;               // 스테이지마다 늘어날 스폰 수
    public int spawnTime;                   // 주기적 스폰 시간
    public int stageFactor;                 // 좀비 등급 상승 팩터
    public int bossSpawnFactor;             // 보스 등장 팩터

    public int stageCount;                  // 진행 스테이지

    public int enemyGradeSpawnIndex;        // 생성할 등급까지의 인덱스
    public int remainEnemyCount;            // 남아있는 에너미 수
    public int stageInKillCount;            // 한 스테이지 내에서 죽인 수
}

public class SpawnManager : MonoBehaviour
{
    const float SPAWN_RANGE = 3f;                       // 스폰 반경
    const float MIN_GENERATE_SIZE = -30;                // 생성지점 최소 좌표
    const float MAX_GENERATE_SIZE = 30;                 // 생성지점 최대 좌표

    [SerializeField] Transform[] spawnPoints;           // 스폰 지점들
    [SerializeField] Transform bossGeneratePoint;       // 보스 생성 지점
    [SerializeField] Wave wave;                         // 웨이브 데이터
    public Wave Wave
    {
        get => wave;
    }

    int spawnPointIndex;                                // 스폰 지점 인덱스

    float lastActionTime;                               // 마지막 행동 시간
    bool isSpawnStart;                                  // 게임 시작되었는지 여부

    bool isGenerateBoss;                                // 보스 생성이 되었는지 여부

    /// <summary>
    /// 스폰 시작 함수
    /// </summary>
    public void SpawnStart()
    {
        lastActionTime = Time.time - wave.spawnTime;
        isSpawnStart = true;

        Debug.Log("SpawnStart");
    }

    private void Update()
    {
        // 게임매니저가 없는 경우 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        // 게임 오버시 리턴
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().IsGameOver)
            return;

        // 스폰 시작이 아니라면 리턴
        if (!isSpawnStart)
            return;

        if (wave.waveCount == 0)
            return;

        // 주기적 스폰
        if (Time.time - lastActionTime >= wave.spawnTime)
        {
            wave.waveCount--;
            wave.remainEnemyCount += wave.spawnCount;

            Spawn();
            lastActionTime = Time.time;
        }
    }

    /// <summary>
    /// 에너미 생성 함수
    /// </summary>
    void Spawn()
    {
        for (int i = 0; i < wave.spawnCount; i++)
        {
            // 랜덤한 등급 좀비 선택
            int ranGradeNum = Random.Range(0, wave.enemyGradeSpawnIndex + 1);
            // 등급 좀비 중 랜덤한 모델 선택
            int ranEnemyNum = Random.Range(0, wave.gradeEnemyData[ranGradeNum].filePath.Length);
            string filePath = wave.gradeEnemyData[ranGradeNum].filePath[ranEnemyNum];
            // 생성
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EnemyManager.Generate(filePath, RandomSpawnPoint(spawnPointIndex));

            // 스폰 지점 이동
            spawnPointIndex++;

            // 스폰지점을 초과했다면 처음 지점으로 인덱스 초기화
            if (spawnPointIndex >= spawnPoints.Length)
                spawnPointIndex = 0;
        }
    }

    /// <summary>
    /// 반지름을 중심으로 랜덤한 위치에 스폰하는 함수
    /// </summary>
    /// <param name="spawnPointIndex">스폰 포인트</param>
    /// <returns></returns>
    public Vector3 RandomSpawnPoint(int spawnPointIndex)
    {
        // 반경 1을 갖는 구의 랜덤한 위치
        Vector3 ranPoint = Random.onUnitSphere;
        ranPoint.y = 0.0f;

        // 반지름만큼 위치 이동
        float ranRange = Random.Range(-SPAWN_RANGE, SPAWN_RANGE);
        ranPoint = ranPoint * ranRange;

        // 스폰 지점과 더해준 후 반환
        return ranPoint + spawnPoints[spawnPointIndex].position;
    }

    /// <summary>
    /// 생성 지점 처리
    /// </summary>
    /// <returns>생성 지점 반환</returns>
    public Vector3 GeneratePoint()
    {
        float x = Random.Range(MIN_GENERATE_SIZE, MAX_GENERATE_SIZE);
        float z = Random.Range(MIN_GENERATE_SIZE, MAX_GENERATE_SIZE);

        Vector3 generatePoint = new Vector3(x, 0, z);

        return generatePoint;
    }

    /// <summary>
    /// 웨이브를 모두 클리어했는지 검사하는 함수
    /// </summary>
    public void OnCheckWaveClear()
    {
        // 남은 에너미 수 감소 처리
        wave.remainEnemyCount--;
        wave.stageInKillCount++;

        // 보스 생성 시점 검사
        BossGenerateCheck();

        // 남은 에너미가 없고 3웨이브를 모두 진행했다면 웨이브정보 초기화 진행
        if (wave.remainEnemyCount <= 0 && wave.waveCount == 0)
        {
            UpdateWaveData();
        }
    }

    /// <summary>
    /// 보스 생성 시점 검사
    /// </summary>
    void BossGenerateCheck()
    {
        // 보스가 생성되었다면 리턴
        if (isGenerateBoss)
            return;

        // 보스 출현 단계가 아니라면 리턴
        if (wave.stageCount % wave.bossSpawnFactor != 0)
            return;

        // 웨이브의 마지막 단계인 경우
        if (wave.remainEnemyCount > 0 && wave.waveCount == 0)
        {
            // 보스 스폰 알림 메세지 출력
            WavePanel wavePanel = PanelManager.GetPanel(typeof(WavePanel)) as WavePanel;
            wavePanel.ShowNotic(WavePanel.BOSS_SPAWN_MESSAGE);

            wave.remainEnemyCount++;

            // 보스 스폰
            StartCoroutine(SpawnBossDelay(bossGeneratePoint));

            isGenerateBoss = true;
        }
    }

    /// <summary>
    /// 지정된 시간 딜레이 후 보스 스폰
    /// </summary>
    /// <param name="spawnPoint">생성 지점</param>
    /// <returns></returns>
    IEnumerator SpawnBossDelay(Transform spawnPoint)
    {
        yield return new WaitForSeconds(WavePanel.NOTIC_SHOW_TIME);
        BossManager bossManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BossManager;

        // 보스 생성 및 회전값 조정
        Boss boss = bossManager.Generate(bossManager.RandomSelectBoss, spawnPoint.position);
        boss.transform.rotation = Quaternion.Euler(new Vector3(0f, 180f, 0f));

        // 보스 패널 노출
        BossPanel bossPanel = PanelManager.GetPanel(typeof(BossPanel)) as BossPanel;
        bossPanel.Show();
    }

    /// <summary>
    /// 웨이브 정보 업데이트
    /// </summary>
    void UpdateWaveData()
    {
        isSpawnStart = false;
        isGenerateBoss = false;

        // 스테이지 증가
        wave.stageCount++;

        // 스테이지 특정 단계마다 등급 업된 에너미 출현
        if (wave.stageCount % wave.stageFactor == 0)
        {
            if (wave.enemyGradeSpawnIndex < wave.gradeEnemyData.Length - 1)
                wave.enemyGradeSpawnIndex++;
        }

        // 웨이브 카운트, 남아있는 적 카운트 초기화 및 스폰될 양 증가
        wave.waveCount = 3;
        wave.remainEnemyCount = 0;
        wave.stageInKillCount = 0;
        wave.spawnCount += wave.addSpawnCount;

        // 스폰 알림 메세지 출력
        WavePanel wavePanel = PanelManager.GetPanel(typeof(WavePanel)) as WavePanel;
        wavePanel.ShowNotic(WavePanel.SPAWN_MESSAGE);

        // 웨이브 시작
        StartCoroutine(SpawnDelay());
    }

    /// <summary>
    /// 지정된 시간 딜레이 후 웨이브 시작
    /// </summary>
    /// <returns></returns>
    public IEnumerator SpawnDelay()
    {
        yield return new WaitForSeconds(WavePanel.NOTIC_SHOW_TIME);
        SpawnStart();
    }
}
