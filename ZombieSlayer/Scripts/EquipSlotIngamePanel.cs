using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlotIngamePanel : EquipSlotPanel
{
    /// <summary>
    /// 업데이트 처리
    /// </summary>
    public override void UpdatePanel()
    {
        base.UpdatePanel();

        UpdateCountText();
    }

    /// <summary>
    /// 총알 및 소모 아이템 갯수 업데이트
    /// </summary>
    void UpdateCountText()
    {
        //// 플레이어 객체 캐싱
        //Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero;

        //// 보조무기가 있는 경우 탄알 수 표시
        //if(player.SubGun != null)
        //    subBulletCountText.text = player.SubGun.RemainBulletCount.ToString();

        //// 주무기가 있는 경우 탄알 수 표시
        //if(player.MainGun != null)
        //    mainBulletCountText.text = player.MainGun.RemainBulletCount + " / " + player.MainGun.ReloadableBulletCount;

        //// 수류탄 갯수 표시
        //grenadeCountText.text = player.GrenadeCount.ToString();
    }
}
