using UnityEngine;

public class DiveKickState : IState
{

    private Vector3 DiveKickDirection;
    private Rigidbody ObjectBody;
    private ActorLookingRight LookingRight;
    private float DiveKickForce;

    public DiveKickState(Rigidbody objectBody, float diveKickForce, ActorLookingRight lookingRight)
    {
        ObjectBody = objectBody;
        DiveKickForce = diveKickForce;
        LookingRight = lookingRight;
    }

    public void Enter()
    {
        DiveKickDirection = new Vector3(LookingRight() ? 50f : -50f, -50f);
        ObjectBody.AddForce(DiveKickDirection * DiveKickForce);
    }

    public void Execute()
    {
        
    }

    public void Exit()
    {
        
    }

    public void FixedExecute()
    {
        
    }

    public delegate bool ActorLookingRight();
}