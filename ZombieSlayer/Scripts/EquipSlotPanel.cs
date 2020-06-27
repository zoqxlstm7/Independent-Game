using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

public class EquipSlotPanel : BasePanel
{
    [SerializeField] Image[] equipSlots;          // 장착 슬롯 이미지

    [SerializeField] Text subBulletCountText;     // 보조무기 남은 탄알 표시
    [SerializeField] Text mainBulletCountText;    // 주무기 남은 탄알 표시
    [SerializeField] Text grenadeCountText;       // 수류탄 갯수 표시
    [SerializeField] Text tntCountText;           // tnt 갯수 표시

    Player player;                                // 참조할 플레이어 모델

    /// <summary>
    /// 소모품 아이템 갯수 텍스트 표시 함수
    /// </summary>
    /// <param name="grenadeCount">표시할 수류탄 갯수</param>
    /// <param name="tntCount">표시할 tnt갯수</param>
    public void InitializeConsumableItemText(int grenadeCount, int tntCount)
    {
        SpriteSet spriteSet = GameManager.Instance.SpriteSetManager;

        // 수류탄 장착 여부 확인
        if (player.GrenadeCount == 0)
        {
            equipSlots[2].sprite = spriteSet.GetSprite(SpriteSet.BLANK_SPRITE_NAME);
            grenadeCountText.text = "";
        }
        else
        {
            equipSlots[2].sprite = spriteSet.GetSprite(AcquireType.GRENADE_ITEM.ToString());
            grenadeCountText.text = grenadeCount.ToString();
        }

        // tnt 장착 여부 확인
        if (player.TntCount == 0)
        {
            equipSlots[3].sprite = spriteSet.GetSprite(SpriteSet.BLANK_SPRITE_NAME);
            tntCountText.text = "";
        }
        else
        {
            equipSlots[3].sprite = spriteSet.GetSprite(AcquireType.TNT_ITEM.ToString()); ;
            tntCountText.text = tntCount.ToString();
        }
    }

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
        // 인게임씬이 아니라면 리턴
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>() == null)
            return;

        // 플레이어 객체 캐싱
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player;

        // 출력될 문자열 포맷 지정
        StringBuilder stringBuilder = new StringBuilder();

        // 보조무기가 있는 경우 탄알 수 표시
        if (player.SubGun != null)
        {
            // 보조무기 탄알 문자열 조합
            stringBuilder.AppendFormat("{0}", player.SubGun.RemainBulletCount);
            subBulletCountText.text = stringBuilder.ToString();

            // 문자열 제거
            stringBuilder.Clear();
        }

        // 주무기가 있는 경우 탄알 수 표시
        if (player.MainGun != null)
        {
            // 주무기 탄알 문자열 조합
            stringBuilder.AppendFormat("{0} / {1}",
                player.MainGun.RemainBulletCount,
                player.MainGun.ReloadableBulletCount);

            mainBulletCountText.text = stringBuilder.ToString();

            // 문자열 제거
            stringBuilder.Clear();
        }
        else
        {
            mainBulletCountText.text = "";
        }

        // 보유중인 수류탄, TnT 갯수 표시
        InitializeConsumableItemText(player.GrenadeCount, player.TntCount);
    }

    /// <summary>
    /// 장착된 무기로 무기 슬롯 변경
    /// </summary>
    public void ChangedEquipSlot(Player player)
    {
        this.player = player;

        SpriteSet spriteSet = GameManager.Instance.SpriteSetManager;

        // 보조무기 장착 여부 확인
        if (player.SubGun == null)
            equipSlots[0].sprite = spriteSet.GetSprite(SpriteSet.BLANK_SPRITE_NAME);
        else
            equipSlots[0].sprite = player.SubGun.GunSpirteImage;


        // 주무기 장착 여부 확인
        if (player.MainGun == null)
            equipSlots[1].sprite = spriteSet.GetSprite(SpriteSet.BLANK_SPRITE_NAME);
        else
            equipSlots[1].sprite = player.MainGun.GunSpirteImage;

        // 보유중인 수류탄 tnt 갯수 표시
        InitializeConsumableItemText(player.GrenadeCount, player.TntCount);
    }

    /// <summary>
    /// 장착 슬롯 버튼을 누를 때 인덱스에 맞는 기능 실행
    /// </summary>
    /// <param name="index">스왑할 무기 인덱스</param>
    public void OnEquipSlotBtn(int index)
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        player.SwapWeapon(index);
    }
}
