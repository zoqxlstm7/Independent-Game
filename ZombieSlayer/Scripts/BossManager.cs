using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossManager : MonoBehaviour
{
    // 캐시할 파일 정보를 담을 변수
    [SerializeField] CacheData[] cacheDatas;
    // 로드된 파일캐시 정보를 저장
    Dictionary<string, GameObject> fileCaches = new Dictionary<string, GameObject>();

    public Boss BossInScene { get; set; }      // 씬내 등장한 보스 정보

    // 랜덤으로 선택된 보스 반환
    public string RandomSelectBoss
    {
        get
        {
            int ran = Random.Range(0, cacheDatas.Length);
            return cacheDatas[ran].filePath;
        }
    }

    private void Start()
    {
        // 게임매니저 오브젝트가 있을 때만 실행
        if (FindObjectOfType<GameManager>())
            Initialize();
    }

    public void Initialize()
    {
        PrepareCache();
    }

    /// <summary>
    /// 캐시 준비
    /// </summary>
    void PrepareCache()
    {
        Debug.Log("PrepareCache: Boss");
        for (int i = 0; i < cacheDatas.Length; i++)
        {
            GameObject go = Load(cacheDatas[i].filePath);
            if (go != null)
                GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BossCacheManager.Generate(cacheDatas[i].filePath, go, cacheDatas[i].cacheCount, transform);
        }
    }

    /// <summary>
    /// 캐싱할 파일 메모리 로드
    /// </summary>
    /// <param name="filePath">생성할 캐시 파일 경로</param>
    /// <returns>로드된 오브젝트</returns>
    GameObject Load(string filePath)
    {
        GameObject go = null;

        // 생성되지 않았다면 로드
        if (!fileCaches.ContainsKey(filePath))
        {
            go = Resources.Load<GameObject>(filePath);
            if (go == null)
            {
                Debug.Log("Load Error! filepath: " + filePath);
                return null;
            }

            fileCaches.Add(filePath, go);
        }
        else
        {
            // 생성되있다면 로드된 오브젝트 반환
            go = fileCaches[filePath];
        }

        return go;
    }

    /// <summary>
    /// 보스 생성
    /// </summary>
    /// <param name="filePath">생성할 오브젝트 파일 경로</param>
    /// <param name="position">생성 지점</param>
    public Boss Generate(string filePath, Vector3 position)
    {
        GameObject go = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BossCacheManager.Archive(filePath, position);

        // 반환받은 객체가 있는 경우 초기화
        if (go != null)
        {
            Boss newBoss = go.GetComponent<Boss>();
            newBoss.FilePath = filePath;

            BossInScene = newBoss;

            return newBoss;
        }
        else
        {
            // 반환받을 객체가 없는 경우 추가 생성
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BossCacheManager.Generate(filePath, Load(filePath), 1, transform);
            Generate(filePath, position);
        }

        return null;
    }

    /// <summary>
    /// 보스 제거
    /// </summary>
    /// <param name="filePath">제거할 오브젝트 파일 경로</param>
    /// <param name="enemy">제거할 오브젝트</param>
    public void Remove(string filePath, GameObject gameObject)
    {
        BossInScene = null;

        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BossCacheManager.Restore(filePath, gameObject);
    }
}
