using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ContinuousDamage : MonoBehaviour
{
    const float ATTACK_TIME_INTERVAL = 1f;  // 지속데미지가 가해질 시간 간격
    const int DAMAGE_COUNT = 4;             // 데미지를 입힐 카운트

    [SerializeField] LayerMask targetMask;  // 타켓 마스크
    [SerializeField] float range;           // 공격 범위

    Actor attacker;                         // 공격자 정보

    int damage;                             // 가할 데미지
    int damageCount;                        // 데미지를 입힐 카운트

    float lastActionTime;                   // 동작이 끝난 시간
    bool isContinuous;                      // 지속 데미지가 가해지는지 여부

    /// <summary>
    /// 지속 데미지 셋팅
    /// </summary>
    /// <param name="damage">가할 데미지</param>
    /// <param name="attacker">공격자 정보</param>
    public void Set(int damage, Actor attacker)
    {
        this.damage = damage;
        this.attacker = attacker;

        damageCount = DAMAGE_COUNT;

        lastActionTime = Time.time;
        isContinuous = true;
    }

    private void Update()
    {
        UpdateContinuosDamage();
    }

    /// <summary>
    /// 지속 데미지 처리 함수
    /// </summary>
    void UpdateContinuosDamage()
    {
        if (!isContinuous)
            return;

        if(Time.time - lastActionTime >= ATTACK_TIME_INTERVAL)
        {
            // 카운트만큼 데미지를 가했다면 중지
            if(damageCount <= 0)
                isContinuous = false;

            damageCount--;
            TakeDamage();

            lastActionTime = Time.time;
        }
    }

    /// <summary>
    /// 데미지 처리 함수
    /// </summary>
    public void TakeDamage()
    {
        // 타켓 마스크에 맞는 객체를 얻어옴
        Collider[] colliders = Physics.OverlapSphere(transform.position, range, targetMask);

        if (colliders.Length > 0)
        {
            // 타켓마스크에 속하는 객체에 데미지를 가함
            for (int i = 0; i < colliders.Length; i++)
            {
                colliders[i].GetComponent<Actor>().OnTakeHit(damage, attacker);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }
}
