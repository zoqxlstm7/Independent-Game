using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TitlePanel : BasePanel
{
    [SerializeField] Text anyKeyText;   // 아무키나 누르시오 텍스트

    float originAlpha;                  // 아무키 텍스트 기존 알파값

    public override void InitializePanel()
    {
        base.InitializePanel();

        // 아무키 텍스트 기존 알파값 저장
        originAlpha = anyKeyText.color.a;
    }

    public override void UpdatePanel()
    {
        base.UpdatePanel();

        PingPongAlpha();
    }

    /// <summary>
    /// 알파값 핑퐁 처리 함수
    /// </summary>
    void PingPongAlpha()
    {
        Color color = anyKeyText.color;
        color.a = Mathf.PingPong(Time.time * 0.8f, originAlpha);
        anyKeyText.color = color;
    }
}
