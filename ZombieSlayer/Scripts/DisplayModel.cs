using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayModel : Player
{
    protected override void InitializeActor()
    {
        SaveData saveData = GameManager.Instance.DataBase.SaveData;
        // 구입한 수류탄, tnt 전달
        GrenadeCount = saveData.grenadeCount;
        TntCount = saveData.tntCount;

        // 장착 슬롯 UI 변경
        EquipSlotPanel equipSlotPanel = PanelManager.GetPanel(typeof(EquipSlotPanel)) as EquipSlotPanel;
        equipSlotPanel.ChangedEquipSlot(this);
    }

    /// <summary>
    /// 총기 모델 교체 함수
    /// </summary>
    /// <param name="selectGunIndex">교체할 총기 인덱스</param>
    public void ChangedGunModel(int selectGunIndex)
    {
        // 게임 매니저 캐싱
        GameManager gameManager = GameManager.Instance;

        // 변경할 총기 오브젝트 생성
        GameObject go = Instantiate(gameManager.GunModels[selectGunIndex]);
        // 부모 지정
        if(go != null)
            go.transform.SetParent(transform);

        Gun newGun = go.GetComponent<Gun>();
        if(newGun != null)
        {
            // 장착 중인 총기 비활성 및 장착총기 NULL값 초기화
            SetActivatedGunModel(EquipGun.gameObject, false);
            EquipGun = null;

            // 보조무기인 경우
            if (newGun is Pistol)
            {
                // 선택된 총기 인덱스 저장
                gameManager.DataBase.SaveData.subGunIndex = selectGunIndex;

                // 보조무기 교체
                if (SubGun != null)
                    Destroy(SubGun.gameObject);
                SubGun = newGun;

                // 무기에 따른 애니메이션 셋 변경
                SetBaseAnimation(WeaponStyle.PISTOL);
            }
            else // 주무기인경우
            {
                // 선택된 총기 인덱스 저장
                gameManager.DataBase.SaveData.mainGunIndex = selectGunIndex;

                // 주무기 교체
                if (MainGun != null)
                    Destroy(MainGun.gameObject);
                MainGun = newGun;

                // 무기에 따른 애니메이션 셋 변경
                SetBaseAnimation(WeaponStyle.WEAPON);
            }

            // 총꺼내는 애니메이션 재생
            Animator.PlayAnimator(AnimationConstantName.DRAW);

            // 장착 총기 설정
            EquipGun = newGun;
        }

        // 저장
        gameManager.DataBase.Save();

        // 장착 슬롯 UI 변경
        EquipSlotPanel equipSlotPanel = PanelManager.GetPanel(typeof(EquipSlotPanel)) as EquipSlotPanel;
        equipSlotPanel.ChangedEquipSlot(this);
    }

    ///// <summary>
    ///// 총기 활성/비활성 셋팅 함수
    ///// </summary>
    ///// <param name="gunModel">총기 모델</param>
    ///// <param name="value">활성/비활성 여부</param>
    void SetActivatedGunModel(GameObject gunModel, bool value)
    {
        gunModel.SetActive(value);
    }
}
