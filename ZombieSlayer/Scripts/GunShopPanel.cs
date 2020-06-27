using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GunShopPanel : BasePanel
{
    // 최대값을 통한 비율 계산에 이용
    const float DAMAGE_MAX_RATE = 60f;          // 최대 데미지
    const float RELOAD_TIME_MAX_RATE = 1f;      // 최대 장전시간
    const float VELOCITY_SPEED_MAX_RATE = 1f;   // 최대 연사력
    const float RANGE_SHOOT_MAX_RATE = 80f;     // 최대 사거리
    const float BULLET_COUNT_MAX_RATE = 200;    // 최대 탄창

    // Lerp 게이지 표현시 사용될 시간 비율
    const float LERP_GAGE_RATE = 3.0f;

    [SerializeField] Image selectGunImage;          // 선택된 총기 이미지 표시
    [SerializeField] Text selectGunNameText;        // 선택된 총기 이름 표시

    [Header("Info Gage")]
    [SerializeField] Image damageInfoGage;          // 총기 데미지 정보 표시
    [SerializeField] Image reloadInfoGage;          // 총기 재장전 시간 정보 표시
    [SerializeField] Image velocityInfoGage;        // 총기 연사력 정보 표시
    [SerializeField] Image rangeInfoGage;           // 총기 사거리 정보 표시
    [SerializeField] Image maxBulletCountInfoGage;  // 총기 탄창 정보 표시

    [SerializeField] Button[] gunsBtn;              // 총기 이미지 슬롯
    [SerializeField] Text goldText;                 // 골드 텍스트
    [SerializeField] Button equipBtn;               // 장착 버튼
    [SerializeField] Button buyBtn;                 // 구매 버튼

    int selectGunIndex;                             // 선택된 총기 모델 인덱스

    SaveData saveData;

    private void OnEnable()
    {
        goldText.text = "";

        equipBtn.gameObject.SetActive(true);
        buyBtn.gameObject.SetActive(false);
    }

    public override void InitializePanel()
    {
        base.InitializePanel();

        // 캐싱
        saveData = GameManager.Instance.DataBase.SaveData;

        // 총기 상점 패널 비활성화
        Close();
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();

        SelectGunInfoGage();
    }

    public override void Show()
    {
        base.Show();

        GunSpriteInit();
    }

    /// <summary>
    /// 구입에 필요한 골드 표시
    /// </summary>
    public void UpdateGoldText(string value)
    {
        goldText.text = value;
    }

    /// <summary>
    /// 총기 스프라이트 이미지 초기화
    /// </summary>
    void GunSpriteInit()
    {
        GameManager gameManager = GameManager.Instance;

        // 총기 스프라이트 이미지 지정
        for (int i = 0; i < gameManager.GunModels.Length; i++)
        {
            Gun gun = gameManager.GunModels[i].GetComponent<Gun>();
            if (gun != null)
            {
                gunsBtn[i].image.sprite = gun.GunSpirteImage;

                // 구매 가능한 총일 경우 구매에 필요한 골드 노출
                ActivatedBuyGold(i, gun.BuyableGold);
            }
        }
    }

    /// <summary>
    /// 선택된 총기 정보 게이지 표시
    /// </summary>
    void SelectGunInfoGage()
    {
        GameManager gameManager = GameManager.Instance;

        Gun selectGun = gameManager.GunModels[selectGunIndex].GetComponent<Gun>();
        if (selectGun != null)
        {
            // 선택된 총기 이름 및 이미지 출력
            selectGunImage.sprite = selectGun.GunSpirteImage;
            selectGunNameText.text = selectGun.GunName;

            // 데미지, 재장전 시간, 연사력, 사거리, 탄창 정보 표시
            damageInfoGage.fillAmount = Mathf.Lerp(damageInfoGage.fillAmount, selectGun.Damage / DAMAGE_MAX_RATE, Time.deltaTime * LERP_GAGE_RATE);
            reloadInfoGage.fillAmount = Mathf.Lerp(reloadInfoGage.fillAmount, selectGun.ReloadTime / RELOAD_TIME_MAX_RATE, Time.deltaTime * LERP_GAGE_RATE);
            velocityInfoGage.fillAmount = Mathf.Lerp(velocityInfoGage.fillAmount, 1f - (selectGun.MuzzleVelocity / VELOCITY_SPEED_MAX_RATE), Time.deltaTime * LERP_GAGE_RATE);
            rangeInfoGage.fillAmount = Mathf.Lerp(rangeInfoGage.fillAmount, selectGun.RangeOfShot / RANGE_SHOOT_MAX_RATE, Time.deltaTime * LERP_GAGE_RATE);
            maxBulletCountInfoGage.fillAmount = Mathf.Lerp(maxBulletCountInfoGage.fillAmount, selectGun.MaxBulletCount / BULLET_COUNT_MAX_RATE, Time.deltaTime * LERP_GAGE_RATE);
        }
    }

    /// <summary>
    /// 선택 총기 인덱스 저장 버튼
    /// </summary>
    /// <param name="index"></param>
    public void OnSelectGunBtn(int index)
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        selectGunIndex = index;

        ActivatedEquipBuyButton(index);

        // 구매 가능한 총기의 경우 오른쪽 밑단 가격 표시
        if (BuyableGunCheck(index))
        {
            Gun gun = GameManager.Instance.GunModels[selectGunIndex].GetComponent<Gun>();
            if (gun != null)
                UpdateGoldText(gun.BuyableGold.ToString());
        }
        else
        {
            UpdateGoldText("");
        }
    }

    /// <summary>
    /// 구매가능한 총인지 검사
    /// </summary>
    /// <param name="index">선택된 인덱스</param>
    /// <returns></returns>
    bool BuyableGunCheck(int index)
    {
        // 구매할 수 있는 총기인지 확인
        if (saveData.buyableGuns.Contains(index))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 장착버튼, 구매버튼 활성/비활성
    /// </summary>
    /// <param name="index">선택된 총 인덱스</param>
    void ActivatedEquipBuyButton(int index)
    {
        // 구매가능한 총인 경우 구매 버튼 활성화
        if (BuyableGunCheck(index))
        {
            equipBtn.gameObject.SetActive(false);
            buyBtn.gameObject.SetActive(true);
        }
        else // 구매한 총인 경우 장착 버튼 활성화
        {
            equipBtn.gameObject.SetActive(true);
            buyBtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 구매에 필요한 골드 활성/비활성
    /// </summary>
    /// <param name="index">선택된 총 인덱스</param>
    /// <param name="needGold">구매에 필요한 골드</param>
    void ActivatedBuyGold(int index, int needGold)
    {
        // 구매하지 않은 총일 시 가격 노출
        if (BuyableGunCheck(index))
            gunsBtn[index].transform.GetChild(0).GetComponent<Text>().text = needGold.ToString();
        else
            gunsBtn[index].transform.GetChild(0).GetComponent<Text>().text = "";
    }

    /// <summary>
    /// 총기 구입 처리 함수
    /// </summary>
    public void OnBuyGun()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        Gun selectGun = GameManager.Instance.GunModels[selectGunIndex].GetComponent<Gun>();
        if(selectGun != null)
        {
            // 구매 가능한 골드를 소지하고 있는 경우
            if(saveData.gold >= selectGun.BuyableGold)
            {
                // 구매 가능한 총기 리스트에서 삭제
                saveData.buyableGuns.Remove(selectGunIndex);
                // 골드 차감
                saveData.gold -= selectGun.BuyableGold;
                GameManager.Instance.DataBase.Save();
                // 구매/장착 버튼 활성/비활성
                ActivatedEquipBuyButton(selectGunIndex);
                // 구매에 필요한 골드 활성/비활성
                ActivatedBuyGold(selectGunIndex, selectGun.BuyableGold);
            }
        }
    }

    /// <summary>
    /// 총기 장착 실행 함수
    /// </summary>
    public void OnEquipGun()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 총기 교체 함수 호출
        GameManager.Instance.GetCurrentSceneManager<LobbySceneManager>().DisplayPlayer.ChangedGunModel(selectGunIndex);
        // 총기 상점 패널 비활성화
        Close();
    }
}
