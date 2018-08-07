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
    [SerializeField]
    internal AnimationClip ActorProyectile;
}

public abstract class ActorCore : MonoBehaviour, IDamageManagement
{
    [Space(20)]
    [Header("Actor States")]
    [SerializeField]
    private bool CurrentlyInvulnerable;

    [SerializeField]
    internal ActorAnimations actorAnimations;
    protected StateMachine actionState;
    protected StateMachine movementState;

    protected IdleState Idle;
    protected AirbornState Jump;
    protected GroundedState Grounded;

    public string CurrentAction;
    public string CurrentMovement;



    [Space(20)]
    [Header("Actor Attributes")]
    [Range(0, 100)]
    public int MaxHealth;
    [SerializeField]
    private int ActorHealth;
    [Range(0, 100)]
    public int MaxStamina;
    private int ActorStamina;
    public float StaminaRecoveryRate;

    internal virtual void Awake()
    {
        actionState = new StateMachine();
        movementState = new StateMachine();
    }

    // Use this for initialization
    void Start()
    {
        ActorHealth = MaxHealth;
        CurrentlyInvulnerable = false;
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

    internal bool DetermineIfDead()
    {
        return ActorHealth <= 0;
    }

    private void FixedUpdate()
    {
        actionState.ExecuteStateFixedUpdate();
        movementState.ExecuteStateFixedUpdate();
    }

    public bool ActorLookingRight()
    {
        return gameObject.transform.eulerAngles.y == 0;
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
            rigidBody.velocity = Vector3.zero;
            rigidBody.angularVelocity = Vector3.zero;
            rigidBody.AddForce(KnockBack, ForceMode.Impulse);
        }
    }

    public static void ThrowObject(this GameObject Thrower, Vector3 ThrowFromPosition, GameObject ObjectToThrow, bool ThrowToTheRight,
        Vector3 ThrowForce)
    {
        ObjectToThrow.transform.position = ThrowFromPosition;
        var rigidBody = ObjectToThrow.GetComponent<Rigidbody>();
        rigidBody.angularVelocity = Vector3.zero;
        rigidBody.velocity = Vector3.zero;

        Vector3 ActualThrowForce =
            (ThrowToTheRight ? ThrowForce : new Vector3(ThrowForce.x * -1, ThrowForce.y));
        rigidBody.AddForce(ActualThrowForce, ForceMode.Impulse);
    }
}