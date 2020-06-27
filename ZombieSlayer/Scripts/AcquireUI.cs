using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AcquireUI : MonoBehaviour
{
    const float DISPLAY_TIME = 1f;          // 표시 시간
    const float OFFSET_Y = 3f;              // 오프셋 Y

    [SerializeField] Image icon;            // 표시할 스트라이트 아이콘
    public Sprite Icon
    {
        set => icon.sprite = value;
    }

    [SerializeField] Text valueText;        // 표시될 값 텍스트    

    int uiIndex;                            // 노출된 UI인덱스
    public int UiIndex
    {
        set => uiIndex = value;
    }

    Transform target;                       // 정보 UI가 표시될 타겟 객체
    float speed = 2f;                       // 움직일 속도
    float changedOffsetY;                   // 변화되는 Y값을 저장할 객체

    bool isMove;                            // 움직일지 여부
    float startTime;                        // 움직이기 시작한 시간

    private void Update()
    {
        UpdateMove();
    }

    /// <summary>
    /// 움직임 업데이트
    /// </summary>
    void UpdateMove()
    {
        // 움직임이 필요없는 경우 리턴
        if (!isMove)
            return;

        // 노출시간이 다한경우 제거
        if(Time.time - startTime > DISPLAY_TIME)
            RemoveInfoUI();

        // 타겟 객체의 위치를 screen point 좌표로 변경
        Vector3 pos = Camera.main.WorldToScreenPoint(new Vector3(target.position.x,
                                                                target.position.y + changedOffsetY,
                                                                target.position.z));
        pos.z = 0;

        // 위로 움직일 수 있도록 y값 변경
        changedOffsetY += Time.deltaTime * speed;
        transform.position = pos;
    }

    /// <summary>
    /// 획득 UI 제거
    /// </summary>
    void RemoveInfoUI()
    {
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().AcquireUIManager.Remove(uiIndex);
        isMove = false;
    }

    /// <summary>
    /// 획득 UI 초기화
    /// </summary>
    /// <param name="value">입력될 값</param>
    /// <param name="target">타겟 객체</param>
    public void SetInfo(int value, Transform target)
    {
        this.target = target;
        changedOffsetY = OFFSET_Y;

        // 값 표시
        valueText.text = "+" + value;

        // 시작 시간 저장
        startTime = Time.time;
        // 움직임 활성화
        isMove = true;
    }
}
