using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss : Enemy
{
    [SerializeField] string bossName;   // 보스 이름
    public string BossName
    {
        get => bossName;
    }

    public override void ResetForAlive()
    {
        base.ResetForAlive();

        // 팩터 계산
        SpawnManager spawnManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().SpawnManager;
        int factor = spawnManager.Wave.stageCount / spawnManager.Wave.bossSpawnFactor;

        // 체력 및 공격력 재설정
        maxHp += 400 * factor;
        currentHp = maxHp;
        damage += factor * 2;
    }

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

    /// <summary>
    /// 공격 처리 함수
    /// </summary>
    /// <param name="damage">가할 데미지</param>
    public override void OnAttack(int damage)
    {
        Animator.PlayAnimator("MeleeAttack");

        StartCoroutine(AttackDelay());

        if (hitPerson != null)
        {
            hitPerson.OnTakeHit(damage, this);
            hitPerson = null;
        }
    }

    public override void OnDead()
    {
        Animator.PlayAnimator("Die");

        // 보스 데스 효과음 재생
        AudioClip audioClip = GameManager.Instance.SoundManager.GetAudioClip(AudioNameConstant.Zombie_Death_SOUND);
        if (audioClip != null)
        {
            audioSource.clip = audioClip;
            audioSource.Play();
        }
        //GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.Zombie_Death_SOUND);

        // 길찾기 중지 및 콜라이더 비활성화
        agent.isStopped = true;
        capsuleCollider.enabled = false;

        // 킬카운트 증가
        attacker.KillCount++;

        // 경험치 증가
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().GameAccumulator.Exp = exp;
        // 경험치 UI 업데이트
        InGamePanel inGamePanel = PanelManager.GetPanel(typeof(InGamePanel)) as InGamePanel;
        inGamePanel.UpdateExpBar();

        InGameSceneManager inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();

        StartCoroutine(DropItem(inGameSceneManager, 10));

        // 웨이브를 모두 클리어했는지 확인
        inGameSceneManager.SpawnManager.OnCheckWaveClear();

        // 사망 딜레이 함수 호출
        StartCoroutine(DeadDelay());
    }

    IEnumerator DropItem(InGameSceneManager inGameSceneManager, int count)
    {
        for (int i = 0; i < count; i++)
        {
            yield return null;

            int ran = Random.Range(0, System.Enum.GetNames(typeof(AcquireType)).Length);
            // 아이템 드랍 실행
            inGameSceneManager.ItemDropManager.TakeItem((AcquireType)ran, target.GetComponent<Player>());

            yield return new WaitForSeconds(0.1f);
        }
    }

    /// <summary>
    /// 죽은 후 딜레이 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator DeadDelay()
    {
        yield return new WaitForSeconds(REMOVE_TIME);
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BossManager.Remove(filePath, gameObject);

        // 보스 패널 숨김
        BossPanel bossPanel = PanelManager.GetPanel(typeof(BossPanel)) as BossPanel;
        bossPanel.Close();
    }
}
