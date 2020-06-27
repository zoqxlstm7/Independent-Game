using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pistol : Gun
{
    private void OnEnable()
    {
        firePointName = "PistolFirePoint";
    }

    public override void Initialize()
    {
        base.Initialize();

        // 인게임 씬이 아닌 경우 리턴
        if (!GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>())
            return;

        // 장착 총이 보조무기가 아닌 경우 보조무기 숨김 처리
        if (!(GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player.EquipGun is Pistol))
            gameObject.SetActive(false);
    }

    /// <summary>
    /// 재장전해야 하는지 검사
    /// </summary>
    /// <returns>재장전 여부</returns>
    public override bool CheckReload()
    {
        // 재장전해야 하는지 검사
        if (remainBulletCount <= 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 재장전 트리거 실행
    /// </summary>
    /// <returns>재장전 여부</returns>
    public override bool OnReload()
    {
        // 총알을 한발이라도 발사했고 재장전해야되는 상황인지 검사
        if (remainBulletCount < maxBulletCount && !isReload)
        {
            lastActionTime = Time.time;
            isReload = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 재장전 처리
    /// </summary>
    /// <returns>재장전 완료 여부</returns>
    public override bool Reload()
    {
        // 재장전 처리가 진행중이 아니라면 리턴
        if (!isReload)
            return false;

        // 리로드 UI 노출
        InGamePanel inGamePanel = PanelManager.GetPanel(typeof(InGamePanel)) as InGamePanel;
        inGamePanel.UpdateReloadBar(Time.time - lastActionTime, ReloadTime);

        // 재장전 시간 경과 후
        if (Time.time - lastActionTime - Player.RELOAD_DELAY_TIME > ReloadTime)
        {
            // 장전해야할 탄알 계산
            int reloadCount = maxBulletCount - remainBulletCount;

            // 총알 장전
            remainBulletCount += reloadCount;

            lastActionTime = Time.time;
            isReload = false;

            return true;
        }

        return false;
    }
}
