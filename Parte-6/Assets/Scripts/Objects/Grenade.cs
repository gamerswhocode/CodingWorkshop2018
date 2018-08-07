using UnityEngine;


public class Grenade : MonoBehaviour, IThrowable
{

    StateMachine _movementState;
    StateMachine _actionState;

    [Header("Grenade Animations")]
    [SerializeField]
    AnimationClip IdleAnimation;
    [SerializeField]
    AnimationClip ExplosionAnimation;

    [Space(20)]
    [Header("Grenade States")]
    [SerializeField]
    string actionState;
    [SerializeField]
    string movementState;
    AirbornState _airborn;
    IdleState _idle;
    GroundedState _grounded;
    DeathState _death;


    private void Awake()
    {
        InitializeVariables();
    }

    void Update()
    {
        actionState = _actionState.GetCurrentlyRunningState().Name;
        movementState = _movementState.GetCurrentlyRunningState().Name;
        _actionState.ExecuteStateUpdate();
        _movementState.ExecuteStateUpdate();
    }

    private void FixedUpdate()
    {
        _actionState.ExecuteStateFixedUpdate();
        _movementState.ExecuteStateFixedUpdate();
    }

    //Here for the Object Pooling
    public void InitializeVariables()
    {

        var componentAnimator = GetComponent<Animator>();
        var componentCollider = GetComponent<Collider>();
        var componentRigidBody = GetComponent<Rigidbody>();
        _movementState = new StateMachine();
        _actionState = new StateMachine();

        _idle = new IdleState(componentAnimator, IdleAnimation);
        _grounded = new GroundedState();
        _airborn = new AirbornState(componentCollider, componentRigidBody, 0, AirbornCallback);
        _death = new DeathState(componentAnimator, ExplosionAnimation, componentCollider, DeathCallback, PostMortermResult);

        _actionState.ChangeState(_idle);
        _movementState.ChangeState(_grounded);
    }

    public void ThrowObject()
    {
        _movementState.ChangeState(_airborn);
    }

    public void AirbornCallback(JumpResults results)
    {
        foreach (var grenadeTouched in results.CollidedWith)
        {
            if (grenadeTouched.gameObject.layer == LayerMask.NameToLayer("Scenario"))
            {
                var rigidbody = this.gameObject.GetComponent<Rigidbody>();
                rigidbody.constraints = RigidbodyConstraints.FreezeRotation 
                    | RigidbodyConstraints.FreezePosition;
                _actionState.ChangeState(_death);
                _movementState.ChangeState(_grounded);
            }
        }
    }

    public void DeathCallback(DeathStateResult results)
    {
        foreach (var explosionTouched in results.CollidedWith)
        {
            if (explosionTouched.gameObject.tag == "Enemy")
            {
                explosionTouched.gameObject.GetComponent<IDamageManagement>().TakeDamage(10);
            }
        }
    }

    public void PostMortermResult()
    {
        //TODO : postmorterm logic, throw back to the pool
        Destroy(this.gameObject);
    }
}


