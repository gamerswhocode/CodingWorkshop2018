using UnityEngine;
using UnityEditor;

public class ProyectileState : IState
{

    ProyectileCallback CallbackFunction;
    AnimationClip Clip;
    Animator ObjectAnimator;

    float Playtime;
    float TotalRunTime;

    public ProyectileState(ProyectileCallback callback, AnimationClip clip, Animator animator)
    {
        ObjectAnimator = animator;
        Clip = clip;
        CallbackFunction = callback;
        TotalRunTime = clip.length;
    }

    public void Enter()
    {
        Playtime = 0;
        ObjectAnimator.Play(Clip.name, -1, 0f);
    }

    public void Execute()
    {
        if (Playtime < TotalRunTime)
        {
            Playtime += 1 * Time.deltaTime;
        }
        else
        {
            CallbackFunction();
        }
    }

    public void Exit()
    {
        ObjectAnimator.StopPlayback();
    }

    public void FixedExecute()
    {
        
    }

    public delegate void ProyectileCallback();


}

