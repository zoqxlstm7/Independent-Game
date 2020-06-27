using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ModelShopPanel : BasePanel
{
    [SerializeField] Image selectModelImage;        // 선택된 모델 이미지 표시

    [SerializeField] Button[] modelsBtn;
    [SerializeField] Text goldText;                 // 골드 텍스트
    [SerializeField] Button equipBtn;               // 장착 버튼
    [SerializeField] Button buyBtn;                 // 구매 버튼

    int selectModelIndex;                           // 선택도니 모델 인덱스

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

        Close();
    }

    public override void Show()
    {
        base.Show();

        ModelSpriteInit();
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();

        ModelSlot modelSlot = GameManager.Instance.Models[selectModelIndex].GetComponent<ModelSlot>();
        if (modelSlot != null)
        {
            // 선택된 모델 이름 및 이미지 출력
            selectModelImage.sprite = modelSlot.Icon;
        }
        else
        {
            Debug.Log("null");
        }
    }

    /// <summary>
    /// 구입에 필요한 골드 표시
    /// </summary>
    public void UpdateGoldText(string value)
    {
        goldText.text = value;
    }

    /// <summary>
    /// 모델 스프라이트 이미지 초기화
    /// </summary>
    void ModelSpriteInit()
    {
        GameManager gameManager = GameManager.Instance;

        // 모델 스프라이트 이미지 지정
        for (int i = 0; i < gameManager.GunModels.Length; i++)
        {
            ModelSlot modelSlot = gameManager.Models[i].GetComponent<ModelSlot>();
            if (modelSlot != null)
            {
                modelsBtn[i].image.sprite = modelSlot.Icon;

                // 구매 가능한 모델일 경우 구매에 필요한 골드 노출
                ActivatedBuyGold(i, modelSlot.BuyableGold);
            }
        }
    }

    /// <summary>
    /// 선택 모델 인덱스 저장 버튼
    /// </summary>
    /// <param name="index"></param>
    public void OnSelectModelBtn(int index)
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        selectModelIndex = index;

        ActivatedEquipBuyButton(index);

        // 구매 가능한 모델의 경우 오른쪽 밑단 가격 표시
        if (BuyableModelCheck(index))
        {
            ModelSlot modelSlot = GameManager.Instance.Models[selectModelIndex].GetComponent<ModelSlot>();
            if (modelSlot != null)
                UpdateGoldText(modelSlot.BuyableGold.ToString());
        }
        else
        {
            UpdateGoldText("");
        }
    }

    /// <summary>
    /// 장착버튼, 구매버튼 활성/비활성
    /// </summary>
    /// <param name="index">선택된 모델 인덱스</param>
    void ActivatedEquipBuyButton(int index)
    {
        // 구매가능한 모델인 경우 구매 버튼 활성화
        if (BuyableModelCheck(index))
        {
            equipBtn.gameObject.SetActive(false);
            buyBtn.gameObject.SetActive(true);
        }
        else // 구매한 모델인 경우 장착 버튼 활성화
        {
            equipBtn.gameObject.SetActive(true);
            buyBtn.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 구매에 필요한 골드 활성/비활성
    /// </summary>
    /// <param name="index">선택된 모델 인덱스</param>
    /// <param name="needGold">구매에 필요한 골드</param>
    void ActivatedBuyGold(int index, int needGold)
    {
        // 구매하지 않은 모델일 시 가격 노출
        if (BuyableModelCheck(index))
            modelsBtn[index].transform.GetChild(0).GetComponent<Text>().text = needGold.ToString();
        else
            modelsBtn[index].transform.GetChild(0).GetComponent<Text>().text = "";
    }

    /// <summary>
    /// 구매가능한 모델인지 검사
    /// </summary>
    /// <param name="index">선택된 모델 인덱스</param>
    /// <returns></returns>
    bool BuyableModelCheck(int index)
    {
        // 구매할 수 있는 모델인지 확인
        if (saveData.buyableModels.Contains(index))
            return true;
        else
            return false;
    }

    /// <summary>
    /// 모델 구입 처리 함수
    /// </summary>
    public void OnBuyModel()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        ModelSlot modelSlot = GameManager.Instance.Models[selectModelIndex].GetComponent<ModelSlot>();
        if (modelSlot != null)
        {
            // 구매 가능한 골드를 소지하고 있는 경우
            if (saveData.gold >= modelSlot.BuyableGold)
            {
                // 구매 가능한 모델 리스트에서 삭제
                saveData.buyableModels.Remove(selectModelIndex);
                // 골드 차감
                saveData.gold -= modelSlot.BuyableGold;
                GameManager.Instance.DataBase.Save();
                // 구매/장착 버튼 활성/비활성
                ActivatedEquipBuyButton(selectModelIndex);
                // 구매에 필요한 골드 활성/비활성
                ActivatedBuyGold(selectModelIndex, modelSlot.BuyableGold);
            }
        }
    }

    /// <summary>
    /// 모델 장착 실행 함수
    /// </summary>
    public void OnEquipModel()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 선택된 모델 인덱스 저장
        saveData.modelIndex = selectModelIndex;
        GameManager.Instance.DataBase.Save();

        // 전시된 모델 제거
        LobbySceneManager lobbySceneManager = GameManager.Instance.GetCurrentSceneManager<LobbySceneManager>();
        Destroy(lobbySceneManager.DisplayPlayer.gameObject);

        // 선택된 모델 인덱스로 전시 모델 생성
        GameObject go = Instantiate(lobbySceneManager.Model, Vector3.zero, Quaternion.identity);
        lobbySceneManager.DisplayPlayer = go.GetComponent<DisplayModel>();

        // 캐릭터 모델, 주무기, 보조무기 설정
        lobbySceneManager.DisplayPlayer.SetPlayer(saveData.modelIndex, saveData.mainGunIndex, saveData.subGunIndex, new Quaternion(0, 180, 0, 0));

        // 모델 상점 패널 비활성화
        Close();
    }
}
