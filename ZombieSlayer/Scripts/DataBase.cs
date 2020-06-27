using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class DataBase : MonoBehaviour
{
    [SerializeField] int startMainGunIndex;                 // 시작 주무기 인덱스
    [SerializeField] int startSubGunIndex;                  // 시작 보조 무기 인덱스
    [SerializeField] int startModelIndex;                   // 시작 캐릭터 모델 인덱스

    [SerializeField] SaveData saveData = new SaveData();    // 세이브 데이터
    public SaveData SaveData
    {
        get => saveData;
    }

    private void Awake()
    {
        Load();
        Debug.Log(Application.persistentDataPath + "/Data.json");
    }

    /// <summary>
    /// 데이터 저장
    /// </summary>
    public void Save()
    {
        // 쓰기
        using (StreamWriter sw = new StreamWriter(new FileStream(Application.persistentDataPath + "/Data.json", FileMode.Create), Encoding.UTF8))
        {
            string json = JsonUtility.ToJson(saveData, prettyPrint:true);
            // Base64로 인코딩하여 아스키 문자열로 변환
            byte[] bytes = Encoding.UTF8.GetBytes(json);
            string codes = System.Convert.ToBase64String(bytes);

            sw.Write(codes);
        }
    }

    /// <summary>
    /// 데이터 불러오기
    /// </summary>
    public void Load()
    {
        // 데이터가 존재하는지 확인
        if(!File.Exists(Application.persistentDataPath + "/Data.json"))
        {
            Debug.Log("세이브 데이터가 없습니다.");

            saveData.modelIndex = startModelIndex;
            saveData.mainGunIndex = startMainGunIndex;
            saveData.subGunIndex = startSubGunIndex;

            // 초기 지급 골드
            saveData.gold += 3000;

            return;
        }

        using (StreamReader sr = new StreamReader(new FileStream(Application.persistentDataPath + "/Data.json", FileMode.Open), Encoding.UTF8))
        {
            string codes = sr.ReadToEnd();
            // 아스키 문자열을 json 문자열로 변환
            byte[] bytes = System.Convert.FromBase64String(codes);
            string json = Encoding.UTF8.GetString(bytes);

            saveData = JsonUtility.FromJson<SaveData>(json);
        }
    }
}
