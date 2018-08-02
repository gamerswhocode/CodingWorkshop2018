using UnityEngine;
using System.Collections;

[System.Serializable]
internal class ActorAnimations
{
    [SerializeField]
    internal AnimationClip ActorDamage;
    [SerializeField]
    internal AnimationClip ActorIdle;
    [SerializeField]
    internal AnimationClip ActorJumping;
    [SerializeField]
    internal AnimationClip ActorAttacking;
    [SerializeField]
    internal AnimationClip ActorHurting;
}

public abstract class ActorCore : MonoBehaviour, IDamageManagement
{
    [Space(20)]
    [Header("Actor States")]
    [SerializeField]
    protected CharacterState ActorState;
    protected bool LookingRight;
    private int AirbornCheckFrame;
    [SerializeField]
    private bool CurrentlyInvulnerable;

    [SerializeField]
    internal ActorAnimations actorAnimations;

    protected enum CharacterState
    {
        Idle,
        Attacking,
        Jumping,
        JumpAttack,
        Damage,
        Dead
    }

    [Space(20)]
    [Header("Player Attributes")]
    [Range(0, 100)]
    public int MaxHealth;
    [SerializeField]
    private int ActorHealth;
    [Range(0, 100)]
    public int MaxStamina;
    private int ActorStamina;
    public float StaminaRecoveryRate;

    // Use this for initialization
    void Start()
    {
        ActorState = CharacterState.Idle;
        AirbornCheckFrame = 10;
        LookingRight = true;
        ActorHealth = MaxHealth;
        CurrentlyInvulnerable = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    public virtual bool TakeDamage(int damage)
    {
        if (!CurrentlyInvulnerable)
        {
            ActorHealth -= damage;
            return true;
        }
        else
        { 
            return false;
        }
    }

    internal IEnumerator BecomeInvulnerable(float time)
    {
        CurrentlyInvulnerable = true;
        yield return new WaitForSeconds(time);
        CurrentlyInvulnerable = false;
    }

    private bool DetermineIfDead()
    {
        Debug.Log(ActorHealth);
        return ActorHealth <= 0;
    }

    private void FixedUpdate()
    {
        LookingRight = gameObject.transform.eulerAngles.y == 0;
    }

    internal void CheckIfGrounded()
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
                    AirbornCheckFrame = 10;
                }
            }
        }
        else
        {
            AirbornCheckFrame--;
        }
    }
}

public interface IDamageManagement
{
    bool TakeDamage(int damage);
}

public static class ActorExtednedMethods
{

    public static void FlipCharacter(this GameObject actor)
    {
        actor.transform.RotateAround(actor.transform.position, Vector3.up, 180f);
    }

    /// <summary>
    /// Knockbacks an object with RigidBody based on where it was hit from. 
    /// Assume knockback force is possitive on x.
    /// </summary>
    /// <param name="MyObject"></param>
    /// <param name="HitBy"></param>
    /// <param name="KnockBackForce"></param>
    public static void KnockBackObject(this GameObject MyObject, GameObject HitBy,
        Vector3 KnockBackForce)
    {
        var myPosition = MyObject.transform.position;
        var hitBy = HitBy.transform.position;

        Vector3 KnockBack = KnockBackForce;
        if (myPosition.x < hitBy.x)
        {
            KnockBack = new Vector3(-KnockBackForce.x, KnockBackForce.y);
        }

        var rigidBody = MyObject.GetComponent<Rigidbody>();
        if (rigidBody != null)
        {
            rigidBody.AddForce(KnockBack, ForceMode.Impulse);
        }
    }
}