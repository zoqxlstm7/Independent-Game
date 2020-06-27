using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ModelSlot : MonoBehaviour
{
    [SerializeField] Sprite icon;       // 아이콘
    public Sprite Icon
    {
        get => icon;
    }

    [SerializeField] int buyableGold;   // 구매에 필요한 골드
    public int BuyableGold
    {
        get => buyableGold;
    }
}
