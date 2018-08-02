using UnityEngine;
using UnityEditor;

public class IdleState : IState
{
    Animator ParentAnimator;
    AnimationClip Clip;
    float PlayTime;

    public IdleState(Animator parentAnimator, AnimationClip animationClip)
    {
        ParentAnimator = parentAnimator;
        Clip = animationClip;
    }

    public void Enter()
    {
        ParentAnimator.Play(Clip.name, -1, 0f);
    }

    public void Execute()
    {
        Debug.Log("Execute Idle");
    }

    public void Exit()
    {
        ParentAnimator.StopPlayback();
    }

    public void FixedExecute()
    {

    }
}