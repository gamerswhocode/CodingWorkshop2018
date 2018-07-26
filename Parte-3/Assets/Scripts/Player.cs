using System.Collections;
using UnityEngine;

[System.Serializable]
public class ActorAnimations
{
    [SerializeField]
    internal AnimationClip PlayerDamage;
    [SerializeField]
    internal AnimationClip PlayerIdle;
    [SerializeField]
    internal AnimationClip PlayerJumping;
    [SerializeField]
    internal AnimationClip PlayerAttacking;
}

public class Player : MonoBehaviour {

    enum CharacterState
    {
        Idle,
        Attacking,
        Jumping,
        JumpAttack,
        Damage,
        Dead
    }


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
    [Header("Player Attributes")]
    [Range(0, 100)]
    public int MaxHealth;
    private int PlayerHealth;
    [Range(0, 100)]
    public int MaxStamina;
    private int PlayerStamina;
    public float StaminaRecoveryRate;
    [Space(20)]
    [Header("Attack Attributes")]
    public float AttackDamage;
    public float AirAttackDamage;
    [SerializeField]
    public ActorAnimations playerAnimations;

    [Space(20)]
    [Header("Actor States")]
    [SerializeField]
    private CharacterState ActorState;
    private bool LookingRight;
    private int AirbornCheckFrame;



    // Use this for initialization
    void Start () {
        ActorState = CharacterState.Idle;
        AirbornCheckFrame = 10;
        LookingRight = true;
        PlayerHealth = MaxHealth;
	}

    // Update is called once per frame
    void Update() {

        if (ActorState == CharacterState.Jumping
            || ActorState == CharacterState.JumpAttack)
            CheckIfGrounded();

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
        if (Input.GetButtonDown("Fire1") && !CurrentlyAttacking())
        {
            ProcessAttack();
            
        }

        else if (Input.GetButtonDown("Jump") && ActorState != CharacterState.Jumping &&
            !CurrentlyAttacking())
        {
            ExecuteJump();
        }
    }

    public bool CurrentlyAttacking()
    {
        return ActorState == CharacterState.JumpAttack ||
            ActorState == CharacterState.Attacking;
    }

    private void FixedUpdate()
    {
        LookingRight = gameObject.transform.eulerAngles.y == 0;
    }

    IEnumerator FinishAttackIn(float time)
    {
        yield return new WaitForSeconds(time);
        ActorState = CharacterState.Idle;
        this.GetComponent<Animator>().Play(playerAnimations.PlayerIdle.name, -1, 0f);
    }

    void ProcessAttack()
    {
        if (ActorState == CharacterState.Jumping)
            ExecuteAirAttack();
        else
            ExecutePunch();
    }

    void ExecutePunch()
    {
        ActorState = CharacterState.Attacking;
        this.GetComponent<Animator>().Play(playerAnimations.PlayerAttacking.name, -1, 0f);
        StartCoroutine(FinishAttackIn(playerAnimations.PlayerAttacking.length));
    }

    void ExecuteAirAttack()
    {
        ActorState = CharacterState.JumpAttack;
        this.GetComponent<Rigidbody>().AddForce(
            new Vector3(LookingRight ? 50f : -50f, -50f) * 20f);
    }


    [ContextMenu("TakeDamage")]
    public void TakeDamage()
    {
        PlayerHealth--;
    }

    void ExecuteJump()
    {
        this.GetComponent<Rigidbody>().AddForce(Vector3.up * JumpSpeed);
        ActorState = CharacterState.Jumping;
    }

    private void CheckIfGrounded()
    {
        if (AirbornCheckFrame <= 0)
        {
            var bottomCollider = this.GetComponent<Collider>();
            Collider[] col = Physics.OverlapBox(bottomCollider.bounds.center, bottomCollider.bounds.extents, bottomCollider.transform.rotation);
            foreach (var c in col)
            {
                if (c.gameObject.layer == LayerMask.NameToLayer("Scenario"))
                {
                    ActorState = CharacterState.Idle;
                }
            }
        }
        else
        {
            AirbornCheckFrame--;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            Destroy(this.gameObject);
        }
    }
}


public static class ActorExtednedMethods {

    public static void FlipCharacter(this GameObject actor)
    {
        actor.transform.RotateAround(actor.transform.position, Vector3.up, 180f);
    }
}