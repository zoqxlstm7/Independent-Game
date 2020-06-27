using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 던질 아이템 타입
/// </summary>
public enum ThrowItemType
{
    NONE,
    GRENADE,
    HEALPACK
};

public class ThrowManager : MonoBehaviour
{
    const int GRANADE_LENGTH = 300;                         // 수류탄 범위 고정 넓이
    const int HEAL_PACK_LENGTH = 100;                       // 힐팩 범위 고정 넓이
    Color GRANADE_COLOR = new Color32(255, 0, 0, 100);      // 수류탄 범위 색상
    Color HEAL_PACK_COLOR = new Color32(0, 255, 0, 100);    // 힐팩 범위 색상

    // 캐싱할 파일 정보
    [SerializeField] CacheData[] cacheDatas;
    // 로드된 파일캐시 정보를 저장
    Dictionary<string, GameObject> fileCache = new Dictionary<string, GameObject>();

    ThrowItemType throwItemType;            // 던질 아이템 종류

    //[SerializeField] Image throwRange;      // 던져질때의 범위 표시

    private void Start()
    {
        // 범위 이미지 숨김 처리
        //SetActivatedThrowImage(false);
    }

    private void Update()
    {
        UpdateThrowMove();
    }

    /// <summary>
    /// 범위 이미지 마우스 위치로 이동
    /// </summary>
    void UpdateThrowMove()
    {
        // 선택된 던질 것이 없다면 리턴
        if (throwItemType == ThrowItemType.NONE)
            return;

        // 마우스 지점으로 이동
        //throwRange.gameObject.transform.position = Input.mousePosition;
    }

    /// <summary>
    /// 던질 정보 초기화
    /// </summary>
    /// <param name="newType">던질 아이템 타입</param>
    public void SetThrowInfo(ThrowItemType newType)
    {
        throwItemType = newType;

        // 던질 아이템 타입에 따른 이미지 색상 및 넓이 초기화
        //switch (newType)
        //{
        //    case ThrowItemType.GRENADE:
        //        throwRange.color = GRANADE_COLOR;
        //        throwRange.rectTransform.sizeDelta = new Vector2(GRANADE_LENGTH, GRANADE_LENGTH);
        //        break;
        //    case ThrowItemType.HEALPACK:
        //        throwRange.color = HEAL_PACK_COLOR;
        //        throwRange.rectTransform.sizeDelta = new Vector2(HEAL_PACK_LENGTH, HEAL_PACK_LENGTH);
        //        break;
        //}
    }

    /// <summary>
    /// 범위 이미지 활성/비활성 처리
    /// </summary>
    /// <param name="value">활성/비활성 여부</param>
    //public void SetActivatedThrowImage(bool value)
    //{
    //    if (throwRange == null)
    //        return;

    //    throwRange.gameObject.SetActive(value);
    //}

    /// <summary>
    /// 던지기 처리
    /// </summary>
    /// <param name="endPoint">이동 지점</param>
    /// <param name="generatePosition">생성 지점</param>
    /// <param name="throwItemIndex">생성할 아이템 인덱스</param>
    /// <param name="attacker">공격자 객체</param>
    public void GenerateThrowItem(Vector3 endPoint, Vector3 generatePosition, int throwItemIndex, Actor attacker)
    {
        //Debug.Log(throwItemType + ": Throw");
        // 던질 아이템 생성
        GameObject go = Generate(cacheDatas[throwItemIndex - 1].filePath, generatePosition);
        if(go != null)
        {
            ThrowItem throwItem = go.GetComponent<ThrowItem>();
            if(throwItem != null)
            {
                // 데미지 세팅
                BuffData buffData = GameManager.Instance.BuffManager.GetBuffData(WeaponStyle.GRENADE, BuffType.GrenadeDamage);
                // 레벨을 올렸는지 검사
                if (buffData.currentLevel > 0)
                {
                    Debug.Log("throwItem.OriginValue: " + throwItem.OriginValue);
                    float factor = buffData.currentValue * 0.01f;
                    int value = (int)(throwItem.OriginValue * factor);
                    throwItem.Value = throwItem.OriginValue;
                    throwItem.Value += value;
                }

                // 범위 세팅
                buffData = GameManager.Instance.BuffManager.GetBuffData(WeaponStyle.GRENADE, BuffType.GrenadeRange);
                // 레벨을 올렸는지 검사
                if (buffData.currentLevel > 0)
                {
                    float factor = buffData.currentValue * 0.01f;
                    float value = throwItem.OriginRange * factor;
                    throwItem.Range = throwItem.OriginRange;
                    throwItem.Range += value;
                }

                // 던질 아이템 투척
                throwItem.Throw(endPoint, attacker);
            }
        }
    }

    /// <summary>
    /// 캐싱할 파일 메모리 로드
    /// </summary>
    /// <param name="filePath">캐싱할 오브젝트 파일 경로</param>
    /// <returns>로드된 오브젝트</returns>
    GameObject Load(string filePath)
    {
        GameObject go = null;

        // 생성되지 않았다면 로드
        if (!fileCache.ContainsKey(filePath))
        {
            go = Resources.Load<GameObject>(filePath);
            if (go == null)
            {
                Debug.Log("Load Error! filepath: " + filePath);
                return null;
            }

            fileCache.Add(filePath, go);
        }
        else
        {
            // 생성되있다면 로드된 오브젝트 반환
            return fileCache[filePath];
        }

        return go;
    }

    /// <summary>
    /// 투척 아이템 생성
    /// </summary>
    /// <param name="filePath">생성할 오브젝트 파일 경로</param>
    /// <param name="generatePosition">생성 지점</param>
    /// <returns></returns>
    public GameObject Generate(string filePath, Vector3 generatePosition)
    {
        GameObject go = Load(filePath);

        if(go != null)
        {
            GameObject newGameObject = Instantiate(go, generatePosition, Quaternion.identity);
            if (newGameObject != null)
                return newGameObject;
        }

        return null;
    }
}
