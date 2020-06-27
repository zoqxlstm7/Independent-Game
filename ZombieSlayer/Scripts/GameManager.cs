using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    /// <summary>
    /// 싱글톤 객체 생성
    /// </summary>
    #region SingleTone
    static GameManager instance;
    public static GameManager Instance
    {
        get => instance;
    }

    private void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }
    #endregion

    // 현재 씬을 관리하는 객체 할당
    BaseSceneManager currentSceneManager;
    public BaseSceneManager CurrentSceneManager
    {
        set => currentSceneManager = value;
    }

    // 씬 관리 객체 반환
    [SerializeField] SceneController sceneController;
    public SceneController SceneController
    {
        get => sceneController;
    }

    // 오디오 관리 객체 반환
    [SerializeField] SoundManager soundManager;
    public SoundManager SoundManager
    {
        get => soundManager;
    }

    // 스프라이트 관리 객체 반환
    [SerializeField] SpriteSet spriteSet;
    public SpriteSet SpriteSetManager
    {
        get => spriteSet;
    }

    // 버프 관리 객체 반환
    [SerializeField] BuffManager buffManager;
    public BuffManager BuffManager
    {
        get => buffManager;
    }

    // 데이터 베이스 객체 반환
    [SerializeField] DataBase dataBase;
    public DataBase DataBase
    {
        get => dataBase;
    }

    // 캐릭터 모델 오브젝트
    [SerializeField] GameObject[] models;
    public GameObject[] Models
    {
        get => models;
    }

    // 총 모델 오브젝트
    [SerializeField] GameObject[] gunModels;
    public GameObject[] GunModels
    {
        get => gunModels;
    }
    
    //public int SelectModelIndex { get; set; }       // 선택한 캐릭터 모델 인덱스
    //public int SelectMainGunIndex { get; set; }     // 선택한 주무기 모델 인덱스
    //public int SelectSubGunIndex { get; set; }      // 선택한 보조무기 모델 인덱스
    //public int GrenadeCount { get; set; }           // 보유한 수류탄 갯수
    //public int TntCount { get; set; }               // 보유한 tnt 갯수
    //public int Gold { get; set; }                   // 보유한 골드

    private void Start()
    {
        // 현재 씬을 관리하는 객체 캐싱
        BaseSceneManager baseSceneManager = FindObjectOfType<BaseSceneManager>();
        currentSceneManager = baseSceneManager;
    }

    private void Update()
    {
        // 안드로이드 상에서 뒤로 가기 버튼이 눌리면 세팅 패널 보여줌
        if (Application.platform == RuntimePlatform.Android)
        {
            if (Input.GetKey(KeyCode.Escape))
            {
                SettingsPanel settingsPanel = PanelManager.GetPanel(typeof(SettingsPanel)) as SettingsPanel;
                settingsPanel.Show();
            }
        }
    }

    /// <summary>
    /// 현재 씬을 관리하는 객체 반환
    /// </summary>
    /// <typeparam name="T">반환받을 타입</typeparam>
    /// <returns></returns>
    public T GetCurrentSceneManager<T>() where T : BaseSceneManager
    {
        return currentSceneManager as T;
    }

    // 데이터 불러오기
    void Load()
    {

    }
}
