using UnityEngine;

public class PunchState : IState
{

    Animator ParentAnimator;
    AnimationClip Clip;
    float AnimationDuration;
    float PlayTime;
    System.Action<PunchResults> PunchResultsCallback;

    public PunchState(Animator parentAnimator, AnimationClip animationClip,
        System.Action<PunchResults> punchResultsCallback)
    {
        ParentAnimator = parentAnimator;
        Clip = animationClip;
        AnimationDuration = animationClip.length;
        PunchResultsCallback = punchResultsCallback;
    }

    public void Enter()
    {
        PlayTime = 0;
        ParentAnimator.Play(Clip.name, -1, 0f);
    }

    public void Execute()
    {
        Debug.Log("Execute Punch");
        if (PlayTime < AnimationDuration)
        {
            PlayTime += 1 * Time.deltaTime;
        }
        else
        {
            var result = new PunchResults();
            PunchResultsCallback(result);
        }
    }

    public void Exit()
    {
        ParentAnimator.StopPlayback();
    }

    public void FixedExecute()
    {
        
    }
}

public class PunchResults
{
    public PunchResults()
    {

    }
}