using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandContainer : MonoBehaviour
{
    [SerializeField] Transform container;  // 총이 장착될 위치
    public Transform Container
    {
        get => container;
    }
}
