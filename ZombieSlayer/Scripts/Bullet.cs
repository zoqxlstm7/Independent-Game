using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    const float BULLET_LIFE_TIME = 5.0f;        // 총알 생존 시간

    string filePath;                            // 파일 경로
    public string FilePath
    {
        set => filePath = value;
    }

    protected Actor owner;                                // 총을 쏜 주인

    protected int damage;                       // 탄알 데미지
    Vector3 dir;                                // 이동 방향
    protected Vector3 firePoint;                // 발사지점
    protected float rangeOfShot;                // 사정거리

    [SerializeField] float speed;                           // 탄알 속도
    [SerializeField] TrailRenderer bulletTrail;             // 트레일렌더러
    [SerializeField] protected CapsuleCollider collider;    // 콜라이더

    bool isFire;                                // 발사되었는지 여부
    bool isThrough;                             // 관통여부

    private void OnEnable()
    {
        //사용될 때 초기값 설정
        ResetForAlive();
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateBullet();
    }

    public void Initialize()
    {
    }

    public void UpdateBullet()
    {
        UpdateMove();
        CheckBulletLifeTime();
    }

    /// <summary>
    /// 탄알 이동 처리
    /// </summary>
    void UpdateMove()
    {
        if (!isFire)
            return;

        Vector3 moveVector = dir * speed * Time.deltaTime;
        transform.position += AdjustMove(moveVector);
    }

    /// <summary>
    /// 총알의 속도가 빠를 때를 대비한 이동벡터 보정 함수
    /// </summary>
    /// <param name="moveVector">보정할 이동벡터</param>
    /// <returns></returns>
    Vector3 AdjustMove(Vector3 moveVector)
    {
        //if (isThrough)
        //    return moveVector;

        RaycastHit hit;

        // 현재 위치에서 다음 이동할 위치에 충돌체가 있는지 확인
        if (Physics.Linecast(transform.position, transform.position + moveVector, out hit))
        {
            Actor targetActor = hit.collider.gameObject.GetComponent<Actor>();

            // Actor 클래스를 상속한 객체라면
            if (targetActor != null)
            {
                // Actor클래스를 상속한 객체 중 레이어가 다른 오브젝트끼리 충돌 처리
                if (owner.gameObject.layer != targetActor.gameObject.layer)
                {
                    moveVector = hit.point - transform.position;
                    OnBulletCollision(targetActor);
                }
            }
            else
            {
                // 다른 충돌체일 때 처리 함수
                OnBulletOtherCollision(hit.collider.gameObject);
            }
        }

        return moveVector;
    }

    /// <summary>
    /// 발사 처리
    /// </summary>
    /// <param name="owner">발사한 객체 정보</param>
    /// <param name="dir">발사 방향</param>
    /// <param name="damage">탄알 데미지</param>
    public void Fire(Actor owner, Vector3 dir, int damage, Vector3 firePoint, float rangeOfShot, bool isThrough = false)
    {
        this.owner = owner;
        this.dir = dir;
        this.damage = damage;
        this.firePoint = firePoint;
        this.rangeOfShot = rangeOfShot;
        this.isThrough = isThrough;

        isFire = true;
    }

    /// <summary>
    /// 총알이 발사되고 난 후 생명주기를 체크
    /// </summary>
    protected virtual void CheckBulletLifeTime()
    {
        // 발사 후 사정거리만큼 이동 후 총알 삭제
        float distance = Vector3.Distance(transform.position, firePoint);
        if (distance > rangeOfShot)
        {
            DestroyBullet();
        }
    }

    /// <summary>
    /// 탄알 오브젝트 삭제
    /// </summary>
    protected void DestroyBullet()
    {
        isFire = false;
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BulletManager.Remove(filePath, gameObject);
    }

    /// <summary>
    /// 적이 아닌 다른 충돌체일 때의 처리 함수
    /// </summary>
    public virtual void OnBulletOtherCollision(GameObject gameObject)
    {
        // 탄알 구멍 사운드 재생
        AudioSource audio = gameObject.GetComponent<AudioSource>(); ;
        // 오디오 소스가 없으면 생성하여 재생
        if (audio == null)
        {
            AudioSource newAudio = gameObject.AddComponent<AudioSource>();
            newAudio.playOnAwake = false;
            newAudio.clip = GameManager.Instance.SoundManager.GetAudioClip(AudioNameConstant.BULLET_HOLE_SOUND);
            newAudio.Play();
        }
        else // 있다면 재생
        {
            audio.clip = GameManager.Instance.SoundManager.GetAudioClip(AudioNameConstant.BULLET_HOLE_SOUND);
            audio.Play();
        }
        
        // 총 종류에 따른 사운드 재생
        //GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BULLET_HOLE_SOUND);

        // 탄알 구멍 이펙트 생성
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(EffectManager.BULLET_HOLE_EFFECT_FILE_PATH
            , transform.position);

        // 콜라이더 비활성화
        collider.enabled = false;
        // 충돌한 탄알 제거
        DestroyBullet();
    }

    /// <summary>
    /// 총알 충돌 처리
    /// </summary>
    /// <param name="targetActor">충돌된 객체</param>
    public virtual void OnBulletCollision(Actor targetActor)
    {
        if (owner == null)
            return;

        // Actor클래스를 상속한 객체 중 레이어가 다른 오브젝트끼리 충돌 처리
        if(owner.gameObject.layer != targetActor.gameObject.layer)
        {
            // 데미지 처리
            targetActor.OnTakeHit(damage, owner);

            // 관통력이 없는 총알인 경우
            if (!isThrough)
            {
                // 콜라이더 비활성화
                collider.enabled = false;
                // 충돌한 탄알 제거
                DestroyBullet();
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Actor 클래스를 상속한 오브젝트만 충돌 처리
        Actor target = other.GetComponent<Actor>();

        if (target)
            OnBulletCollision(target);
    }

    /// <summary>
    /// 살아났을 때의 초기화 처리
    /// </summary>
    public void ResetForAlive()
    {
        // 트레일 렌더러 초기화
        bulletTrail.Clear();
        // 콜라이더 활성화
        collider.enabled = true;
    }
}
