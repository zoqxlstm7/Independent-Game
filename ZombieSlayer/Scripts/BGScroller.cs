using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[System.Serializable]
public class ScrollData
{
    public Image backGround;
    public float offset;
    public float speed;
}

public class BGScroller : MonoBehaviour
{
    [SerializeField] ScrollData[] scrollDatas;

    private void Update()
    {
        UpdateScroll();
    }

    void UpdateScroll()
    {
        for (int i = 0; i < scrollDatas.Length; i++)
        {
            ScrollTexture(scrollDatas[i]);
        }
    }

    void ScrollTexture(ScrollData data)
    {
        data.offset += Time.deltaTime * data.speed;
        if (data.offset > 1f)
            data.offset = data.offset % 1f;

        Vector2 offset = new Vector2(data.offset, 0);
        data.backGround.material.SetTextureOffset("_MainTex", offset);
    }
}
