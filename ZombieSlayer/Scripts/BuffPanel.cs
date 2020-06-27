using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 버프 슬롯 정보를 저장할 클래스
/// </summary>
[System.Serializable]
public class BuffSlot
{
    public Image buffIcon;      // 버프 아이콘 이미지
    public Image weaponIcon;    // 무기 아이콘 이미지
    public Text levelText;      // 레벨 텍스트
    public Text descriptText;   // 설명 텍스트
}

public class BuffPanel : BasePanel
{
    [SerializeField] BuffSlot[] buffSlots;  // 버프 슬롯 배열
    [SerializeField] BuffData[] selectBuffData = new BuffData[3];

    public override void InitializePanel()
    {
        base.InitializePanel();

        Close();
    }

    /// <summary>
    /// 패널 노출
    /// </summary>
    public override void Show()
    {
        base.Show();

        StringBuilder sb = new StringBuilder();

        BuffData[] buffdata = GameManager.Instance.BuffManager.GetRandomBuffData(buffSlots.Length);
        // 받은 데이터가 없다면 리턴
        if (buffdata == null)
            return;

        // 버프 데이터를 반환 받아 정보 노출
        for (int i = 0; i < buffdata.Length; i++)
        {
            // 무기, 버프 아이콘 표시
            buffSlots[i].buffIcon.sprite = buffdata[i].buffIcon;
            buffSlots[i].weaponIcon.sprite = buffdata[i].weaponIcon;

            // 버프 레벨 표시
            sb.AppendFormat("Lv. {0}", buffdata[i].currentLevel);
            buffSlots[i].levelText.text = sb.ToString();
            sb.Clear();

            // 버프 설명 표시
            sb.AppendFormat("{0} (+{1}%)", buffdata[i].Descript, buffdata[i].maxValue / buffdata[i].maxLevel);
            buffSlots[i].descriptText.text = sb.ToString();
            sb.Clear();

            // 스롯에 노출된 버프 데이터 저장
            selectBuffData[i] = buffdata[i];
        }
    }

    /// <summary>
    /// 선택된 버프 처리 함수
    /// </summary>
    /// <param name="index">선택된 버프 인덱스</param>
    public void OnSelectBuffSlot(int index)
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        // 버프 업그레이드 처리
        GameManager.Instance.BuffManager.UpgradeBuffData(selectBuffData[index]);

        // 타임 스켈일 정상화
        Time.timeScale = 1;
        Close();
    }
}
