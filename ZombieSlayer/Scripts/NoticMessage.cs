using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NoticMessage : MonoBehaviour
{
    [SerializeField] Text noticText;    // 알림 텍스트

    bool isNotic;                       // 알리는 중인지 여부
    float lastActionTime;               // 마지막 시간 저장

    private void Start()
    {
        gameObject.SetActive(false);
    }

    private void Update()
    {
        UpdateNotic();
    }

    /// <summary>
    /// 알림 UI 노출
    /// </summary>
    void UpdateNotic()
    {
        if (!isNotic)
            return;

        // 지정된 시간동안 노출
        if (Time.time - lastActionTime >= WavePanel.NOTIC_SHOW_TIME)
        {
            isNotic = false;
            gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// 메세지 설정 후 알림 UI 노출
    /// </summary>
    /// <param name="msg"></param>
    public void ShowNotic(string msg)
    {
        gameObject.SetActive(true);
        noticText.text = msg;

        isNotic = true;
        lastActionTime = Time.time;
    }
}
