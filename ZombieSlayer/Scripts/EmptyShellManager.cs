using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EmptyShellManager : MonoBehaviour
{
    // 캐싱할 파일 정보
    [SerializeField] CacheData[] cacheDatas;

    // 로드된 파일캐시 정보를 저장
    Dictionary<string, GameObject> fileCaches = new Dictionary<string, GameObject>();

    private void Start()
    {
        // 게임매니저 오브젝트가 있을 때만 실행
        if (FindObjectOfType<GameManager>())
            Initialize();
    }

    void Initialize()
    {
        PrepareCache();
    }

    /// <summary>
    /// 캐시 준비
    /// </summary>
    void PrepareCache()
    {
        Debug.Log("PrepareCache: EmptyShell");
        for (int i = 0; i < cacheDatas.Length; i++)
        {
            GameObject go = Load(cacheDatas[i].filePath);
            if (go != null)
                GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EmptyShellCacheManager.Generate(cacheDatas[i].filePath, go, cacheDatas[i].cacheCount, transform);
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
    /// 탄피 생성
    /// </summary>
    /// <param name="filePath">생성할 오브젝트 파일 경로</param>
    /// <param name="position">생성 지점</param>
    public EmptyShell Generate(/*string filePath*/int bulletIndex, Vector3 position)
    {
        GameObject go = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EmptyShellCacheManager.Archive(cacheDatas[bulletIndex].filePath, position);

        // 반환받은 객체가 있는 경우 초기화
        if (go != null)
        {
            EmptyShell emptyShell = go.GetComponent<EmptyShell>();
            emptyShell.FilePath = cacheDatas[bulletIndex].filePath;

            return emptyShell;
        }
        else
        {
            // 반환받을 객체가 없는 경우 추가 생성
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EmptyShellCacheManager.Generate(cacheDatas[bulletIndex].filePath, Load(cacheDatas[bulletIndex].filePath), CacheManager.DEFAUT_CACHE_COUNT, transform);
        }

        return null;
    }

    /// <summary>
    /// 탄피 제거
    /// </summary>
    /// <param name="filePath">제거할 오브젝트 파일 경로</param>
    /// <param name="gameObject">제거할 오브젝트</param>
    public void Remove(string filePath, GameObject gameObject)
    {
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EmptyShellCacheManager.Restore(filePath, gameObject);
    }
}
