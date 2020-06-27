using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemSlot : MonoBehaviour
{
    [SerializeField] string itemName;               // 아이템 이름
    [SerializeField, TextArea] string itemDescript; // 아이템 설명
    [SerializeField] protected int buyForGold;      // 구입하기 위한 골드

    protected Sprite itemSprite;                    // 적용할 스프라이트 이미지

    Image itemSlot;                                 // 슬롯 이미지

    private void Start()
    {
        // 자기 자신의 이미지 컴포넌트 캐싱
        itemSlot = GetComponent<Image>();

        InitializeSlot();
    }

    /// <summary>
    /// 초기화 함수
    /// </summary>
    public virtual void InitializeSlot()
    {
        // 스프라이트 이미지 설정
        itemSlot.sprite = itemSprite;
    }

    /// <summary>
    /// 아이템 설명을 보여주는 함수
    /// </summary>
    public void ShowDescript()
    {
        ItemShopPanel itemShopPanel = PanelManager.GetPanel(typeof(ItemShopPanel)) as ItemShopPanel;
        itemShopPanel.ShowDescript(itemSprite, itemName, itemDescript, this);
    }

    /// <summary>
    /// 아이템을 구매했을 때의 처리 함수
    /// </summary>
    public virtual void Buy()
    {
    }

    /// <summary>
    /// 슬롯이 선택되었을 때의 처리
    /// </summary>
    public virtual void OnSelectSlot()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 구입에 필요한 골드 표시
        ItemShopPanel itemShopPanel = PanelManager.GetPanel(typeof(ItemShopPanel)) as ItemShopPanel;
        itemShopPanel.UpdateGoldText(buyForGold);
    }
}
