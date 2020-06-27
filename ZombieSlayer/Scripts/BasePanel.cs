using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasePanel : MonoBehaviour
{
    private void Awake()
    {
        // 패널관리자에서 패널 등록
        PanelManager.Regist(GetType(), this);
    }

    private void Start()
    {
        InitializePanel();
    }

    private void Update()
    {
        UpdatePanel();
    }

    /// <summary>
    /// 파괴될 때 호출
    /// </summary>
    private void OnDestroy()
    {
        DestroyPanel();
    }

    /// <summary>
    /// 패널 초기화
    /// </summary>
    public virtual void InitializePanel()
    {
        
    }

    /// <summary>
    /// 패널 업데이트 처리
    /// </summary>
    public virtual void UpdatePanel()
    {
    }

    /// <summary>
    /// 패널 파괴
    /// </summary>
    public virtual void DestroyPanel()
    {
        // 패널관리자에서 패널 등록 해제
        PanelManager.UnRegist(GetType());
    }

    /// <summary>
    /// 패널 노출
    /// </summary>
    public virtual void Show()
    {
        gameObject.SetActive(true);
    }

    /// <summary>
    /// 패널 숨김
    /// </summary>
    public virtual void Close()
    {
        gameObject.SetActive(false);
    }
}
