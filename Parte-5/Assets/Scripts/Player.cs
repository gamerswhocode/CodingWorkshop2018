using System.Collections;
using UnityEngine;

public class Player : ActorCore {


    [Header("Character Stuff")]
    public string CharacterName;
    [TextArea]
    public string CharacterDescription;
    [Multiline]
    public string CharacterBackStory;
    [Space(20)]
    [Header("Speed")]
    [Range(0, 12)]
    [Tooltip("Zoom Zoom")]
    public float MoveSpeed;
    [Range(250, 500)]
    public float JumpSpeed;
   
    [Space(20)]
    [Header("Attack Attributes")]
    public float AttackDamage;
    public float AirAttackDamage;

    [Space(20)]
    [Header("Player States")]
    PunchState Punch;
    DiveKickState DiveKick;
    KnockbackState Knockback;


    internal override void Awake()
    {
        base.Awake();
        var playerAnimator = GetComponent<Animator>();
        var playerRigidBody = GetComponent<Rigidbody>();
        var playerCollider = GetComponent<Collider>();
        Punch = new PunchState(playerAnimator, actorAnimations.ActorAttacking,
            DonePunching);
        Idle = new IdleState(playerAnimator, actorAnimations.ActorIdle);
        Grounded = new GroundedState();
        Jump = new JumpState(playerCollider, 
           playerRigidBody , JumpSpeed, EvalJumpData);
        DiveKick = new DiveKickState(playerRigidBody, 20f, ActorLookingRight);
        Knockback = new KnockbackState(playerCollider, 5f, ActorLookingRight, EvalKnockbackResult);
        actionState.ChangeState(Idle);
        movementState.ChangeState(Grounded);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateStates();
        EvalInputPriority();
    }

    private void EvalInputPriority()
    {
        EvalMovement();

        if (IsGoingToAttack())
        {
            ProcessAttack();
        }
        else if (IsGoingToJump())
        {
            ExecuteJump();
        }
    }

    private void UpdateStates()
    {
        CurrentAction = actionState.GetCurrentlyRunningState().Name;
        CurrentMovement = movementState.GetCurrentlyRunningState().Name;
        actionState.ExecuteStateUpdate();
        movementState.ExecuteStateUpdate();
    }


    public void EvalJumpData(JumpResults results)
    {
        foreach (var c in results.CollidedWith)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Scenario"))
            {
                movementState.ChangeState(Grounded);
                actionState.ChangeState(Idle);
            }
            //else if collided with wall or object, figure out what to do.
        }
    }

    public void DonePunching(PunchResults results)
    {
        actionState.SwitchToPreviousState();
    }

    void ProcessAttack()
    {
        if (movementState.GetCurrentlyRunningState() == Jump.GetType())
        {
            var playerBody = GetComponent<Rigidbody>();
            if(playerBody != null)
                actionState.ChangeState(DiveKick);
        }
        else
        {
            actionState.ChangeState(Punch);
        }
    }

    private bool IsGoingToAttack()
    {
        return Input.GetButtonDown("Fire1") && !CurrentlyAttacking()
            && PlayerCanMove();
    }

    private bool IsGoingToJump()
    {
        return Input.GetButtonDown("Jump") &&
                    movementState.GetCurrentlyRunningState() != Jump.GetType() &&
                    !CurrentlyAttacking() &&
                    PlayerCanMove();
    }

    private void EvalMovement()
    {
        if (!CurrentlyAttacking() && PlayerCanMove())
        {

            var directionX = Input.GetAxis("Horizontal");
            var directionZ = Input.GetAxis("Vertical");

            if ((directionX > 0 && !ActorLookingRight()) ||
            directionX < 0 && ActorLookingRight())
            {
                gameObject.FlipCharacter();
            }
            this.transform.position += (new Vector3(
                directionX, 0, directionZ
                ) * Time.deltaTime * MoveSpeed);
        }
    }

    public bool CurrentlyAttacking()
    {
        return actionState.GetCurrentlyRunningState()
            == Punch.GetType() ||
            actionState.GetCurrentlyRunningState() == DiveKick.GetType();
    }

    public bool PlayerCanMove()
    {
        return movementState.GetCurrentlyRunningState() != Knockback.GetType();
    }

    void ExecuteJump()
    {
        movementState.ChangeState(Jump);
    }

    private void KnockbackPlayer(GameObject hitBy)
    {
        StartCoroutine(BecomeInvulnerable(1.5f));
        this.gameObject.KnockBackObject(hitBy, new Vector3(10f, 3f));
        movementState.ChangeState(Knockback);
    }

    public void EvalKnockbackResult(KnockbackResultData results)
    {
        foreach (var c in results.CollidedWith)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Scenario"))
            {
                movementState.ChangeState(Grounded);
                actionState.ChangeState(Idle);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            //base.TakeDamage(1);
            if(TakeDamage(1))
                KnockbackPlayer(other.gameObject);
        }
    }

    /*
    public override bool TakeDamage(int dmg)
    {
        return true;
    }
    */
}


