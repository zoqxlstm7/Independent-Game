using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PistolBurst : Pistol
{
    const int BURST_COUNT = 3;                  // 점사시 총알 수
    const float BURST_TIME_INTERVAL = 0.1f;     // 점사시 시간 간격

    public override void Fire(Actor owner)
    {
        StartCoroutine(BurstShot(owner));
    }

    IEnumerator BurstShot(Actor owner)
    {
        lastActionTime = Time.time;

        for (int i = 0; i < BURST_COUNT; i++)
        {
            yield return null;

            // 남아있는 총알이 없다면 리턴
            if (remainBulletCount <= 0)
                break;

            Bullet newBullet = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BulletManager.Generate((int)bulletStyle, firePoint.position);

            if (newBullet != null)
            {
                // 총 종류에 따른 사운드 재생
                GameManager.Instance.SoundManager.PlaySFX(bulletStyle.ToString());

                // 발사 이펙트 생성 함수 실행
                GenerateMuzzleEffect();

                //발사처리
                newBullet.Fire(owner, firePoint.forward, Damage, firePoint.position, rangeOfShot);
                // 총구 방향에 맞춰 총알 회전
                newBullet.transform.rotation = firePoint.rotation;
                // 남은 총알 감소
                remainBulletCount--;

                // 탄피생성 함수 실행
                GenerateEmptyShell((int)bulletStyle, owner);
            }

            // 시간 간격 후 재실행
            yield return new WaitForSeconds(BURST_TIME_INTERVAL);
        }
    }
}
