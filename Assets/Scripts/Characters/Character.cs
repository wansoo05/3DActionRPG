using System;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(WeaponComponent))]
[RequireComponent(typeof(HealthPointComponent))]
[RequireComponent(typeof(StateComponent))]
public abstract class Character : MonoBehaviour, IStoppable
{
    [SerializeField]
    protected float armor = 0.0f;
    public float Armor { get => armor; set => armor = value; }

    [SerializeField]
    protected float additionalPower = 0.0f;
    public float AddionalPower { get => additionalPower; set => additionalPower = value; }

    protected Animator animator;

    protected HealthPointComponent healthPoint;
    protected WeaponComponent weapon;
    protected StateComponent state;
    protected new Rigidbody rigidbody;

    public event Action OnEndDamaged;
    public event Action OnBeginDamaged;

    protected virtual void Awake()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<StateComponent>();
        rigidbody = GetComponent<Rigidbody>();
        healthPoint = GetComponent<HealthPointComponent>();
        weapon = GetComponent<WeaponComponent>();
    }

    protected virtual void Update()
    {

    }

    protected virtual void OnEnable()
    {
        Regist_MovableStopper();
    }
    protected virtual void Start()
    {
        Regist_MovableStopper();
    }

    protected virtual void FixedUpdate()
    {
        
    }


    protected virtual void Begin_Damaged()
    {
        OnBeginDamaged?.Invoke();
    }

    protected virtual void End_Damaged()
    {
        OnEndDamaged?.Invoke();
    }

    protected virtual void OnFootStep(AnimationEvent animationEvent)
    {
    }

    Vector3 rootMotion;
    protected virtual void OnAnimatorMove()
    {
        rootMotion = animator.deltaPosition;
        transform.position += rootMotion;
    }
    

    public void Regist_MovableStopper()
    {
        if (MovableStopper.Instance != null)
        {
            MovableStopper.Instance.Regist(this);
        }
    }

    public IEnumerator Start_FrameDelay(int frame)
    {
        animator.speed = 0.0f;

        for (int i = 0; i < frame; i++)
            yield return new WaitForFixedUpdate();

        if (animator != null)
            animator.speed = 1.0f;
    }

    public void Delete_MovableStopper()
    {
        MovableStopper.Instance.Delete(this);
    }

    protected virtual void OnDisable()
    {
        Delete_MovableStopper();
        StopAllCoroutines();
    }
}