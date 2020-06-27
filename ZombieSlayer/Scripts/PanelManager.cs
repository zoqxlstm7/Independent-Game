using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelManager : MonoBehaviour
{
    // 등록된 패널을 관리할 객체
    static Dictionary<System.Type, BasePanel> panels = new Dictionary<System.Type, BasePanel>();

    /// <summary>
    /// 패널 등록
    /// </summary>
    /// <param name="panelType">등록할 패널 객체 타입</param>
    /// <param name="panel">등록할 패널</param>
    public static void Regist(System.Type panelType, BasePanel panel)
    {
        // 등록되어있지 않다면 등록
        if (!panels.ContainsKey(panelType))
        {
            panels.Add(panelType, panel);
        }
    }

    /// <summary>
    /// 패널 등록 해제
    /// </summary>
    /// <param name="panelType">등록 해제할 패널 객체 타입</param>
    public static void UnRegist(System.Type panelType)
    {
        // 등록되어있는 패널이라면 등록 해제
        if (panels.ContainsKey(panelType))
        {
            panels.Remove(panelType);
        }
    }

    /// <summary>
    /// 등록된 패널 객체 반환
    /// </summary>
    /// <param name="panelType">반환받을 패널 객체 타입</param>
    /// <returns></returns>
    public static BasePanel GetPanel(System.Type panelType)
    {
        // 등록되어있는 패널이라면 반환
        if (panels.ContainsKey(panelType))
        {
            return panels[panelType];
        }

        return null;
    }
}
