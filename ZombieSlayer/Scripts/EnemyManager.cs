using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyManager : MonoBehaviour
{
    // 캐시할 파일 정보를 담을 변수
    [SerializeField] CacheData[] cacheDatas;

    // 로드된 파일캐시 정보를 저장
    Dictionary<string, GameObject> fileCaches = new Dictionary<string, GameObject>();
    // 생성된 에너미 관리 리스트
    [SerializeField] List<Enemy> enemies = new List<Enemy>();                           

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
        Debug.Log("PrepareCache: Enemy");
        for (int i = 0; i < cacheDatas.Length; i++)
        {
            GameObject go = Load(cacheDatas[i].filePath);
            if(go != null)
                GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EnemyCacheManager.Generate(cacheDatas[i].filePath, go, cacheDatas[i].cacheCount, transform);
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
            if(go == null)
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
    /// 에너미 생성
    /// </summary>
    /// <param name="filePath">생성할 오브젝트 파일 경로</param>
    /// <param name="position">생성 지점</param>
    public void Generate(string filePath, Vector3 position)
    {
        GameObject go = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EnemyCacheManager.Archive(filePath, position);

        // 반환받은 객체가 있는 경우 초기화
        if(go != null)
        {
            Enemy newEnemy = go.GetComponent<Enemy>();
            newEnemy.FilePath = filePath;
            AddList(newEnemy);
        }
        else
        {
            // 반환받을 객체가 없는 경우 추가 생성
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EnemyCacheManager.Generate(filePath, Load(filePath), CacheManager.DEFAUT_CACHE_COUNT, transform);
            Generate(filePath, position);
        }
    }

    /// <summary>
    /// 에너미 제거
    /// </summary>
    /// <param name="filePath">제거할 오브젝트 파일 경로</param>
    /// <param name="enemy">제거할 오브젝트</param>
    public void Remove(string filePath, Enemy enemy)
    {
        enemies.Remove(enemy);
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EnemyCacheManager.Restore(filePath, enemy.gameObject);
    }

    /// <summary>
    /// 에너미 리스트에 추가
    /// </summary>
    /// <param name="enemy">추가할 에너미 오브젝트</param>
    void AddList(Enemy enemy)
    {
        if (!enemies.Contains(enemy))
            enemies.Add(enemy);
    }

    public int GetCount()
    {
        return enemies.Count;
    }
}
