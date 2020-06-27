using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 버프 유형
/// </summary>
public enum BuffType
{
    Magazine,
    ReloadTime,
    Velocity,
    Damage,
    RangeOfShot,
    GrenadeDamage,
    TntDamage,
    GrenadeRange,
    TntRange,
    MaxHP,
    SecondForHP,
    MoveSpeed,
    DecreaseDamage,
}

/// <summary>
/// 버프 데이터 정보 클래스
/// </summary>
[System.Serializable]
public class BuffData
{
    public WeaponStyle weaponStyle; // 무기 타입
    public BuffType buffType;       // 버프 타입
    public int maxLevel;            // 버프 최대 레벨
    public int currentLevel;        // 버프 현재 레벨
    public int maxValue;            // 최대 향상 값
    public int currentValue;        // 현재 향상 값
    [TextArea]
    public string Descript;         // 버프 설명

    public Sprite weaponIcon;       // 무기 이미지
    public Sprite buffIcon;         // 버프 이미지

    /// <summary>
    /// 현재 적용값 셋팅
    /// </summary>
    public void SetCurrentValue()
    {
        currentValue = currentLevel * (maxValue / maxLevel);
    }
}

public class BuffManager : MonoBehaviour
{
    [SerializeField] BuffData[] buffDatas;  // 버프 데이터 배열

    private void Start()
    {
        // 버프 및 무기 이미지 스프라이트 설정
        SpriteSet spriteSet = GameManager.Instance.SpriteSetManager;
        for (int i = 0; i < buffDatas.Length; i++)
        {
            buffDatas[i].weaponIcon = spriteSet.GetSprite(buffDatas[i].weaponStyle.ToString());
            buffDatas[i].buffIcon = spriteSet.GetSprite(buffDatas[i].buffType.ToString());
        }
    }

    /// <summary>
    /// 적용 버프 초기화
    /// </summary>
    public void ResetBuff()
    {
        for (int i = 0; i < buffDatas.Length; i++)
        {
            buffDatas[i].currentLevel = 0;
            buffDatas[i].currentValue = 0;
        }
    }

    /// <summary>
    /// 버프 유형에 맞는 버프데이터를 반환하는 함수
    /// </summary>
    /// <param name="buffType">버프 유형</param>
    /// <returns></returns>
    public BuffData GetBuffData(WeaponStyle weaponStyle, BuffType buffType)
    {
        List<BuffData> buffdataList = new List<BuffData>();
        // 배열을 리스트로 변환
        buffdataList.AddRange(buffDatas);

        List<BuffData> datas = buffdataList.FindAll(delegate (BuffData a) { return a.weaponStyle == weaponStyle; });

        // 버프 유형이 같은 데이터를 반환
        return datas.Find(delegate (BuffData a) { return a.buffType == buffType; });
    }

    /// <summary>
    /// 버프 데이터 배열 랜덤 반환
    /// </summary>
    /// <param name="count">루프를 돌 횟수</param>
    /// <returns></returns>
    public BuffData[] GetRandomBuffData(int count)
    {
        List<BuffData> buffdataList = new List<BuffData>();
        // 배열을 리스트로 변환
        buffdataList.AddRange(buffDatas);
        // 현재 레벨이 5미만인 버프만 찾음
        buffdataList = buffdataList.FindAll(delegate (BuffData a) { return a.currentLevel < 5; });

        // 남아 있는 버프가 3개 이상인지 검사
        if (buffdataList.Count < 3)
            return null;

        // 랜덤 선택된 데이터
        BuffData[] selectDatas = new BuffData[count];

        // 매개변수 만큼 루프
        for (int i = 0; i < count; i++)
        {
            int ran = Random.Range(0, buffdataList.Count);
            selectDatas[i] = buffdataList[ran];
            // 중복되지 않도록 리스트에서 제거
            buffdataList.Remove(buffdataList[ran]);
        }

        return selectDatas;
    }

    /// <summary>
    /// 버프 업그레이드
    /// </summary>
    /// <param name="buffData">업그레이드할 버프 데이터</param>
    public void UpgradeBuffData(BuffData buffData)
    {
        buffData.currentLevel += 1;
        buffData.SetCurrentValue();

        // 업그레이드 된 버프 적용 준비
        SetBuffApply(buffData);
    }

    /// <summary>
    /// 업그레이드 된 버프 적용 준비
    /// </summary>
    /// <param name="buffData">적용할 버프 객체</param>
    public void SetBuffApply(BuffData buffData)
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player;

        switch (buffData.weaponStyle)
        {
            case WeaponStyle.WEAPON:
                BuffApply(buffData, player.MainGun);
                break;
            case WeaponStyle.PISTOL:
                BuffApply(buffData, player.SubGun);
                break;
            //case WeaponStyle.GRENADE:
            //    BuffApply(buffData);
            //    break;
            //case WeaponStyle.TNT:
            //    BuffApply(buffData);
            //    break;
            case WeaponStyle.MELEE:
                BuffApply(buffData, null, player);
                break;
        }
    }

    /// <summary>
    /// 업그레이드 된 버프 적용
    /// </summary>
    /// <param name="buffData">적용할 버프 데이터</param>
    /// <param name="gun">총 객체</param>
    /// <param name="player">플레이어 객체</param>
    void BuffApply(BuffData buffData, Gun gun = null, Player player = null)
    {
        switch (buffData.buffType)
        {
            // 탄창 증가
            case BuffType.Magazine:
                if(gun != null)
                {
                    float factor = buffData.currentValue * 0.01f;
                    int value = (int)(gun.OriginMaxBulletCount * factor);
                    gun.MaxBulletCount = gun.OriginMaxBulletCount;
                    gun.MaxBulletCount += value;
                }
                break;
            // 재장전 시간 감소
            case BuffType.ReloadTime:
                if (gun != null)
                {
                    float factor = buffData.currentValue * 0.01f;
                    float value = gun.OriginReloadTime * factor;
                    gun.ReloadTime = gun.OriginReloadTime;
                    gun.ReloadTime -= value;
                }
                break;
            // 연사력 증가
            case BuffType.Velocity:
                if (gun != null)
                {
                    float factor = buffData.currentValue * 0.01f;
                    float value = gun.OriginMuzzleVelocity * factor;
                    gun.MuzzleVelocity = gun.OriginMuzzleVelocity;
                    gun.MuzzleVelocity -= value;
                }
                break;
            // 데미지 증가
            case BuffType.Damage:
                if (gun != null)
                {
                    float factor = buffData.currentValue * 0.01f;
                    int value = (int)(gun.OriginDamage * factor);
                    gun.Damage = gun.OriginDamage;
                    gun.Damage += value;
                }
                break;
            // 사거리 증가
            case BuffType.RangeOfShot:
                if (gun != null)
                {
                    float factor = buffData.currentValue * 0.01f;
                    float value = gun.OriginRangeOfShot * factor;
                    gun.RangeOfShot = gun.OriginRangeOfShot;
                    gun.RangeOfShot += value;
                }
                break;
            // 수류탄 데미지 증가
            //case BuffType.GrenadeDamage:
            //    break;
            //// tnt 데미지 증가
            //case BuffType.TntDamage:
            //    {
            //        int damage = 150;

            //        float factor = buffData.currentValue * 0.01f;
            //        int value = (int)(damage * factor);

            //        TnTManager tnTManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().TnTManager;
            //        tnTManager.ResetDamage(value);
            //    }
            //    break;
            //// 수류탄 폭발 범위 증가
            //case BuffType.GrenadeRange:
            //    break;
            //// tnt 폭발 범위 증가
            //case BuffType.TntRange:
            //    {
            //        float range = 5f;

            //        float factor = buffData.currentValue * 0.01f;
            //        float value = range * factor;

            //        TnTManager tnTManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().TnTManager;
            //        tnTManager.ResetRange(value);
            //    }
            //    break;
            // 최대 체력 증가
            case BuffType.MaxHP:
                {
                    float factor = buffData.currentValue * 0.01f;
                    float value = player.OriginMaxHp * factor;
                    player.MaxHP = player.OriginMaxHp;
                    player.MaxHP += value;

                    player.CurrentHP = player.MaxHP;
                }
                break;
            // 10초당 체력 회복 증가
            case BuffType.SecondForHP:
                {
                    float factor = buffData.currentValue * 0.01f;
                    float value = player.MaxHP * factor;
                    player.RecoveryHP = player.OriginRecoveryHp;
                    player.RecoveryHP += value;
                }
                break;
            // 이동속도 증가
            case BuffType.MoveSpeed:
                {
                    float factor = buffData.currentValue * 0.01f;
                    float value = player.OriginMoveSpeed * factor;
                    player.Speed = player.OriginMoveSpeed;
                    player.Speed += value;
                }
                break;
            // 데미지 감소
            case BuffType.DecreaseDamage:
                {
                    float factor = buffData.currentValue * 0.01f;
                    player.DecreasseDamage = player.OriginDecreasseDamage;
                    player.DecreasseDamage += factor;
                }
                break;
        }
    }
}
