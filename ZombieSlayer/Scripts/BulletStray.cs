using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletStray : Bullet
{
    [SerializeField] float range;

    /// <summary>
    /// 적이 아닌 다른 충돌체일 때의 처리 함수
    /// </summary>
    public override void OnBulletOtherCollision(GameObject gameObject)
    {
        // 폭발 이펙트 생성
        GenerateExplosiveEffect();
        // 범위 데미지
        OnTakeHitInRange();

        // 콜라이더 비활성화
        collider.enabled = false;
        // 충돌한 탄알 제거
        DestroyBullet();
    }

    
    /// <summary>
    /// 총알 충돌 처리
    /// </summary>
    /// <param name="targetActor">충돌된 객체</param>
    public override void OnBulletCollision(Actor targetActor)
    {
        if (owner == null)
            return;

        // Actor클래스를 상속한 객체 중 레이어가 다른 오브젝트끼리 충돌 처리
        if (owner.gameObject.layer != targetActor.gameObject.layer)
        {
            // 폭발 이펙트 생성
            GenerateExplosiveEffect();

            // 콜라이더 비활성화
            collider.enabled = false;
            // 범위 데미지
            OnTakeHitInRange();
            
            // 충돌한 탄알 제거
            DestroyBullet();
        }
    }

    /// <summary>
    /// 범위 안 대상에게 데미지를 가하는 함수
    /// </summary>
    void OnTakeHitInRange()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, range);

        if(colliders.Length > 0)
        {
            for (int i = 0; i < colliders.Length; i++)
            {
                // 레이어가 에너미인 객체만 데미지 처리
                if (colliders[i].gameObject.layer == LayerMask.NameToLayer("Enemy"))
                {
                    // 데미지 처리
                    colliders[i].gameObject.GetComponent<Actor>().OnTakeHit(damage, owner);
                }
            }
        }
    }

    /// <summary>
    /// 폭발 이펙트 생성 함수
    /// </summary>
    void GenerateExplosiveEffect()
    {
        // 폭발 사운드 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.GRENADE_SOUND);

        // 폭발 이펙트 생성
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(EffectManager.EXPLOSION_EFFECT_FILE_PATH
            , new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z));

        // 잔여 불씨 생성
        GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().EffectManager.Generate(EffectManager.REMAIN_FIRE_FILE_PATH
            , new Vector3(transform.position.x, transform.position.y - 1f, transform.position.z));
    }

    /// <summary>
    /// 총알이 발사되고 난 후 생명주기를 체크
    /// </summary>
    protected override void CheckBulletLifeTime()
    {
        // 발사 후 사정거리만큼 이동 후 총알 삭제
        float distance = Vector3.Distance(transform.position, firePoint);
        if (distance > rangeOfShot)
        {
            // 폭발 처리
            GenerateExplosiveEffect();
            // 범위 데미지
            OnTakeHitInRange();

            DestroyBullet();
        }
    }
}
