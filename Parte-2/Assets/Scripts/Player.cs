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
    private bool Jumping;
    private bool LookingRight;
    private bool CurrentlyAttacking;
    private int AirbornCheckFrame;



    // Use this for initialization
    void Start () {
        AirbornCheckFrame = 10;
        Jumping = false;
        LookingRight = true;
        CurrentlyAttacking = false;
        PlayerHealth = MaxHealth;
	}

    // Update is called once per frame
    void Update() {
        if (Jumping)
            CheckIfGrounded();

        if (!CurrentlyAttacking)
        {

            var directionX = Input.GetAxis("Horizontal");
            var directionZ = Input.GetAxis("Vertical");
            
            if ((directionX > 0 && !LookingRight) ||
            directionX < 0 && LookingRight)
                this.transform.RotateAround(this.transform.position, Vector3.up, 180f);
            this.transform.position += (new Vector3(
                directionX, 0, directionZ
                ) * Time.deltaTime * MoveSpeed);
        }
        if (Input.GetButtonDown("Fire1") && !CurrentlyAttacking)
        {
            ProcessAttack();
            
        }

        else if (Input.GetButtonDown("Jump") && !Jumping &&
            !CurrentlyAttacking)
        {
            ExecuteJump();
        }
    }

    private void FixedUpdate()
    {
        LookingRight = gameObject.transform.eulerAngles.y == 0;
    }

    IEnumerator FinishAttackIn(float time)
    {
        yield return new WaitForSeconds(time);
        CurrentlyAttacking = false;
        this.GetComponent<Animator>().Play(playerAnimations.PlayerIdle.name, -1, 0f);
    }

    void ProcessAttack()
    {
        CurrentlyAttacking = true;
        if (Jumping)
            ExecuteAirAttack();
        else
            ExecutePunch();
    }


    void ExecutePunch()
    {
        this.GetComponent<Animator>().Play(playerAnimations.PlayerAttacking.name, -1, 0f);
        StartCoroutine(FinishAttackIn(playerAnimations.PlayerAttacking.length));
    }

    void ExecuteAirAttack()
    {
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
        Jumping = true;
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
                    Jumping = false;
                    CurrentlyAttacking = false;
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
