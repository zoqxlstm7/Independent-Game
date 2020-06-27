using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] JoyStick movejoyStick;     // 이동 조이패드
    [SerializeField] JoyStick rotJoyStic;       // 방향 조이패드

    InGameSceneManager inGameSceneManager;      // 인게임씬 매니저

    private void Start()
    {
        // 인게임씬 매니저 캐싱
        inGameSceneManager = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>();
    }

    private void Update()
    {
        // 게임매니저가 없을시 리턴
        if (!FindObjectOfType<GameManager>())
            return;

        // 히어로 객체가 null일시 리턴
        if (inGameSceneManager.Player == null)
            return;

        // 게임오버시 리턴
        if (inGameSceneManager.IsGameOver)
            return;

        UpdateInput();
    }

    void UpdateInput()
    {
        // 플레이어 사망시 리턴
        if (inGameSceneManager.Player.IsDead)
            return;

        UpdateMove();
        UpdateMouseInput();
    }

    void UpdateMouseInput()
    {
        Player player = GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player;

        // 총 사용
        if (player.WeaponState == WeaponStyle.WEAPON || player.WeaponState == WeaponStyle.PISTOL)
            UpdateRotate();
        // 던지기 사용
        else if (player.WeaponState == WeaponStyle.THROW)
            ThrowControl();
    }

    /// <summary>
    /// 이동 처리 함수
    /// </summary>
    void UpdateMove()
    {
        // 조이패드의 방향을 입력받아 처리
        float v, h;
        v = movejoyStick.GetVerticalValue();
        h = movejoyStick.GetHorizontalValue();

        inGameSceneManager.Player.UpdateMove(v, h);

        // 키보드 버전
        //v = Input.GetAxisRaw("Vertical");
        //h = Input.GetAxisRaw("Horizontal");

        //inGameSceneManager.Player.UpdateMove(v, h);
    }

    void UpdateRotate()
    {
        FaceToTargetFromJoypad();
    }

    void FaceToTargetFromJoypad()
    {
        // 조이패드의 방향을 입력받아 처리
        float v, h;
        v = rotJoyStic.GetVerticalValue();
        h = rotJoyStic.GetHorizontalValue();

        if (v == 0 && h == 0)
        {
            inGameSceneManager.Player.StopFire();
            return;
        }

        // 패드 방향으로 플레이어 회전
        inGameSceneManager.Player.transform.rotation = Quaternion.Euler(0f, Mathf.Atan2(h, v) * Mathf.Rad2Deg, 0f);

        // 패드 방향으로 총 발사
        inGameSceneManager.Player.Fire();
    }

    /// <summary>
    /// 수류탄 준비 함수
    /// </summary>
    public void OnReadyGrenade()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        inGameSceneManager.Player.SetThrowItem(ThrowItemType.GRENADE);
    }

    /// <summary>
    /// 수류탄 준비 취소 함수
    /// </summary>
    public void OnCancleGrenade()
    {
        inGameSceneManager.Player.SetBackBaseWeaponState();
    }

    /// <summary>
    /// tnt 설치 함수
    /// </summary>
    public void OnTntInstall()
    {
        // 버튼음 재생
        GameManager.Instance.SoundManager.PlaySFX(AudioNameConstant.BUTTON_SOUND);

        Player player = inGameSceneManager.Player;

        if (player.TntCount > 0)
        {
            player.TntCount--;
            inGameSceneManager.TnTManager.Generate(TnTManager.TNT_FILE_PATH, player.transform.position, player);
        }
    }

    /// <summary>
    /// 던질 때의 행동 처리
    /// </summary>
    void ThrowControl()
    {
        // 왼쪽 마우스 버튼이 떼질 때 던짐 함수 실행
        if (Input.GetMouseButtonDown(0))
        {
            GameManager.Instance.GetCurrentSceneManager<InGameSceneManager>().Player.Throw();
        }
    }

    /// <summary>
    /// 재장전 처리 함수
    /// </summary>
    public void OnReload()
    {
        inGameSceneManager.Player.Reload();
    }
}
