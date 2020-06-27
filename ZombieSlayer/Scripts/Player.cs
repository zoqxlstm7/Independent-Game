using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 무기 상태
/// </summary>
public enum WeaponStyle
{
    WEAPON,
    PISTOL,
    THROW,
    GRENADE,
    TNT,
    MELEE
}

public class Player : Actor
{
    public const float RELOAD_DELAY_TIME = 1f;  // 리로드 후 딜레이 시간
    const float THROW_DELAY_TIME = 0.5f;        // 던진 후 딜레이 시간
    const float RECOVERY_TIME_INTERVAL = 10f;   // 체력 회복 시간 간격
    const float EMPTY_GUN_SOUND_INTERVAL = 0.5f;// 빈탄창 효과음 시간 간격

    int throwItemIndex;                         // 던질 아이템 인덱스

    [SerializeField] LayerMask groundMask;      // 그라운드 레이어 마스크
    [SerializeField] MotionState state;
    public MotionState State                    // 상태 객체 반환
    {
        get => state;
    }
    [SerializeField] WeaponStyle weaponState;
    public WeaponStyle WeaponState              // 무기상태 객체 반환
    {
        get => weaponState;
    }

    [SerializeField] Gun equipGun;              // 장착 총
    public Gun EquipGun
    {
        get => equipGun;
        set => equipGun = value;
    }

    [SerializeField] int grenadeCount;          // 보유중인 수류탄 갯수
    public int GrenadeCount
    {
        get => grenadeCount;
        set => grenadeCount = value;
    }

    [SerializeField] int tntCount;              // 보유중인 tnt 갯수
    public int TntCount
    {
        get => tntCount;
        set => tntCount = value;
    }

    [SerializeField] float recoveryHP;          // 10초당 회복량
    public float RecoveryHP
    {
        get => recoveryHP;
        set => recoveryHP = value;
    }

    [SerializeField] float decreaseDamage;      // 데미지 감소
    public float DecreasseDamage
    {
        get => decreaseDamage;
        set => decreaseDamage = value;
    }

    [SerializeField] Avatar avatar;             // 애니메이션 적용에 쓰일 아바타 

    GameObject model;                           // 선택 모델
    public GameObject Model
    {
        get => model;
    }

    Gun mainGun;                                // 선택 주무기 모델
    public Gun MainGun
    {
        get => mainGun;
        set => mainGun = value;
    }
    Gun subGun;                                 // 선택 보조무기 모델
    public Gun SubGun
    {
        get => subGun;
        set => subGun = value;
    }

    public float OriginMaxHp { get; set; }
    public float OriginRecoveryHp { get; set; }
    public float OriginMoveSpeed { get; set; }
    public float OriginDecreasseDamage { get; set; }

    float lastRecoveryTime;
    float lastActionTime;

    /// <summary>
    /// 기본 총 정보 저장
    /// </summary>
    void SetOriginValue()
    {
        OriginMaxHp = maxHp;
        OriginRecoveryHp = recoveryHP;
        OriginMoveSpeed = speed;
        OriginDecreasseDamage = decreaseDamage;
    }

    protected override void InitializeActor()
    {
        base.InitializeActor();

        // 기본 정보 저장
        SetOriginValue();

        // 선택한 모델, 주무기, 보조무기 초기화
        SaveData saveData = GameManager.Instance.DataBase.SaveData;
        SetPlayer(saveData.modelIndex, saveData.mainGunIndex, saveData.subGunIndex, Quaternion.identity);

        // 수류탄 갯수 초기화
        grenadeCount = saveData.grenadeCount;
        // tnt 갯수 초기화
        TntCount = saveData.tntCount;

        // 캐릭터 모델을 구매한 경우 최대 체력 증가
        if (saveData.modelIndex > 0)
        {
            maxHp += 50;
            currentHp = maxHp;

            OriginMaxHp = maxHp;
        }

        // 장착 슬롯 초기화
        EquipSlotPanel equipSlotPanel = PanelManager.GetPanel(typeof(EquipSlotPanel)) as EquipSlotPanel;
        equipSlotPanel.ChangedEquipSlot(this);

        // 구입한 버프 적용
        for (int i = 0; i < saveData.buffDatas.Count; i++)
        {
            BuffData buffData = GameManager.Instance.BuffManager.GetBuffData(saveData.buffDatas[i].weaponStyle, saveData.buffDatas[i].buffType);
            GameManager.Instance.BuffManager.UpgradeBuffData(buffData);
        }
    }

    protected override void UpdateActor()
    {
        // 인게임씬이 아니라면 리턴
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>() == null)
            return;

        // 게임 오버시 리턴
        if (GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().IsGameOver)
        {
            Animator.PlayAnimator(AnimationConstantName.RUN, false);
            Animator.PlayAnimator(AnimationConstantName.SHOOT, false);
            Animator.PlayAnimator(AnimationConstantName.RELOAD, false);
            return;
        }

        base.UpdateActor();

        // 재장전이 완료되었는지 확인
        if (equipGun.Reload())
        {
            // 유탄 발사기인 경우 리로드 사운드 변경
            if (equipGun.BulletType == Gun.BulletStyle.STRAY)
                ReloadComplete(AudioNameConstant.LAUNCHER_RELOAD_SOUND);
            else
                ReloadComplete(AudioNameConstant.RELOAD_SOUND);
        }

        // 체력 회복 업데이트
        UpdateRecoveryHp();
    }

    /// <summary>
    /// 10초당 체력 회복 루틴
    /// </summary>
    void UpdateRecoveryHp()
    {
        // 사망 시 리턴
        if (isDead)
            return;

        if (Time.time - lastRecoveryTime >= RECOVERY_TIME_INTERVAL)
        {
            currentHp += recoveryHP;

            if (currentHp > maxHp)
                currentHp = maxHp;

            lastRecoveryTime = Time.time;
        }
    }

    /// <summary>
    /// 움직임 처리
    /// </summary>
    /// <param name="v">수직 입력값</param>
    /// <param name="h">수평 입력값</param>
    public void UpdateMove(float v, float h)
    {
        // none 상태가 아니면 리턴
        if (state != MotionState.NONE)
            return;

        // 뛰는 애니메이션 처리
        if (v == 0 && h == 0)
            Animator.PlayAnimator(AnimationConstantName.RUN, false);
        else
            Animator.PlayAnimator(AnimationConstantName.RUN, true);

        // 카메라 방향에 따른 플레이어 이동 처리
        Vector3 camForward = Vector3.Scale(Camera.main.transform.forward, new Vector3(1, 0, 1)).normalized;
        Vector3 dir = camForward * v + Camera.main.transform.right * h;
        transform.position += dir.normalized * speed * Time.deltaTime;

        // 플레이어 회전 처리
        if (dir.magnitude > 1f) dir.Normalize();
        dir = transform.InverseTransformDirection(dir);
        float turnAmount = Mathf.Atan2(dir.x, dir.z);
        transform.Rotate(0, turnAmount * 500f * Time.deltaTime, 0);
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
    /// 기본 무기(총을 든 상태) 상태로 변경
    /// </summary>
    public void SetBackBaseWeaponState()
    {
        weaponState = WeaponStyle.WEAPON;

        ThrowManager throwManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().ThrowManager;
        // 던지는 정보 설정
        throwManager.SetThrowInfo(ThrowItemType.NONE);
        //throwManager.SetActivatedThrowImage(false);
    }

    /// <summary>
    /// 총알 발사 처리
    /// </summary>
    public void Fire()
    {
        // 재장전 상태일 경우 리턴
        if (state == MotionState.RELOAD)
            return;

        // 공격 가능 시간이 아니라면 리턴
        if (!equipGun.IsAttackable())
            return;

        state = MotionState.SHOOT;

        // 재장전이 필요한 경우 재장전 실행
        if (equipGun.CheckReload())
        {

            Animator.PlayAnimator(AnimationConstantName.RUN, false);

            // 빈탄창 효과음 재생
            if (Time.time - lastActionTime >= EMPTY_GUN_SOUND_INTERVAL)
            {
                GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.EMPTY_GUN_SOUND);
                lastActionTime = Time.time;
            }    

            //Reload();
            return;
        }

        // 발사 애니메이션 처리
        Animator.PlayAnimator(AnimationConstantName.SHOOT, true);

        // gun 객체의 총알 발사 함수 실행
        equipGun.Fire(this);
    }

    /// <summary>
    /// 총알 발사를 멈췄을 때의 처리
    /// </summary>
    public void StopFire()
    {
        // 재장전 상태일 경우 리턴
        if (state == MotionState.RELOAD)
            return;

        // 발사 애니메이션 처리
        Animator.PlayAnimator(AnimationConstantName.SHOOT, false);

        state = MotionState.NONE;
    }

    /// <summary>
    /// 재장전 시작
    /// </summary>
    public void Reload()
    {
        // 재장전 상태일 시 리턴
        if (state == MotionState.RELOAD)
            return;

        Animator.PlayAnimator(AnimationConstantName.RUN, false);

        // 총 장작 상태가 아닌 경우 리턴
        if (weaponState != WeaponStyle.WEAPON && weaponState != WeaponStyle.PISTOL)
            return;

        StopFire();
        if (equipGun.OnReload())
        {
            // 리로드 시 리로드 UI 노출
            (PanelManager.GetPanel(typeof(InGamePanel)) as InGamePanel).ActivatedReloadBar();

            Animator.PlayAnimator(AnimationConstantName.RELOAD, true);

            state = MotionState.RELOAD;
        }
    }

    /// <summary>
    /// 재장전 완료
    /// </summary>
    void ReloadComplete(string soundName)
    {
        Animator.PlayAnimator(AnimationConstantName.RELOAD, false);

        // 재장전 효과음 재생
        GameManager.Instance.SoundManager.PlaySFX(soundName);

        // 딜레이 처리 코루틴 시작
        StartCoroutine(ReloadDelay());
    }

    /// <summary>
    /// 재장전 완료 후 딜레이 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator ReloadDelay()
    {
        yield return new WaitForSeconds(RELOAD_DELAY_TIME);
        state = MotionState.NONE;

        // 리로드 완료 후 리로드 UI 숨김 처리
        (PanelManager.GetPanel(typeof(InGamePanel)) as InGamePanel).ActivatedReloadBar();
    }

    /// <summary>
    /// 던지는 상태로 전환 및 던질 아이템 타입 셋팅
    /// </summary>
    /// <param name="throwItemType">던질 아이템 타입</param>
    public void SetThrowItem(ThrowItemType throwItemType)
    {
        // 재장전 상태일 시 리턴
        if (state == MotionState.RELOAD)
            return;

        // 던지는 중일 때 리턴
        if (state == MotionState.THROW)
            return;

        // 사용할 아이템이 남아있는지 검사
        switch (throwItemType)
        {
            case ThrowItemType.GRENADE:
                if (grenadeCount <= 0)
                    return;
                break;
        }

        // 던지기 상태로 변경
        weaponState = WeaponStyle.THROW;

        ThrowManager throwManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().ThrowManager;
        // 던지는 정보 설정
        throwManager.SetThrowInfo(throwItemType);
        //throwManager.SetActivatedThrowImage(true);

        // 던질 아이템 인덱스 설정
        throwItemIndex = (int)throwItemType;
    }

    /// <summary>
    /// 던지기 처리
    /// </summary>
    public void Throw()
    {
        // 던지고 있는 중인 경우 리턴
        if (state == MotionState.THROW)
            return;

        // 사용하는 아이템 수량 감소
        switch ((ThrowItemType)throwItemIndex)
        {
            case ThrowItemType.GRENADE:
                grenadeCount--;
                break;
        }

        // 마우스 지점으로 이동 벡터 생성
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, groundMask))
        {
            // 해당 지점을 바라보는 처리
            TargetToFace(hit.point);

            // 발사 애니메이션 처리
            Animator.PlayAnimator(AnimationConstantName.THROW);

            state = MotionState.THROW;
            // 딜레이 처리 코루틴 시작
            StartCoroutine(ThrowDelay(hit.point));
        }
    }

    /// <summary>
    /// 던진 후 딜레이 처리
    /// </summary>
    /// <returns></returns>
    IEnumerator ThrowDelay(Vector3 point)
    {
        ThrowManager throwManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().ThrowManager;
        //throwManager.SetActivatedThrowImage(false);

        yield return new WaitForSeconds(THROW_DELAY_TIME);

        // Throw 함수 실행 및 정보 초기화
        // 던지기 실행
        throwManager.GenerateThrowItem(point, transform.position, throwItemIndex, this);
        throwManager.SetThrowInfo(ThrowItemType.NONE);

        yield return new WaitForSeconds(THROW_DELAY_TIME);
        state = MotionState.NONE;
        // 총 사용 상태로 무기 상태 변경
        weaponState = WeaponStyle.WEAPON;
    }

    /// <summary>
    /// 처음 시작시 플레이어 모델 및 무기 설정
    /// </summary>
    /// <param name="modelIndex">설정할 모델 인덱스</param>
    /// <param name="mainGunIndex">설정할 주무기 인덱스</param>
    /// <param name="subGunIndex">설정할 보조무기 인덱스</param>
    /// <param name="generateModelRot">생성시 회전값</param>
    public void SetPlayer(int modelIndex, int mainGunIndex, int subGunIndex, Quaternion generateModelRot)
    {
        GameManager gameManager = GameManager.Instance;

        // 시작 캐릭터 모델 생성 및 설정
        model = Instantiate(gameManager.Models[modelIndex], Vector3.zero, generateModelRot);
        model.transform.SetParent(transform);

        GameObject go = null;

        // 보조무기 생성
        go = Instantiate(gameManager.GunModels[subGunIndex], transform);

        // 보조무기 설정
        subGun = go.GetComponent<Gun>();
        if (subGun != null)
        {
            equipGun = subGun;
        }

        // 주무기가 구입된 상태인지 확인
        if (mainGunIndex != -1)
        {
            // 주무기 생성
            go = Instantiate(gameManager.GunModels[mainGunIndex], transform);

            // 주무기 설정
            mainGun = go.GetComponent<Gun>();
            if (mainGun != null)
            {
                // 주무기가 있다면 장착
                equipGun = mainGun;
            }  
        }

        // 애니메이터 아바타 설정
        SetAvatar(avatar);

        // 장착 슬롯 UI 변경
        EquipSlotPanel equipSlotPanel = PanelManager.GetPanel(typeof(EquipSlotPanel)) as EquipSlotPanel;
        equipSlotPanel.ChangedEquipSlot(this);

        // 착용무기가 보조무기인지 확인
        if (equipGun is Pistol)
        {
            // 무기에 따른 애니메이션 설정
            SetBaseAnimation(WeaponStyle.PISTOL);
            return;
        }

        SetBaseAnimation(WeaponStyle.WEAPON);
    }

    /// <summary>
    /// 애니메이터 아바타 설정 함수
    /// </summary>
    void SetAvatar(Avatar avatar)
    {
        Animator.GetAnimator.avatar = avatar;
    }

    /// <summary>
    /// 무기에 따른 애니메이션 출력 구분 함수
    /// </summary>
    /// <param name="weaponStyle">무기 종류</param>
    public void SetBaseAnimation(WeaponStyle weaponStyle)
    {
        // 무기 상태 변경
        weaponState = weaponStyle;

        // 무기 종류 비교
        switch (weaponStyle)
        {
            // 라이플인 경우 
            case WeaponStyle.WEAPON:
                Animator.GetAnimator.SetLayerWeight(1, 0f);
                break;
            // 피스톨인 경우
            case WeaponStyle.PISTOL:
                Animator.GetAnimator.SetLayerWeight(1, 1f);
                break;
        }
    }

    /// <summary>
    /// 무기 스왑
    /// </summary>
    /// <param name="index">스왑할 무기 인덱스</param>
    public void SwapWeapon(int index)
    {
        // 재장전 중일 때는 리턴
        if (state == MotionState.RELOAD)
            return;

        state = MotionState.SWAP;

        // 0: 보조무기 인덱스
        // 1: 주무기 인덱스
        switch (index)
        {
            case 0:
                // 무기에 따른 애니메이션 셋 변경
                SetBaseAnimation(WeaponStyle.PISTOL);

                // 총꺼내는 애니메이션 재생
                Animator.PlayAnimator(AnimationConstantName.DRAW);

                // 보조무기 활성화
                subGun.gameObject.SetActive(true);
                // 주무기가 있다면 비활성화
                if (mainGun != null)
                    mainGun.gameObject.SetActive(false);

                // 장착 총기 변경
                equipGun = subGun;
                break;
            case 1:
                // 주무기가 없다면 리턴
                if (mainGun == null)
                    return;

                // 무기에 따른 애니메이션 셋 변경
                SetBaseAnimation(WeaponStyle.WEAPON);

                // 총꺼내는 애니메이션 재생
                Animator.PlayAnimator(AnimationConstantName.DRAW);

                // 주무기 활성화, 보조무기 비활성화
                mainGun.gameObject.SetActive(true);
                subGun.gameObject.SetActive(false);

                // 장착 총기 변경
                equipGun = mainGun;
                break;
        }

        state = MotionState.NONE;
    }

    /// <summary>
    /// 데미지 감소 처리
    /// </summary>
    /// <param name="damage">받은 데미지</param>
    /// <returns>감소된 데미지</returns>
    public override int OnDecreaseDamage(int damage)
    {
        int value = (int)(damage * decreaseDamage);
        damage -= value;

        return damage;
    }

    public override void OnDead()
    {
        //Animator.PlayAnimator(RUN_ANIMATION, false);
        //Animator.PlayAnimator(SHOOT_ANIMATION, false);
        //Animator.PlayAnimator(RELOAD_ANIMATION, false);

        base.OnDead();

        // 사망 시 콜라이더 및 강체 비활성
        GetComponent<Rigidbody>().useGravity = false;
        GetComponent<CapsuleCollider>().enabled = false;
    }

    /// 플레이어 생성 시 Init
    //public void OnPhotonInstantiate(PhotonMessageInfo info)
    //{
    //    // 인게임 매니저 초기화 셋팅
    //    GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Hero = this;


    //    // 카메라 설정
    //    FindObjectOfType<FollowCamera>().Target = transform;
    //    // 소모품 초기값 설정
    //    grenadeCount = 1;
    //    // 선택된 총, 캐릭터 모델
    //    selectModelIndex = GameManager.Instance.ModelIndex;
    //    selectGunModelIndex = GameManager.Instance.GunModelIndex;
    //    // 캐릭터 모델 오브젝트 생성
    //    Instantiate(GameManager.Instance.Models[selectModelIndex], transform);
    //    // 애니메이션 동기화를 위한 아바타 설정
    //    Animator.GetAnimator().avatar = avatar;
    //    // 총 모델 오브젝트 생성 후 참조
    //    equipGun = Instantiate(GameManager.Instance.GunModels[selectGunModelIndex], transform).GetComponent<Gun>();
    //}
}
