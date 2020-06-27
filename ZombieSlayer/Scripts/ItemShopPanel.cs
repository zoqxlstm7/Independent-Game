using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemShopPanel : BasePanel
{
    [SerializeField] Image itemImage;       // 아이템 이미지
    [SerializeField] Text itemName;         // 아이템 이름
    [SerializeField] Text itemDescript;     // 아이템 설명
    [SerializeField] Text goldText;         // 골드 텍스트
    [SerializeField] Button buyButton;      // 구매 버튼

    ItemSlot selectItem;                    // 선택된 아이템

    private void OnEnable()
    {
        goldText.text = "";
    }

    public override void InitializePanel()
    {
        base.InitializePanel();

        Close();
    }

    /// <summary>
    /// 구입에 필요한 골드 표시
    /// </summary>
    public void UpdateGoldText(int value)
    {
        goldText.text = value.ToString();
    }

    /// <summary>
    /// 선택된 아이템의 정보를 표시하는 함수
    /// </summary>
    /// <param name="itemSprite">아이템 이미지</param>
    /// <param name="itemName">아이템 이름</param>
    /// <param name="itemDescript">아이템 설명</param>
    /// <param name="itemSlot">선택된 아이템 슬롯</param>
    public void ShowDescript(Sprite itemSprite, string itemName, string itemDescript, ItemSlot itemSlot)
    {
        itemImage.sprite = itemSprite;
        this.itemName.text = itemName;
        this.itemDescript.text = itemDescript;

        selectItem = itemSlot;
    }

    /// <summary>
    /// 구매 버튼
    /// </summary>
    public void OnBuyBtn()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 선택한 아이템 구매 처리
        if(selectItem != null)
            selectItem.Buy();
        
        // 장착 슬롯 UI 변경
        EquipSlotPanel equipSlotPanel = PanelManager.GetPanel(typeof(EquipSlotPanel)) as EquipSlotPanel;
        equipSlotPanel.ChangedEquipSlot(GameManager.Instance.GetCurrentSceneManager<LobbySceneManager>().DisplayPlayer);
    }

    /// <summary>
    /// 구매 버튼 인터랙터블 설정
    /// </summary>
    /// <param name="value">설정 값</param>
    public void SetInteractable(bool value)
    {
        buyButton.interactable = value;
    }
}
