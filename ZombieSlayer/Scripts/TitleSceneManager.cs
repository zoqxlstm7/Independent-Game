using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleSceneManager : BaseSceneManager
{
    public override void Initialize()
    {
        base.Initialize();

        // 메인 BGM 재생
        GameManager.Instance.SoundManager.PlayBGM(AudioNameConstant.MAIN_SOUND);
    }

    public override void UpdateManager()
    {
        base.UpdateManager();

        // 아무키나 눌렸을 때
        if (Input.anyKeyDown)
        {
            LoadLobbyScene();
        }
    }

    /// <summary>
    /// 로비씬 로드
    /// </summary>
    void LoadLobbyScene()
    {
        GameManager.Instance.SceneController.LoadScene(SceneNameConstant.LOBBY_SCENE);
    }
}
