using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TnTManager : MonoBehaviour
{
    public const string TNT_FILE_PATH = "Prefabs/TnT";

    // 캐싱할 파일 정보
    [SerializeField] CacheData[] cacheDatas;
    // 로드된 파일캐시 정보를 저장
    Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        PrepareCache();
    }

    /// <summary>
    /// 캐시 미리 생성
    /// </summary>
    void PrepareCache()
    {
        for (int i = 0; i < cacheDatas.Length; i++)
        {
            GameObject go = Load(cacheDatas[i].filePath);
            if (go != null)
                GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().TntCacheManager.Generate(cacheDatas[i].filePath, go, cacheDatas[i].cacheCount, transform);
        }
    }

    /// <summary>
    /// 캐시를 메모리로 로드
    /// </summary>
    /// <param name="filePath">로드할 파일 경로</param>
    /// <returns></returns>
    GameObject Load(string filePath)
    {
        GameObject go = null;

        // 생성되지 않았다면 로드
        if (!fileCache.ContainsKey(filePath))
        {
            go = Resources.Load<GameObject>(filePath);
            if (go != null)
                fileCache.Add(filePath, go);
        }
        else
        {
            // 생성되있다면 로드된 오브젝트 반환
            go = fileCache[filePath];
        }

        return go;
    }

    /// <summary>
    /// tnt 생성 함수
    /// </summary>
    /// <param name="filePath">생성할 오브젝트 파일 경로</param>
    /// <param name="generatePos">생성 지점</param>
    /// <param name="attacker">공격자 정보</param>
    /// <returns></returns>
    public TNT Generate(string filePath, Vector3 generatePos, Actor attacker)
    {
        GameObject go = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().TntCacheManager.Archive(filePath, generatePos);
        
        // 반환받은 객체가 있는 경우 초기화
        if (go != null)
        {
            TNT newTnt = go.GetComponent<TNT>();
            if (newTnt != null)
            {
                // 데미지 세팅
                BuffData buffData = GameManager.Instance.BuffManager.GetBuffData(WeaponStyle.TNT, BuffType.TntDamage);
                // 레벨을 올렸는지 검사
                if(buffData.currentLevel > 0)
                {
                    Debug.Log("newTnt.OriginValue: " + newTnt.OriginValue);
                    float factor = buffData.currentValue * 0.01f;
                    int value = (int)(newTnt.OriginValue * factor);
                    newTnt.Value = newTnt.OriginValue;
                    newTnt.Value += value;
                }

                // 범위 세팅
                buffData = GameManager.Instance.BuffManager.GetBuffData(WeaponStyle.TNT, BuffType.TntRange);
                // 레벨을 올렸는지 검사
                if (buffData.currentLevel > 0)
                {
                    Debug.Log("newTnt.OriginRange: " + newTnt.OriginRange);
                    float factor = buffData.currentValue * 0.01f;
                    float value = newTnt.OriginRange * factor;
                    newTnt.Range = newTnt.OriginRange;
                    newTnt.Range += value;
                }

                // 파일 경로 및 공격자 설정
                newTnt.FilePath = filePath;
                newTnt.attacker = attacker;
            }

            return newTnt;
        }
        else
        {
            // 반환받을 객체가 없는 경우 추가 생성
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().TntCacheManager.Generate(filePath, Load(filePath), CacheManager.DEFAUT_CACHE_COUNT, transform);
            Generate(filePath, generatePos, attacker);
        }

        return null;
    }

    /// <summary>
    /// tnt 제거 함수
    /// </summary>
    /// <param name="filePath">제거할 오브젝트 파일 경로</param>
    /// <param name="gameObject">제거할 오브젝트</param>
    public void Remove(string filePath, GameObject gameObject)
    {
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().TntCacheManager.Restore(filePath, gameObject);
    }

    /// <summary>
    /// 생성된 tnt 데미지 재설정
    /// </summary>
    /// <param name="damage">설정할 데미지값</param>
    public void ResetDamage(int damage)
    {
        TNT[] tnts = GetComponentsInChildren<TNT>();

        for (int i = 0; i < tnts.Length; i++)
        {
            tnts[i].Value = damage;
        }
    }

    /// <summary>
    /// 생성된 tnt 폭발 범위 재설정
    /// </summary>
    /// <param name="range"></param>
    public void ResetRange(float range)
    {
        TNT[] tnts = GetComponentsInChildren<TNT>();

        for (int i = 0; i < tnts.Length; i++)
        {
            tnts[i].Range = range;
        }
    }
}
