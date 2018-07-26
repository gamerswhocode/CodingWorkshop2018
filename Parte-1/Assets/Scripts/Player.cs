using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour {

    public float moveSpeed;
    public float jumpSpeed;

    public int MaxHealth;
    public int PlayerHealth;
    public int MaxStamina;
    public int PlayerStamina;
    public float StaminaRecoveryRate;

    public float AttackDamage;
    public float AirAttackDamage;

    public AnimationClip PlayerDamage;
    public AnimationClip PlayerIdle;
    public AnimationClip PlayerJumping;
    public AnimationClip PlayerAttacking;

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
                ) * Time.deltaTime * moveSpeed);
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
        this.GetComponent<Animator>().Play(PlayerIdle.name, -1, 0f);
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
        this.GetComponent<Animator>().Play(PlayerAttacking.name, -1, 0f);
        StartCoroutine(FinishAttackIn(PlayerAttacking.length));
    }

    void ExecuteAirAttack()
    {
        this.GetComponent<Rigidbody>().AddForce(
            new Vector3(LookingRight ? 50f : -50f, -50f) * 20f);
    }

    void ExecuteJump()
    {
        this.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpSpeed);
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
