using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 캐시 정보를 담을 클래스
/// </summary>
[System.Serializable]
public class CacheData
{
    public string filePath; // 캐시 파일 경로
    public int cacheCount;  // 캐시를 만들 수
}

public class CacheManager
{
    public const int DEFAUT_CACHE_COUNT = 30;  // 추가 생성시 기본값

    // 캐시 데이터를 보관할 변수
    Dictionary<string, Queue<GameObject>> caches = new Dictionary<string, Queue<GameObject>>();

    /// <summary>
    /// 캐시 생성
    /// </summary>
    /// <param name="filePath">생성할 캐시 파일 경로</param>
    /// <param name="gameObject">생성할 캐시 오브젝트</param>
    /// <param name="cacheCount">생성할 양</param>
    /// <param name="parentTransform">부모 트랜스폼</param>
    public void Generate(string filePath, GameObject gameObject, int cacheCount, Transform parentTransform = null)
    {
        // 캐시가 생성되지 않았다면 캐시 생성
        if (!caches.ContainsKey(filePath))
        {
            Queue<GameObject> queue = new Queue<GameObject>();

            // 캐시 카운트만큼 캐시 생성
            for (int i = 0; i < cacheCount; i++)
            {
                GameObject go = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(parentTransform);
                queue.Enqueue(go);
                go.SetActive(false);
            }

            // 캐시 적재
            caches.Add(filePath, queue);
        }
        else
        {
            // 캐시가 부족할 경우 추가 생성
            for (int i = 0; i < cacheCount; i++)
            {
                GameObject go = Object.Instantiate(gameObject, Vector3.zero, Quaternion.identity);
                go.transform.SetParent(parentTransform);
                caches[filePath].Enqueue(go);
                go.SetActive(false);
            }
        }
    }

    /// <summary>
    /// 캐시 보관 및 사용
    /// </summary>
    /// <param name="filePath">사용할 캐시 파일 경로</param>
    /// <param name="position">생성할 지점</param>
    /// <returns></returns>
    public GameObject Archive(string filePath, Vector3 position)
    {
        GameObject go = null;

        // 남아있는 캐시가 없다면
        if (caches[filePath].Count == 0)
        {
            Debug.Log("Archive not remain. filePath: " + filePath);
            return null;
        }

        // 캐시가 저장되어 있다면 저장된 캐시 오브젝트 반환
        if (caches.ContainsKey(filePath))
        {
            go = caches[filePath].Dequeue();
            go.transform.position = position;
            go.SetActive(true);

            return go;
        }

        return go;
    }

    /// <summary>
    /// 사용한 캐시 반환
    /// </summary>
    /// <param name="filePath">반환할 캐시 파일 경로</param>
    /// <param name="gameObject">반환할 오브젝트</param>
    /// <returns>반환 성공 유무</returns>
    public bool Restore(string filePath, GameObject gameObject)
    {
        // 캐시가 저장되어 있다면
        if (caches.ContainsKey(filePath))
        {
            caches[filePath].Enqueue(gameObject);
            gameObject.SetActive(false);

            return true;
        }

        return false;
    }
}
