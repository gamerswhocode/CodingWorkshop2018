using UnityEngine;
using UnityEditor;

public class KnockbackState : IState
{

    Collider ObjectCollider;
    float KnockbackForce;
    ObjectFacingRight FacingRight;
    System.Action<KnockbackResultData> CallBack;
    float AirbornCheckFrame;


    public KnockbackState(Collider objectCollider, float knockbackForce, 
        ObjectFacingRight facingRight, System.Action<KnockbackResultData> callbackFunction)
    {
        CallBack = callbackFunction;
        ObjectCollider = objectCollider;
    }

    public void Enter()
    {
        //ObjectRigidBody.AddForce(new Vector3(FacingRight() ? 50f : -50f, -50f) * KnockbackForce);
        AirbornCheckFrame = 3;
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }

    public void FixedExecute()
    {
        CheckForCollisions();
    }

    private void CheckForCollisions()
    {
        if (AirbornCheckFrame <= 0)
        {
            Collider[] col = Physics.OverlapBox(ObjectCollider.bounds.center, ObjectCollider.bounds.extents, ObjectCollider.transform.rotation);
            if (col.Length > 0)
            {
                CallBack(new KnockbackResultData(col));
            }
        }
        else
        {
            AirbornCheckFrame--;
        }
    }

    public delegate bool ObjectFacingRight();
}

public class KnockbackResultData
{
    public Collider[] CollidedWith;

    public KnockbackResultData(Collider[] collidedWith)
    {
        CollidedWith = collidedWith;
    }
}
