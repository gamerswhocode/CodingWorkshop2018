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
        stateMachine.ChangeState(Idle);
    }

    // Update is called once per frame
    void Update()
    {
        stateMachine.ExecuteStateUpdate();
        if (ActorState == CharacterState.Jumping
            || ActorState == CharacterState.JumpAttack)
            CheckIfGrounded();

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

    public void DonePunching(PunchResults results)
    {
        Debug.Log("Done punching");
        stateMachine.SwitchToPreviousState();
    }

    void ProcessAttack()
    {
        if (ActorState == CharacterState.Jumping)
            ExecuteAirAttack();
        else
        {
            stateMachine.ChangeState(Punch);
        }
    }

    void ExecutePunch()
    {
        ActorState = CharacterState.Attacking;
        this.GetComponent<Animator>().Play(actorAnimations.ActorAttacking.name, -1, 0f);
        StartCoroutine(FinishAttackIn(actorAnimations.ActorAttacking.length));
    }

    IEnumerator FinishAttackIn(float time)
    {
        yield return new WaitForSeconds(time);
        ActorState = CharacterState.Idle;
        this.GetComponent<Animator>().Play(actorAnimations.ActorIdle.name, -1, 0f);
    }
    private bool IsGoingToAttack()
    {
        return Input.GetButtonDown("Fire1") && !CurrentlyAttacking();
    }

    private bool IsGoingToJump()
    {
        return Input.GetButtonDown("Jump") && ActorState != CharacterState.Jumping &&
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
        return ActorState == CharacterState.JumpAttack ||
            ActorState == CharacterState.Attacking;
    }

    void ExecuteAirAttack()
    {
        ActorState = CharacterState.JumpAttack;
        this.GetComponent<Rigidbody>().AddForce(
            new Vector3(LookingRight ? 50f : -50f, -50f) * 20f);
    }

    void ExecuteJump()
    {
        this.GetComponent<Rigidbody>().AddForce(Vector3.up * JumpSpeed);
        ActorState = CharacterState.Jumping;
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


