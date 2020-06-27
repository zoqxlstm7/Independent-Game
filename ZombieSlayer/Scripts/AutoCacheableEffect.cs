using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoCacheableEffect : MonoBehaviour
{
    [SerializeField] ParticleSystem myParticle;
    // 파일 경로
    string filePath;
    public string FilePath
    {
        set => filePath = value;
    }

    private void OnEnable()
    {
        StartCoroutine(IsAlive());
    }

    /// <summary>
    /// 파티클의 생존 주기 체크
    /// </summary>
    /// <returns></returns>
    IEnumerator IsAlive()
    {
        while (true)
        {
            yield return null;

            // 파티클 활동이 끝났다면 반환 처리
            if (!myParticle.IsAlive(true))
            {
                GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Remove(filePath, gameObject);
            }
        }
    }
}
