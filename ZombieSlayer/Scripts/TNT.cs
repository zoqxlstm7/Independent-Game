using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TNT : MonoBehaviour
{
    const float UP_TO_THE_EXPLOSION_TIME = 2f;                      // 폭발하기까지 걸리는 시간

    [SerializeField] protected int value;   // 효과량 (ex. 폭탄데미지 및 힐팩 힐량 설정)
    public int Value
    {
        get => value;
        set => this.value = value;
    }
    [SerializeField] protected float range; // 범위
    public float Range
    {
        get => range;
        set => range = value;
    }

    public int OriginValue { get; set; }
    public float OriginRange { get; set; }

    public string FilePath { get; set; }    // 파일 경로
    public Actor attacker { get; set; }

    float generateTime;                     // 생성 시간      
    bool isExplosion;                       // 폭발이 진행되었는지 여부

    /// <summary>
    /// 활성화시 초기화 진행
    /// </summary>
    private void OnEnable()
    {
        generateTime = Time.time;
        isExplosion = false;
    }

    private void Awake()
    {
        OriginValue = value;
        OriginRange = range;
    }

    private void Update()
    {
        UpdateBoomTimeOutCheck();
    }

    void UpdateBoomTimeOutCheck()
    {
        // 폭발했다면 리턴
        if (isExplosion)
            return;

        // 폭발 시간이 되었다면
        if(Time.time - generateTime >= UP_TO_THE_EXPLOSION_TIME)
        {
            // 폭발 플래그 처리
            isExplosion = true;

            // 폭발 사운드 재생
            GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.GRENADE_SOUND);

            // 범위 내 콜라이더를 가진 객체를 얻어옴
            Collider[] colliders = Physics.OverlapSphere(transform.position, range);

            // 길이가 0이상일 때
            if (colliders.Length > 0)
            {
                for (int i = 0; i < colliders.Length; i++)
                {
                    // 레이어가 에너미인 객체만 데미지 처리
                    if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
                    {
                        // 데미지 처리
                        colliders[i].gameObject.GetComponent<Actor>().OnTakeHit(value, attacker);
                    }
                }
            }

            // 폭발 이펙트 생성
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(EffectManager.EXPLOSION_EFFECT_FILE_PATH
                , new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));

            // 지뢰 폭발 이펙트 생성
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(EffectManager.LAND_MINE_EFFECT_FILE_PATH
                , new Vector3(transform.position.x, transform.position.y + 1f, transform.position.z));

            // 잔여 불씨 생성
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(EffectManager.REMAIN_FIRE_FILE_PATH
                , transform.position);

            // tnt 제거
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().TnTManager.Remove(FilePath, gameObject);
        }
    }

    /// <summary>
    /// 기즈모 표시
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
