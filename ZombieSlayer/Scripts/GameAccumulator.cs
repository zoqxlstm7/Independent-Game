using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameAccumulator : MonoBehaviour
{
    const int ADDED_EXP = 20;           // 추가 경험치
    const int MAX_LEVEL = 20;           // 최대 레벨
    const float DELAY_INTERVAL = 0.5f;  // 딜레이 시간 간격

    [SerializeField] int exp;   // 플레이어 획득 경험치
    public int Exp
    {
        get
        {
            return exp;
        }
        set
        {
            // 플레이어 사망 시 리턴
            if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player.IsDead)
                return;

            // 최대 레벨 도달 시 리턴
            if (Level >= MAX_LEVEL)
                return;

            exp += value;

            // 획득경험치가 최대 경험치보다 큰 겨웅
            if (MaxExp <= exp)
            {
                // 남은 경험치를 계산하고 최대 경험치 증가
                int num = exp - MaxExp;
                MaxExp += ADDED_EXP;
                exp = num;

                Level += 1;

                // 레벨업시 체력 회복
                Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player;
                player.CurrentHP = player.MaxHP;

                // 버프 패널 노출
                StartCoroutine(LevelUpDelay());
            }
        }
    }
    public int MaxExp { get; set; } = 20;   // 최대 경험치
    public int Gold { get; set; }           // 획득 골드
    public int Level { get; set; } = 1;     // 플레이어 레벨

    /// <summary>
    /// 레벨업 시 딜레이 후 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator LevelUpDelay()
    {
        yield return new WaitForSeconds(DELAY_INTERVAL);
        // 시간 정지 후 버프 패널 노출
        Time.timeScale = 0f;
        BuffPanel buffPanel = PanelManager.GetPanel(typeof(BuffPanel)) as BuffPanel;
        buffPanel.Show();
    }
}
