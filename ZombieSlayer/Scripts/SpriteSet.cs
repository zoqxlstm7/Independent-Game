using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 스프라이트 정보 저장 클래스
/// </summary>
[System.Serializable]
public class SpriteData
{
    public string spriteName;   // 검색에 사용할 스프라이트 이름
    public Sprite sprite;       // 스프라이트 이미지
}

public class SpriteSet : MonoBehaviour
{
    public const string BLANK_SPRITE_NAME = "Blank";     // 블랭크 스프라이트 이름

    // 저장할 스프라이트  데이터
    [SerializeField] SpriteData[] spriteDatas;
    [SerializeField] SpriteData[] buffIcons;
    [SerializeField] SpriteData[] weaponIcons;
    // 검색을 빠르게 하기 위한 딕셔너리
    Dictionary<string, Sprite> spriteTable = new Dictionary<string, Sprite>();

    private void Awake()
    {
        // 스프라이트 데이터를 딕셔너리에 등록
        for (int i = 0; i < spriteDatas.Length; i++)
        {
            // 등록되지 않은 경우 딕셔너리에 등록
            if (!spriteTable.ContainsKey(spriteDatas[i].spriteName))
                spriteTable.Add(spriteDatas[i].spriteName, spriteDatas[i].sprite);
        }

        // 버프 아이콘 데이터를 딕셔너리에 등록
        for (int i = 0; i < buffIcons.Length; i++)
        {
            // 등록되지 않은 경우 딕셔너리에 등록
            if (!spriteTable.ContainsKey(buffIcons[i].spriteName))
                spriteTable.Add(buffIcons[i].spriteName, buffIcons[i].sprite);
        }

        // 무기 아이콘 데이터를 딕셔너리에 등록
        for (int i = 0; i < weaponIcons.Length; i++)
        {
            // 등록되지 않은 경우 딕셔너리에 등록
            if (!spriteTable.ContainsKey(weaponIcons[i].spriteName))
                spriteTable.Add(weaponIcons[i].spriteName, weaponIcons[i].sprite);
        }
    }

    /// <summary>
    /// 지정한 스프라이트 이름을 통해 스프라이트 이미지 반환
    /// </summary>
    /// <param name="spriteName">반환받을 스프라이트명</param>
    /// <returns></returns>
    public Sprite GetSprite(string spriteName)
    {
        // 키값이 등록되었는지 확인
        if (!spriteTable.ContainsKey(spriteName))
        {
            Debug.LogWarning("등록된 스프라이트가 없습니다. spriteName: " + spriteName);
            return null;
        }

        return spriteTable[spriteName];
    }
}
