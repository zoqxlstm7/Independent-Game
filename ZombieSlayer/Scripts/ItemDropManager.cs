using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropManager : MonoBehaviour
{
    public const int RIFLE_GET_VALUE = 20;      // 라이플 탄알 기본 획득 수
    public const int SNIPE_GET_VALUE = 5;      // 스나이트 탄알 기본 획득 수
    public const int SHOTGUN_GET_VALUE = 5;    // 샷건 탄알 기본 획득 수
    public const int STRAY_GET_VALUE = 5;      // 유탄 탄알 기본 획득 수
    public const int THROW_ITEM_GET_VALUE = 1;  // 던질 아이템 기본 획득 수
    public const int TNT_ITEM_GET_VALUE = 1;    // tnt 아이템 기본 획득 수
    public const int HEAL_PACK_VALUE = 15;      // 힐팩 기본 회복량
    public const int GOLD_GET_MIN_VALUE = 50;   // 힐팩 기본 회복량
    public const int GOLD_GET_MAX_VALUE = 100;   // 힐팩 기본 회복량

    int maxValue = 100;                     // 최대 확률

    [Range(0,100)]
    [SerializeField] int dropValue = 60;    // 드랍될 확률

    [Range(0, 100), Header("bullet")]
    [SerializeField] int itemDropRate1;     // 아이템1 드랍 확률
    [Range(0, 100), Header("gold")]
    [SerializeField] int itemDropRate2;     // 아이템2 드랍 확률
    [Range(0, 100), Header("heal")]
    [SerializeField] int itemDropRate3;     // 아이템3 드랍 확률
    [Range(0, 100), Header("grenade")]
    [SerializeField] int itemDropRate4;     // 아이템4 드랍 확률
    [Range(0, 100), Header("tnt")]
    [SerializeField] int itemDropRate5;     // 아이템5 드랍 확률

    /// <summary>
    /// 아이템이 드랍되는지 검사하는 함수
    /// </summary>
    /// <param name="gainer">아이템 획득자</param>
    public void DropTale(Actor gainer)
    {
        int pick = Random.Range(0, maxValue + 1);

        // 아이템 드랍된다면 드랍 함수 실행
        if(pick <= dropValue)
        {
            DropItem(gainer);
        }
    }

    /// <summary>
    /// 아이템 드랍 함수
    /// </summary>
    /// <param name="gainer">아이템 획득자</param>
    void DropItem(Actor gainer)
    {
        Player player = gainer.GetComponent<Player>();

        if(player != null)
        {
            int pick = Random.Range(0, maxValue + 1);

            // 비율에 따라 드랍될 아이템 선택 후 아이템 획득 함수 실행
            if (pick <= itemDropRate1)
            {
                TakeItem(AcquireType.BULLET_ITEM, player);
            }
            else if (pick > itemDropRate1 && pick <= itemDropRate1 + itemDropRate2)
            {
                TakeItem(AcquireType.GOLD_ITEM, player);
            }
            else if(itemDropRate1 + itemDropRate2 < pick && pick <= itemDropRate1 + itemDropRate2 + itemDropRate3)
            {
                TakeItem(AcquireType.HEAL_ITEM, player);
            }
            else if(itemDropRate1 + itemDropRate2 + itemDropRate3 < pick && pick <= itemDropRate1 + itemDropRate2 + itemDropRate3 + itemDropRate4)
            {
                TakeItem(AcquireType.GRENADE_ITEM, player);
            }
            else if(itemDropRate1 + itemDropRate2 + itemDropRate3 + itemDropRate4 < pick && pick <= itemDropRate1 + itemDropRate2 + itemDropRate3 + itemDropRate4 + itemDropRate5)
            {
                TakeItem(AcquireType.TNT_ITEM, player);
            }
        }
    }

    /// <summary>
    /// 아이템 획득 처리
    /// </summary>
    /// <param name="acquireType">획득할 아이템 타입</param>
    /// <param name="gainer">아이템 획득자</param>
    public void TakeItem(AcquireType acquireType, Player gainer)
    {
        int value = 0;

        // 아이템 타입에 따라 획득량 증가
        switch (acquireType)
        {
            case AcquireType.BULLET_ITEM:
                // 주무기가 없는 경우 리턴
                if (gainer.MainGun == null)
                    return;

                // 불렛 타입에 따른 총알 수 증가량 변경
                switch (gainer.MainGun.BulletType)
                {
                    case Gun.BulletStyle.RIFLE:
                        value = RIFLE_GET_VALUE;
                        break;
                    case Gun.BulletStyle.SNIPE:
                        value = SNIPE_GET_VALUE;
                        break;
                    case Gun.BulletStyle.SHOTGUN:
                        value = SHOTGUN_GET_VALUE;
                        break;
                    case Gun.BulletStyle.STRAY:
                        value = STRAY_GET_VALUE;
                        break;
                }

                gainer.MainGun.ReloadableBulletCount += value;

                break;
            // 힐 아이템
            case AcquireType.HEAL_ITEM:
                value = HEAL_PACK_VALUE;
                gainer.OnTakeHeal(value);
                break;
            // 수류탄
            case AcquireType.GRENADE_ITEM:
                value = THROW_ITEM_GET_VALUE;
                gainer.GrenadeCount += value;
                break;
            // TnT
            case AcquireType.TNT_ITEM:
                value = TNT_ITEM_GET_VALUE;
                gainer.TntCount += value; 
                break;
            // 골드 
            case AcquireType.GOLD_ITEM:
                int ran = Random.Range(GOLD_GET_MIN_VALUE, GOLD_GET_MAX_VALUE + 1);
                value = ran;
                GameAccumulator ga = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().GameAccumulator;
                ga.Gold += value;
                break;
        }

        // 획득 UI 노출
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().AcquireUIManager.ActivatedInfoUI(value, gainer, acquireType);
    }
}
