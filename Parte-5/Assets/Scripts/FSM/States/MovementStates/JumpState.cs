using UnityEngine;

public class JumpState : IState
{
    private int AirbornCheckFrame;
    private Collider ObjectCollider;
    System.Action<JumpResults> JumpResultsCallback;
    private float JumpVelocity;
    private Rigidbody ObjectRigidBody;


    public JumpState(Collider objectCollider, Rigidbody objectRigidBody,
        float jumpVelocity, System.Action<JumpResults> jumpResultsCallback)
    {
        JumpResultsCallback = jumpResultsCallback;
        ObjectCollider = objectCollider;
        JumpVelocity = jumpVelocity;
        ObjectRigidBody = objectRigidBody;
    }

    public void Enter()
    {
        AirbornCheckFrame = 5;
        
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
                JumpResultsCallback(new JumpResults(col));
            }
        }
        else
        {
            AirbornCheckFrame--;
        }
    }
}

public class JumpResults
{
    public Collider[] CollidedWith;
    public JumpResults(Collider[] collidedWith)
    {
        CollidedWith = collidedWith;
    }
}
    