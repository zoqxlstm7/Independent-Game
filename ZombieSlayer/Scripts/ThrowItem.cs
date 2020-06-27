using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowItem : MonoBehaviour
{
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

    float speed;                            // 날아가는 속도    
    Vector3 endPoint;                       // 도착지점
    Vector3 startPoint;                     // 출발점
    Actor attacker;                         // 공격자

    bool isMove;                            // 움직임 시작여부

    private void Awake()
    {
        OriginValue = value;
        OriginRange = range;
    }

    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateThrow();
    }

    /// <summary>
    /// 초기화 함수
    /// </summary>
    public virtual void Initialize()
    {
        
    }

    /// <summary>
    /// 업데이트 처리 함수
    /// </summary>
    public virtual void UpdateThrow()
    {
        UpdateMove();
    }

    /// <summary>
    /// 궤적대로 날아가는 처리 실행
    /// </summary>
    void UpdateMove()
    {
        // 움직일 필요가 없으면 리턴
        if (!isMove)
            return;

        // 거리 계산
        float distance = Vector3.Distance(transform.position, endPoint);

        // 도착점 도착시 효과처리 함수 실행
        if (distance <= 2f)
        {
            isMove = false;
            AfterThrow(attacker);
        }

        // 시작점부터 도착점까지 포물선 궤적으로 날아갈 수 있도록 처리
        speed += Time.deltaTime;
        speed %= 0.5f;
        transform.position = MathParabola.Parabola(startPoint, endPoint, 5f, speed / 0.5f);
    }

    /// <summary>
    /// 던짐 시작 함수
    /// </summary>
    /// <param name="point">도착지점</param>
    /// <param name="attacker">공격자 객체</param>
    public void Throw(Vector3 point, Actor attacker)
    {
        // 공격자 설정
        this.attacker = attacker;

        // 시작점, 도착점 셋팅 및 움직임 true 처리
        startPoint = transform.position;
        endPoint = point;
        isMove = true;
    }

    /// <summary>
    /// 목표지점에 도착한 후에 처리를 실행
    /// </summary>
    /// <param name="attacker">공격자 객체</param>
    public virtual void AfterThrow(Actor attacker)
    {
        //Debug.Log("AfterThrow");
        Destroy(gameObject);
    }
}
