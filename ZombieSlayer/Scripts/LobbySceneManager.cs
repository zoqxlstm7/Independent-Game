using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LobbySceneManager : BaseSceneManager
{
    [SerializeField] GameObject model;              // 생성할 모델 객체
    public GameObject Model
    {
        get => model;
    }
    
    [SerializeField] DisplayModel displayModel;     // 전시된 모델 
    public DisplayModel DisplayPlayer
    {
        get => displayModel;
        set => displayModel = value;
    }

    private void Start()
    {
        // 전시될 게임오브젝트 생성
        GameObject go = Instantiate(model, Vector3.zero, Quaternion.identity);
        displayModel = go.GetComponent<DisplayModel>();

        SaveData saveData = GameManager.Instance.DataBase.SaveData;

        // 초기 플레이어 설정
        SetBeginningPlayer(saveData.modelIndex, saveData.mainGunIndex, saveData.subGunIndex);
        // 초기 소모품 아이템 설정
        EquipSlotPanel equipSlotPanel = PanelManager.GetPanel(typeof(EquipSlotPanel)) as EquipSlotPanel;
        equipSlotPanel.InitializeConsumableItemText(displayModel.GrenadeCount, displayModel.TntCount);
    }

    /// <summary>
    /// 처음 다운로드 시 플레이어 설정
    /// </summary>
    /// <param name="modelIndex">설정할 모델 인덱스</param>
    /// <param name="mainGunIndex">설정할 주무기 인덱스</param>
    /// <param name="subGunIndex">설정할 보조무기 인덱스</param>
    public void SetBeginningPlayer(int modelIndex, int mainGunIndex, int subGunIndex)
    {
        // 캐릭터 모델, 주무기, 보조무기 설정
        displayModel.SetPlayer(modelIndex, mainGunIndex, subGunIndex, new Quaternion(0, 180, 0, 0));
    }

    /// <summary>
    /// 인게임 씬으로 이동
    /// </summary>
    public void OnBattleStartBtn()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 인게임 플레이어에게 적용할 수 있도록 구매한 수류탄, tnt 갯수를 게임 매니저에 전달
        SaveData saveData = GameManager.Instance.DataBase.SaveData;
        saveData.grenadeCount = displayModel.GrenadeCount;
        saveData.tntCount = displayModel.TntCount;

        GameManager.Instance.SceneController.LoadScene(SceneNameConstant.INGAME_SCENE);
    }
}
