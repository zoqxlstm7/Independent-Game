using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    [SerializeField] Vector3 offset;    // 기본 오프셋

    [SerializeField] Transform target;
    public Transform Target
    {
        set => target = value;
    }

    private void Start()
    {
        offset = transform.position;
    }

    private void LateUpdate()
    {
        // 타겟이 없으면 리턴
        if (target == null)
            return;

        transform.position = Vector3.Lerp(target.position, transform.position + offset, 0.5f);
    }
}
