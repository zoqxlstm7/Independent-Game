using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectManager : MonoBehaviour
{
    public const string EXPLOSION_EFFECT_FILE_PATH = "Effect/GrenadeEffect";        // 폭발 이펙트 파일 경로
    public const string REMAIN_FIRE_FILE_PATH = "Effect/RemainFireEffect";          // 잔여 불 이펙트 파일 경로
    public const string VENOM_SPEAR_FILE_PATH = "Effect/VenomSpearEffect";          // 베놈스피어 이펙트 파일 경로
    public const string LAND_MINE_EFFECT_FILE_PATH = "Effect/TntEffect";            // 지뢰 폭발 이펙트 파일 경로
    public const string BULLET_HOLE_EFFECT_FILE_PATH = "Effect/BulletHoleEffect";   // 총알 구멍 이펙트 파일 경로

    // 캐싱할 파일 정보
    [SerializeField] CacheData[] cacheDatas;
    // 로드된 파일캐시 정보를 저장
    Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    private void Start()
    {
        // 게임매니저 오브젝트가 있을 때만 실행
        if (FindObjectOfType(typeof(GameManager)))
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
        for (int i = 0; i < cacheDatas.Length; i++)
        {
            GameObject go = Load(cacheDatas[i].filePath);
            if (go != null)
                GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectCacheManager.Generate(cacheDatas[i].filePath, go, cacheDatas[i].cacheCount, transform);
        }
    }

    /// <summary>
    /// 캐싱할 파일 메모리 로드
    /// </summary>
    /// <param name="filePath">캐싱할 오브젝트 파일 경로</param>
    /// <returns>로드된 오브젝트</returns>
    GameObject Load(string filePath)
    {
        GameObject go = null;

        if (!fileCache.ContainsKey(filePath))
        {
            go = Resources.Load<GameObject>(filePath);
            if (go == null)
            {
                Debug.Log("Load Error! filepath: " + filePath);
                return null;
            }

            fileCache.Add(filePath, go);
        }
        else
        {
            // 생성되있다면 로드된 오브젝트 반환
            return fileCache[filePath];
        }

        return go;
    }

    /// <summary>
    /// 이펙트 생성
    /// </summary>
    /// <param name="filePath">생성할 오브젝트 파일 경로</param>
    /// <param name="position">생성 지점</param>
    public GameObject Generate(string filePath, Vector3 position)
    {
        GameObject go = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectCacheManager.Archive(filePath, position);

        // 반환받은 객체가 있는 경우 반환
        if (go != null)
        {
            AutoCacheableEffect autoCacheableEffect = go.GetComponent<AutoCacheableEffect>();
            if (autoCacheableEffect)
            {
                autoCacheableEffect.FilePath = filePath;
                return go;
            }
        }
        else
        {
            // 반환받을 객체가 없는 경우 추가 생성
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectCacheManager.Generate(filePath, Load(filePath), CacheManager.DEFAUT_CACHE_COUNT, transform);
        }

        return null;
    }

    /// <summary>
    /// 이펙트 제거
    /// </summary>
    /// <param name="filePath">제거할 오브젝트 파일 경로</param>
    /// <param name="gameObject">제거할 오브젝트</param>
    public void Remove(string filePath, GameObject gameObject)
    {
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectCacheManager.Restore(filePath, gameObject);
    }
}
