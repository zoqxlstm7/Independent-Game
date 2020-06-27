using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 애니메이션 이름 상수화 클래스
/// </summary>
public class AnimationConstantName
{
    // 피스톨 애니메이션

    // 라이플 애니메이션
    public const string DRAW = "Draw";
    public const string RUN = "Run";
    public const string SHOOT = "Shoot";
    public const string ATTACK = "Attack";
    public const string RELOAD = "Reload";
    public const string THROW = "Throw";
    public const string DIE = "Die";
}

public class AnimatorController
{
    Animator animator;          // 애니메이터 객체
    public Animator GetAnimator
    {
        get => animator;
    }
    string currnetAnimation;    // 현재 재생중인 애니메이션

    /// <summary>
    /// 초기화 함수
    /// </summary>
    /// <param name="animator">지정할 애니메이터 객체</param>
    public AnimatorController(Animator animator)
    {
        this.animator = animator;
    }

    /// <summary>
    /// bool형 애니메이션 실행
    /// </summary>
    /// <param name="name">실행할 애니메이션 이름</param>
    /// <param name="value">활성 비활성 여부</param>
    public void PlayAnimator(string name, bool value)
    {
        currnetAnimation = name;
        animator.SetBool(name, value);
    }

    /// <summary>
    /// trigger형 애니메이션 실행
    /// </summary>
    /// <param name="name">실행할 애니메이션 이름</param>
    public void PlayAnimator(string name)
    {
        if (!string.IsNullOrEmpty(currnetAnimation))
        {
            animator.SetBool(currnetAnimation, false);
        }
        
        animator.SetTrigger(name);
    }
}
