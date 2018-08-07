using UnityEngine;

class DeathState : IState
{

    AnimationClip Clip;
    Animator ObjectAnimator;
    Collider ObjectCollider;

    float PlaytimeTotal;
    System.Action<DeathStateResult> ResultCallback;
    PostMorterm CallbackFunction;

    public DeathState(Animator objectAnimator, AnimationClip clip, Collider objectCollider,
        System.Action<DeathStateResult> callback,
        PostMorterm postMorterm
        )
    {
        ObjectAnimator = objectAnimator;
        ObjectCollider = objectCollider;
        Clip = clip;
        ResultCallback = callback;
        CallbackFunction = postMorterm;
    }

    public void Enter()
    {
        PlaytimeTotal = Clip.length;
        ObjectAnimator.Play(Clip.name, -1, 0f);
    }

    public void Execute()
    {

    }

    public void Exit()
    {
        //ObjectAnimator.StopPlayback();
    }

    public void FixedExecute()
    {
        PlaytimeTotal -= 1 * Time.deltaTime;
        if (PlaytimeTotal <= 0)
        {
            CallbackFunction();
        }
        else
            CheckForCollisions();
    }

    private void CheckForCollisions()
    {
            Collider[] col = Physics.OverlapBox(ObjectCollider.bounds.center, ObjectCollider.bounds.extents, ObjectCollider.transform.rotation);
            if (col.Length > 0)
            {
                ResultCallback(new DeathStateResult(col));
            }
    }

    public delegate void PostMorterm();
}

public class DeathStateResult
{
    public Collider[] CollidedWith;

    public DeathStateResult(Collider[] objectCollidedWith)
    {
        CollidedWith = objectCollidedWith;
    }
}

    