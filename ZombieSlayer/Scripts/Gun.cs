using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum BulletStyle : int
    {
        RIFLE = 0,
        SHOTGUN,
        STRAY,
        SNIPE,
        NONE
    }

    const string MUZZLE_EFFECT_FILE_PATH = "Effect/MuzzleEffect";

    [SerializeField] Sprite gunSpirteImage;                     // 총기 이미지
    public Sprite GunSpirteImage
    {
        get => gunSpirteImage;
    }
    [SerializeField] protected BulletStyle bulletStyle;         // 탄알 정의
    public BulletStyle BulletType
    {
        get => bulletStyle;
    }
    [SerializeField] protected Transform firePoint;             // 총알 발사 위치
    [SerializeField] protected Transform emptyShellPoint;       // 탄피 배출 지점
    [SerializeField] protected int maxBulletCount;              // 최대 탄알 장전 수
    public int MaxBulletCount
    {
        get => maxBulletCount;
        set => maxBulletCount = value;
    }
    [SerializeField] protected int remainBulletCount;           // 남은 탄알 수
    public int RemainBulletCount
    {
        get => remainBulletCount;
    }

    [SerializeField] protected int reloadableBulletCount; // 장전할 수 있는 남은 탄알 수
    public int ReloadableBulletCount
    {
        get => reloadableBulletCount;
        set => reloadableBulletCount = value;
    }

    [SerializeField] string gunName;                            // 총이름
    public string GunName
    {
        get => gunName;
    }

    [SerializeField] float muzzleVelocity;                      // 연사 속도
    public float MuzzleVelocity
    {
        get => muzzleVelocity;
        set => muzzleVelocity = value;
    }

    [SerializeField] float reloadTime;                          // 재장전 시간
    public float ReloadTime
    {
        get => reloadTime;
        set => reloadTime = value;
    }

    [SerializeField] protected float rangeOfShot;               // 사정거리
    public float RangeOfShot
    {
        get => rangeOfShot;
        set => rangeOfShot = value;
    }

    [SerializeField] int damage;                                // 탄알 데미지
    public int Damage
    {
        get => damage;
        set => damage = value;
    }

    [SerializeField] int buyableGold;                           // 구매하기 위한 가격
    public int BuyableGold
    {
        get => buyableGold;
    }

    public int OriginMaxBulletCount { get; set; }               // 기본 최대 탄알 수
    public float OriginReloadTime { get; set; }                 // 기본 재장전 시간
    public float OriginMuzzleVelocity { get; set; }             // 기본 연사속도
    public float OriginRangeOfShot { get; set; }                // 기본 사거리
    public int OriginDamage { get; set; }                       // 기본 데미지

    protected string firePointName = "FirePoint";               // 참조할 발사지점 오브젝트 이름
    protected float lastActionTime;                             // 기능을 수행한 마지막 시간
    protected bool isReload;                                    // 재장전 여부

    InGameSceneManager inGameSceneManager;                      // 인게임 매니저 객체

    private void Awake()
    {
        // 기본 정보 저장
        SetOriginValue();
    }

    private void Start()
    {
        Initialize();
    }

    /// <summary>
    /// 기본 총 정보 저장
    /// </summary>
    void SetOriginValue()
    {
        OriginMaxBulletCount = maxBulletCount;
        OriginReloadTime = reloadTime;
        OriginMuzzleVelocity = muzzleVelocity;
        OriginRangeOfShot = rangeOfShot;
        OriginDamage = damage;
    }

    public virtual void Initialize()
    {
        // 초반 탄약 장전
        remainBulletCount = maxBulletCount;

        /* 
         * 포지션 설정
         */
        Transform parent = transform.parent;
        HandContainer handContainer = parent.GetChild(0).GetComponent<HandContainer>();
        transform.SetParent(handContainer.Container);
        // 무기 포지션 설정
        transform.localPosition = Vector3.zero;
        transform.localRotation = new Quaternion(0, 0, 0, 0);

        // 발사 지점 설정
        firePoint = handContainer.Container.Find(firePointName);

        // 탄피 배출 지점 설정
        emptyShellPoint = handContainer.Container.Find("EmptyShellPoint");

        // 인게임씬 매니저 캐싱
        inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();
    }

    /// <summary>
    /// 공격 가능 여부 반환
    /// </summary>
    /// <returns></returns>
    public bool IsAttackable()
    {
        if (Time.time - lastActionTime > muzzleVelocity)
            return true;

        return false;
    }

    /// <summary>
    /// 발사 처리
    /// </summary>
    /// <param name="owner">발사한 객체 정보</param>
    public virtual void Fire(Actor owner)
    {
        Bullet newBullet = inGameSceneManager.BulletManager.Generate((int)bulletStyle, firePoint.position);

        if (newBullet != null)
        {
            // 총 종류에 따른 사운드 재생
            GameManager.Instance.SoundManager.PlaySFX(bulletStyle.ToString());

            // 발사 이펙트 생성 함수 실행
            GenerateMuzzleEffect();
            // 발사 처리
            newBullet.Fire(owner, firePoint.forward, damage, firePoint.position, rangeOfShot);
            // 총구 방향에 맞춰 총알 회전
            newBullet.transform.rotation = firePoint.rotation;
            // 남은 총알 감소
            remainBulletCount--;

            // 탄피생성 함수 실행
            GenerateEmptyShell((int)bulletStyle, owner);

            lastActionTime = Time.time;
        }
    }

    /// <summary>
    /// 탄피 생성 함수
    /// </summary>
    /// <param name="bulletIndex">생성할 탄알 종류</param>
    /// <param name="Actor">탄알을 쏜 객체 정보</param>
    /// <param name="emptyShellSound">지정할 탄피 사운드</param>
    protected virtual void GenerateEmptyShell(int bulletIndex, Actor owner, string emptyShellSound = null)
    {
        // 탄피생성
        EmptyShell emptyShell = inGameSceneManager.EmptyShellManager.Generate(bulletIndex, emptyShellPoint.position);
        if (emptyShell != null)
        {
            // 지정된 탄피 효과음이 있는지 확인
            string tempSound;
            // 기본 사운드 재생
            if (emptyShellSound == null)
                tempSound = AudioNameConstant.EMPTY_SHELL_SOUND;
            else // 원하는 사운드 재생
                tempSound = emptyShellSound;

            // 탄피 효과음 재생
            GameManager.Instance.SoundManager.PlaySFX(tempSound);

            // 총구 방향에 맞춰 탄피 회전
            emptyShell.transform.rotation = emptyShellPoint.transform.rotation;
            // 탄피 배출 처리 함수 실행
            emptyShell.AddForece(emptyShellPoint);
        }
    }

    /// <summary>
    /// 발사 이펙트 생성
    /// </summary>
    protected void GenerateMuzzleEffect()
    {
        // 발사 이펙트 표시
        GameObject muzzleEffect = inGameSceneManager.EffectManager.Generate(MUZZLE_EFFECT_FILE_PATH, firePoint.position);
        if (muzzleEffect)
            muzzleEffect.transform.rotation = firePoint.rotation;
    }

    /// <summary>
    /// 재장전해야 하는지 검사
    /// </summary>
    /// <returns>재장전 여부</returns>
    public virtual bool CheckReload()
    {
        // 재장전해야 하는지 검사
        if(remainBulletCount <= 0 && reloadableBulletCount >= 0)
        {
            return true;
        }

        return false;
    }

    /// <summary>
    /// 재장전 트리거 실행
    /// </summary>
    /// <returns>재장전 여부</returns>
    public virtual bool OnReload()
    {
        // 총알을 한발이라도 발사했고 재장전해야되는 상황인지 검사
        if(remainBulletCount < maxBulletCount && !isReload && reloadableBulletCount > 0)
        {
            lastActionTime = Time.time;
            isReload = true;
            return true;
        }

        return false;
    }

    /// <summary>
    /// 재장전 처리
    /// </summary>
    /// <returns>재장전 완료 여부</returns>
    public virtual bool Reload()
    {
        // 재장전 처리가 진행중이 아니라면 리턴
        if (!isReload)
            return false;

        // 리로드 UI 노출
        InGamePanel inGamePanel = PanelManager.GetPanel(typeof(InGamePanel)) as InGamePanel;
        inGamePanel.UpdateReloadBar(Time.time - lastActionTime, reloadTime);

        // 재장전 시간 경과 후
        if(Time.time - lastActionTime - Player.RELOAD_DELAY_TIME > reloadTime)
        {
            // 장전할 탄알이 남아있다면
            if(reloadableBulletCount > 0)
            {
                // 장전해야할 탄알 계산
                int reloadCount = maxBulletCount - remainBulletCount;

                // 장전해야되는 탄알 수가 재장전 가능한 탄알 수보다 많은지 검사
                if (reloadCount > reloadableBulletCount)
                    reloadCount = reloadableBulletCount;

                // 총알 장전
                reloadableBulletCount -= reloadCount;
                remainBulletCount += reloadCount;

                lastActionTime = Time.time;
                isReload = false;

                return true;
            }
        }

        return false;
    }
}
