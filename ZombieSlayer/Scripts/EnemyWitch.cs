using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyWitch : Enemy
{
    const float THROW_DELAY_TIME = 0.5f;        // 마법시전 후 딜레이 시간

    /// <summary>
    /// 움직임 처리
    /// </summary>
    public override void UpdateMove()
    {
        base.UpdateMove();

        // 공격 범위 안에 있다면 리턴
        if (AttackRangeInTarget())
        {
            agent.SetDestination(transform.position);
            return;
        }
    }

    /// <summary>
    /// 공격 범위안에 타겟이 있는지 확인
    /// </summary>
    /// <returns></returns>
    bool AttackRangeInTarget()
    {
        // 거리 확인
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 공격 처리 함수
    /// </summary>
    /// <param name="damage">가할 데미지</param>
    public override void OnAttack(int damage)
    {
        Animator.PlayAnimator("Throw");

        StartCoroutine(ThrowDelay(/*target.position, */damage));

        StartCoroutine(AttackDelay());
    }

    /// <summary>
    /// 딜레이 후 마법 시전
    /// </summary>
    /// <param name="targetPos">타겟 위치</param>
    /// <returns></returns>
    IEnumerator ThrowDelay(/*Vector3 targetPos, */int damage)
    {
        yield return new WaitForSeconds(THROW_DELAY_TIME);
        GameObject go = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(EffectManager.VENOM_SPEAR_FILE_PATH, target.transform.position/*targetPos*/);
        if(go != null)
        {
            ContinuousDamage continuousDamage = go.GetComponent<ContinuousDamage>();
            if(continuousDamage != null)
            {
                continuousDamage.Set(damage, this);
            }
        }
    }
}
