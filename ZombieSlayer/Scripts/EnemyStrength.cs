using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStrength : Enemy
{
    /// <summary>
    /// 움직임 처리
    /// </summary>
    public override void UpdateMove()
    {
        // 속도를 체크하여 애니메이션 재생
        if (agent.velocity != Vector3.zero)
            Animator.PlayAnimator("SpeedRun", true);
        else
            Animator.PlayAnimator("SpeedRun", false);

        // None이 아닌 상태일 경우 리턴
        if (state != MotionState.NONE)
            return;

        // 타겟이 없는 경우 리턴
        if (target == null)
            return;

        // 타겟지점으로 이동
        agent.SetDestination(target.position);
    }
}
