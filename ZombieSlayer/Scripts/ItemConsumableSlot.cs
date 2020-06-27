using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemConsumableSlot : ItemSlot
{
    [SerializeField] AcquireType acquireType;   // 소모품 아이템 타입

    ItemShopPanel itemShopPanel;                // 아이템 샵 패널 객체
    SaveData saveData;                          // 세이브 데이터 객체

    /// <summary>
    /// 초기화 진행 함수
    /// </summary>
    public override void InitializeSlot()
    {
        // 타입에 따른 스프라이트 설정
        itemSprite = GameManager.Instance.SpriteSetManager.GetSprite(acquireType.ToString());

        base.InitializeSlot();

        // 캐싱
        itemShopPanel = PanelManager.GetPanel(typeof(ItemShopPanel)) as ItemShopPanel;
        saveData = GameManager.Instance.DataBase.SaveData;
    }

    /// <summary>
    /// 수량이 초과되었는지 검사
    /// </summary>
    /// <returns></returns>
    bool IsExcessCount()
    {
        DisplayModel displayModel = GameManager.Instance.GetCurrentSceneManager<LobbySceneManager>().DisplayPlayer;

        switch (acquireType)
        {
            // 수류탄인 경우
            case AcquireType.GRENADE_ITEM:
                if (displayModel.GrenadeCount >= 3)
                    return true;
                break;
            // tnt인 경우
            case AcquireType.TNT_ITEM:
                if (displayModel.TntCount >= 3)
                    return true;
                break;
        }

        return false;
    }

    /// <summary>
    /// 수량을 초과했는지 검사하는 함수
    /// </summary>
    void CheckCount()
    {
        if (IsExcessCount())
            itemShopPanel.SetInteractable(false);
        else
            itemShopPanel.SetInteractable(true);
    }

    /// <summary>
    /// 구매 가능한지 검사
    /// </summary>
    void CheckBuyable()
    {
        // 구매가능한 슬롯만 구매버튼 활성화
        if (saveData.gold >= buyForGold)
            CheckCount();
        else
            itemShopPanel.SetInteractable(false);
    }

    /// <summary>
    /// 슬롯이 선택되었을 때의 처리
    /// </summary>
    public override void OnSelectSlot()
    {
        base.OnSelectSlot();

        // 구매 가능한지 검사
        CheckBuyable();
    }

    /// <summary>
    /// 아이템 구매 시 처리 함수
    /// </summary>
    public override void Buy()
    {
        DisplayModel displayModel = GameManager.Instance.GetCurrentSceneManager<LobbySceneManager>().DisplayPlayer;

        switch (acquireType)
        {
            // 수류탄인 경우 수류탄 수 증가
            case AcquireType.GRENADE_ITEM:
                // 3개까지 구매 제한
                if(displayModel.GrenadeCount < 3)
                {
                    displayModel.GrenadeCount++;
                    saveData.grenadeCount++;
                }
                break;
            // tnt인 경우 tnt 수 증가
            case AcquireType.TNT_ITEM:
                // 3개까지 구매 제한
                if(displayModel.TntCount < 3)
                {
                    displayModel.TntCount++;
                    saveData.tntCount++;
                }
                break;
        }

        // 금액 차감
        saveData.gold -= buyForGold;
        // 구매 가능한지 검사
        CheckBuyable();

        GameManager.Instance.DataBase.Save();
    }
}
