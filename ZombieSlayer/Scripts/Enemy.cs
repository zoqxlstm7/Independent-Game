using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
[RequireComponent(typeof(AudioSource))]
public class Enemy : Actor
{
    protected const float REMOVE_TIME = 3.0f;       // 제거될 때까지 걸리는 시간

    [SerializeField] protected MotionState state;   // 모션 상태 변수

    [SerializeField] protected Transform target;                // 타겟 객체
    [SerializeField] protected NavMeshAgent agent;              // Nav Mesh 객체
    [SerializeField] protected CapsuleCollider capsuleCollider; // 콜라이더 컴포넌트
    [SerializeField] protected AudioSource audioSource;         // 오디오 소스 컴포넌트

    [SerializeField] protected float attackRange;   // 공격 범위
    [SerializeField] float attackSpeed;             // 공격 속도
    [SerializeField] protected int damage;          // 데미지
    [SerializeField] protected int exp;             // 잡았을 때의 획득 경험치
        
    float lastActionTime;                           // 마지막 액션 시간
    bool isEmerge;                                  // 땅에서 나오는지 여부

    protected string filePath;                      // 파일 경로
    public string FilePath
    {
        set => filePath = value;
    }

    protected Actor hitPerson;                      // 공격을 당한 대상

    private void OnEnable()
    {
        ResetForAlive();
    }

    /// <summary>
    /// 초기화 처리
    /// </summary>
    protected override void InitializeActor()
    {
        base.InitializeActor();

        // nav mesh 속도값을 지정한 속도값으로 지정
        agent.speed = speed;
        // 타겟 지정
        target = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player.transform;
    }

    /// <summary>
    /// 업데이트 처리
    /// </summary>
    protected override void UpdateActor()
    {
        // 사망 시 리턴
        if (isDead)
            return;

        InGameSceneManager inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();
        // 게임 오버시 리턴
        if (inGameSceneManager.IsGameOver)
            return;

        // 플레이어 사망시 리턴
        if (inGameSceneManager.Player.IsDead)
        {
            agent.SetDestination(transform.position);
            Animator.PlayAnimator("Run", false);
            return;
        }

        if (isEmerge)
            return;

        base.UpdateActor();

        UpdateMove();
        CheckAttackRange();
    }

    /// <summary>
    /// 움직임 처리
    /// </summary>
    public virtual void UpdateMove()
    {
        // 속도를 체크하여 애니메이션 재생
        if (agent.velocity != Vector3.zero)
            Animator.PlayAnimator("Run", true);
        else
            Animator.PlayAnimator("Run", false);

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
    /// 공격 범위내에 적이 있는지 확인
    /// </summary>
    void CheckAttackRange()
    {
        if (target == null)
            return;

        // 거리 확인
        float distance = Vector3.Distance(transform.position, target.position);

        if (distance <= attackRange)
        {
            // 범위 내에 있을 시 대상을 주시
            TargetToFace(target.position);

            // 공격 속도에 맞춰 공격 실행
            if (Time.time - lastActionTime >= attackSpeed)
            {
                hitPerson = target.GetComponent<Actor>();

                state = MotionState.ATTACK;
                OnAttack(damage);
                lastActionTime = Time.time;
            }
        }
    }

    /// <summary>
    /// 타겟지점을 바라보도록 처리하는 함수
    /// </summary>
    /// <param name="targetPos">바라볼 지점</param>
    void TargetToFace(Vector3 targetPos)
    {
        Vector3 dir = (targetPos - transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(dir.x, 0, dir.z));
        transform.rotation = lookRotation;
    }

    /// <summary>
    /// 공격 처리 함수
    /// </summary>
    /// <param name="damage">가할 데미지</param>
    public override void OnAttack(int damage)
    {
        base.OnAttack(damage);

        StartCoroutine(AttackDelay());

        if(hitPerson != null)
        {
            hitPerson.OnTakeHit(damage, this);
            hitPerson = null;
        }
    }

    /// <summary>
    /// 공격 후 딜레이 처리
    /// </summary>
    /// <returns></returns>
    protected IEnumerator AttackDelay()
    {
        yield return new WaitForSeconds(2.0f);
        state = MotionState.NONE;
    }

    /// <summary>
    /// 죽었을 때의 처리
    /// </summary>
    public override void OnDead()
    {
        base.OnDead();

        // 좀비 데스 효과음 재생
        AudioClip audioClip = GameManager.Instance.SoundManager.GetAudioClip(AudioNameConstant.Zombie_Death_SOUND);
        if(audioClip != null)
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

        // 아이템 드랍 실행
        inGameSceneManager.ItemDropManager.DropTale(attacker);

        // 웨이브를 모두 클리어했는지 확인
        inGameSceneManager.SpawnManager.OnCheckWaveClear();

        // 사망 딜레이 함수 호출
        StartCoroutine(DeadDelay());
    }

    /// <summary>
    /// 죽은 후 딜레이 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator DeadDelay()
    {
        yield return new WaitForSeconds(REMOVE_TIME);
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EnemyManager.Remove(filePath, this);
    }

    /// <summary>
    /// 살아났을 때의 초기화 처리
    /// </summary>
    public virtual void ResetForAlive()
    {
        isDead = false;
        capsuleCollider.enabled = true;
        currentHp = maxHp;

        StartCoroutine(EmergeDelay());
    }

    /// <summary>
    /// 땅에서 나오는 모션 처리 함수
    /// </summary>
    /// <returns></returns>
    IEnumerator EmergeDelay()
    {
        // 땅에서 나오는 처리
        isEmerge = true;
        Animator.PlayAnimator("Emerge");
        yield return new WaitForSeconds(3f);
        // 땅에서 나온 후 업데이트문 진행
        isEmerge = false;
    }

    /// <summary>
    /// 공격 범위 기즈모 표시
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
    }
}
