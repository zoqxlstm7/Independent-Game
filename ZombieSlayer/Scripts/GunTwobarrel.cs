﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunTwobarrel : Gun
{
    const int Twobarrel_COUNT = 2;                  // 점사시 총알 수
    const float Twobarrel_TIME_INTERVAL = 0.2f;     // 점사시 시간 간격

    const int MAX_GENERATE_BULLET = 5;              // 최대 산탄총알 생성 횟수
    const float ROTATION_INTERVAL = 15f;            // 회전 간격

    public override void Fire(Actor owner)
    {
        StartCoroutine(TwoBarrelShot(owner));
    }

    IEnumerator TwoBarrelShot(Actor owner)
    {
        lastActionTime = Time.time;

        for (int x = 0; x < Twobarrel_COUNT; x++)
        {
            yield return null;

            // 남아있는 총알이 없다면 리턴
            if (remainBulletCount <= 0)
                break;

            // 시작 산탄 발사 방향
            float rotation = -30f;

            // 총 종류에 따른 사운드 재생
            GameManager.Instance.SoundManager.PlaySFX(bulletStyle.ToString());

            // 발사되는 산탄수만큼 탄알 반복 생성 후 처리
            for (int i = 0; i < MAX_GENERATE_BULLET; i++)
            {
                Bullet newBullet = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().BulletManager.Generate((int)bulletStyle, firePoint.position);

                if (newBullet != null)
                {
                    // 발사 이펙트 생성 함수 실행
                    GenerateMuzzleEffect();

                    newBullet.Fire(owner, firePoint.forward, Damage, firePoint.position, rangeOfShot);

                    // 산탄 방향 지정
                    Quaternion quat = Quaternion.identity;
                    quat.eulerAngles = new Vector3(firePoint.localRotation.x + rotation, firePoint.localRotation.y, firePoint.localRotation.z);
                    firePoint.localRotation = quat;

                    // 총구 방향에 맞춰 총알 회전
                    newBullet.transform.rotation = firePoint.rotation;

                    // 산탄 방향 변경
                    rotation += ROTATION_INTERVAL;
                }
            }

            // 남은 총알 감소
            remainBulletCount--;
            // 탄피생성 함수 실행
            GenerateEmptyShell((int)bulletStyle, owner);

            // 시간 간격 후 재실행
            yield return new WaitForSeconds(Twobarrel_TIME_INTERVAL);
        }
    }
}
