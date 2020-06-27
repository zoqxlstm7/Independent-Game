using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseSceneManager : MonoBehaviour
{
    private void Start()
    {
        Initialize();
    }

    private void Update()
    {
        UpdateManager();
    }

    public virtual void Initialize()
    {

    }

    public virtual void UpdateManager()
    {

    }
}
