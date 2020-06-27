using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandGrenade : ThrowItem
{
    /// <summary>
    /// 목표지점에 도착한 후에 처리를 실행
    /// </summary>
    /// /// <param name="attacker">공격자 객체</param>
    public override void AfterThrow(Actor attacker)
    {
        // 폭발 사운드 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.GRENADE_SOUND);

        // 범위 내 콜라이더를 가진 객체를 얻어옴
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        // 길이가 0이상일 때
        if(colliders.Length > 0)
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
            , new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z));

        // 잔여 불씨 생성
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(EffectManager.REMAIN_FIRE_FILE_PATH
            , new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z));

        base.AfterThrow(attacker);
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
