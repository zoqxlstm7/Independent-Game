using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemBuffSlot : ItemSlot
{
    [SerializeField] Image weaponIcon;          // 무기 아이콘
    [SerializeField] WeaponStyle weaponStyle;   // 무기 유형
    [SerializeField] BuffType buffType;         // 버프 유형

    ItemShopPanel itemShopPanel;                // 아이템샵 패널
    SaveData saveData;                          // 세이브데이터 객체
    BuffData buffData;                          // 버프데이터 객체

    /// <summary>
    /// 초기화 진행 함수
    /// </summary>
    public override void InitializeSlot()
    {
        // 버프 타입에 따른 스프라이트 설정
        itemSprite = GameManager.Instance.SpriteSetManager.GetSprite(buffType.ToString());
        // 무기 타입에 따른 스프라이트 설정
        weaponIcon.sprite = GameManager.Instance.SpriteSetManager.GetSprite(weaponStyle.ToString());

        base.InitializeSlot();

        // 캐싱
        BuffManager buffManager = GameManager.Instance.BuffManager;
        buffData = buffManager.GetBuffData(weaponStyle, buffType);

        saveData = GameManager.Instance.DataBase.SaveData;
    }

    /// <summary>
    /// 구매한 버프인지 검사
    /// </summary>
    /// <returns></returns>
    void CheckAlreadyBuy()
    {
        BuffData temp = saveData.buffDatas.Find(delegate (BuffData data) { return (data.weaponIcon == buffData.weaponIcon && data.buffType == buffData.buffType); });

        if (temp == null)
            itemShopPanel.SetInteractable(true);
        else
            itemShopPanel.SetInteractable(false);
    }

    /// <summary>
    /// 구매 가능한지 검사
    /// </summary>
    void CheckBuyable()
    {
        // 구매가능한 슬롯만 구매버튼 활성화
        if (saveData.gold >= buyForGold)
            // 구매한 버프인지에 따라 버튼 인터랙터블 초기화
            CheckAlreadyBuy();
        else
            itemShopPanel.SetInteractable(false);
    }

    /// <summary>
    /// 슬롯이 선택되었을 때의 처리
    /// </summary>
    public override void OnSelectSlot()
    {
        base.OnSelectSlot();

        itemShopPanel = PanelManager.GetPanel(typeof(ItemShopPanel)) as ItemShopPanel;

        // 구매 가능한지 검사
        CheckBuyable();
    }

    /// <summary>
    /// 아이템 구매 시 처리 함수
    /// </summary>
    public override void Buy()
    {
        // 금액 차감
        saveData.gold -= buyForGold;

        // 버프데이터 등록 후 저장
        saveData.buffDatas.Add(buffData);
        GameManager.Instance.DataBase.Save();

        // 구매 가능한지 검사
        CheckBuyable();
    }
}
