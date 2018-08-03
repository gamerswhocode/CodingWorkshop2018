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

    internal override void Awake()
    {
        base.Awake();
        var playerAnimator = GetComponent<Animator>();
        Punch = new PunchState(playerAnimator, actorAnimations.ActorAttacking,
            DonePunching);
        Idle = new IdleState(playerAnimator, actorAnimations.ActorIdle);
        Grounded = new GroundedState();
        Jump = new JumpState(this.GetComponent<Collider>(), 
            this.GetComponent<Rigidbody>(), JumpSpeed, EvalJumpData);
        actionState.ChangeState(Idle);
        movementState.ChangeState(Grounded);
    }

    // Update is called once per frame
    void Update()
    {
        actionState.ExecuteStateUpdate();
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

    public void EvalJumpData(JumpResults results)
    {
        foreach (var c in results.CollidedWith)
        {
            if (c.gameObject.layer == LayerMask.NameToLayer("Scenario"))
            {
                movementState.ChangeState(Grounded);
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
        //if (ActorState == CharacterState.Jumping)
        //    ExecuteAirAttack();
        //else
        {
            actionState.ChangeState(Punch);
        }
    }

    private bool IsGoingToAttack()
    {
        return Input.GetButtonDown("Fire1") && !CurrentlyAttacking();
    }

    private bool IsGoingToJump()
    {
        return Input.GetButtonDown("Jump") &&
                    //ActorState != CharacterState.Jumping &&
                    !CurrentlyAttacking();
    }

    private void EvalMovement()
    {
        if (!CurrentlyAttacking())
        {

            var directionX = Input.GetAxis("Horizontal");
            var directionZ = Input.GetAxis("Vertical");

            if ((directionX > 0 && !LookingRight) ||
            directionX < 0 && LookingRight)
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
            == Punch.GetType();
    }

    void ExecuteJump()
    {
        movementState.ChangeState(Jump);
    }

    private void KnockbackPlayer(GameObject hitBy)
    {
        StartCoroutine(BecomeInvulnerable(1.5f));
        this.gameObject.KnockBackObject(hitBy, new Vector3(10f, 3f));
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


