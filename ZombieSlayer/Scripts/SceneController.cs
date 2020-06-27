using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// 씬이름 상수화
/// </summary>
public class SceneNameConstant
{
    public const string LOBBY_SCENE = "LobbyScene";
    public const string LOADING_SCENE = "LoadingScene";
    public const string INGAME_SCENE = "InGameScene";
}

public class SceneController : MonoBehaviour
{
    private void Start()
    {
        // 씬이 로드되었을 때의 이벤트 함수 연결
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /// <summary>
    /// 현재 씬이름 반환
    /// </summary>
    /// <returns></returns>
    public string GetCurrentSceneName()
    {
        return SceneManager.GetActiveScene().name;
    }

    /// <summary>
    /// 씬을 불러옴
    /// </summary>
    /// <param name="sceneName">불러올 씬이름</param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    /// <summary>
    /// 비동기로드
    /// </summary>
    /// <param name="sceneName">불러올 씬이름</param>
    public void LoadAsync(string sceneName)
    {
        StartCoroutine(LoadSceneAsync(sceneName));
    }

    /// <summary>
    /// 비동기 로드
    /// </summary>
    /// <param name="sceneName">불러올 씬이름</param>
    /// <returns></returns>
    IEnumerator LoadSceneAsync(string sceneName)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        asyncOperation.allowSceneActivation = false;
        float timer = 0.0f;

        while (!asyncOperation.isDone)
        {
            yield return null;

            timer += Time.deltaTime;

            if(asyncOperation.progress >= 0.9f)
            {
                asyncOperation.allowSceneActivation = true;
            }
        }
    }

    /// <summary>
    /// 씬이 로드되었을 때의 콜백메소드
    /// </summary>
    /// <param name="scene">로드된 씬</param>
    /// <param name="loadSceneMode">로드 모드</param>
    public void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        // 현재 씬 관리 객체를 할당
        BaseSceneManager baseSceneManager = FindObjectOfType<BaseSceneManager>();
        GameManager.Instance.CurrentSceneManager = baseSceneManager;
    }
}
