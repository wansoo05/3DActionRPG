using Cinemachine;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.UI;
using UnityEngine.UIElements;

public abstract class Skill : MonoBehaviour
{
    [SerializeField]
    protected SkillType type;

    [SerializeField]
    protected DoActionData doActionData;

    [SerializeField]
    public Sprite iconImage;

    [SerializeField]
    protected float coolTime;

    [SerializeField]
    protected GameObject auraPrefab;

    [SerializeField]
    private float useMagicPoint;
    public float UseMagicPoint { get => useMagicPoint; }

    public SkillType Type { get => type; }

    protected Collider[] colliders;
    private List<GameObject> hittedList;
    protected CinemachineImpulseSource impulse;
    protected CinemachineBrain brain;
    protected GameObject rootObject;
    protected StateComponent state;
    protected Animator animator;
    private float currentCoolTime = 0.0f;
    private WeaponComponent weapon;
    private Transform auraTransform;

    protected int hitIndex = -1;
    protected TrailRenderer trail;

    private GameObject auraObject;

    protected virtual void Reset()
    {

    }

    protected virtual void Awake()
    {
        rootObject = transform.root.gameObject;
        Debug.Assert(rootObject != null);

        animator = rootObject.GetComponent<Animator>();
        state = rootObject.GetComponent<StateComponent>();
        colliders = GetComponentsInChildren<Collider>();
        hittedList = new List<GameObject>();
        impulse = GetComponent<CinemachineImpulseSource>();
        weapon = rootObject.GetComponent<WeaponComponent>();
    }

    protected virtual void Start()
    {
        End_Collision();
        auraTransform = rootObject.transform.FindChildByName("Aura");
    }

    private void OnEnable()
    {
        brain = Camera.main.GetComponent<CinemachineBrain>();
        currentCoolTime = 0.0f;
    }

    protected virtual void Update()
    {
        Update_CoolTime();

        if (weapon.Type != doActionData.weaponType)
            return;

        UIController.Instance.AdjustSkillCoolTime(type, currentCoolTime, coolTime);
    }

    private void Update_CoolTime()
    {
        if (currentCoolTime <= 0.0f)
        {
            currentCoolTime = 0.0f;
            return;
        }

        currentCoolTime -= Time.deltaTime;
    }

    public bool CheckCoolTime()
    {
        return currentCoolTime > 0.0f;
    }

    public virtual void DoAction()
    {
        state.SetActionMode();
        CheckMovable();
        
    }

    public virtual void Begin_DoAction()
    {
        if (auraPrefab == null) return;
        if (auraTransform == null) return;

        auraObject = Instantiate(auraPrefab, auraTransform);
    }

    public virtual void End_DoAction()
    {
        state.SetIdleMode();

        PlayerMovingComponent moving = rootObject.GetComponent<PlayerMovingComponent>();
        if (moving != null)
        {
            moving.Move();
        }

        currentCoolTime = coolTime;
        animator.ResetTrigger("Skill");
        hitIndex = -1;
        if (auraObject != null)
            Destroy(auraObject);
    }

    public virtual void Begin_Collision(AnimationEvent e)
    {
        foreach (Collider collider in colliders)
            collider.enabled = true;

        hitIndex++;

    }

    public virtual void End_Collision()
    {
        foreach (Collider collider in colliders)
            collider.enabled = false;

        hittedList.Clear();
    }

    public virtual void Shoot_Projectile(AnimationEvent e)
    {

    }

    public virtual void Play_Particle()
    {
    }

    protected void CheckMovable()
    {
        PlayerMovingComponent moving = rootObject.GetComponent<PlayerMovingComponent>();

        if (moving == null)
            return;

        if (doActionData.bCanMove == false)
            moving.Stop();
        else
            moving.Move();
    }

    public virtual void Play_Impulse()
    {
        if (impulse == null)
            return;

        if (doActionData.hittingDatas[hitIndex].ImpulseSettings == null)
            return;

        if (doActionData.hittingDatas[hitIndex].ImpulseDirection.magnitude == 0)
            return;

        CinemachineVirtualCamera camera = brain.ActiveVirtualCamera as CinemachineVirtualCamera;
        if (camera != null)
        {
            CinemachineImpulseListener listener = camera.GetComponent<CinemachineImpulseListener>();
            listener.m_ReactionSettings.m_SecondaryNoise = doActionData.hittingDatas[hitIndex].ImpulseSettings;
        }

        impulse.GenerateImpulse(doActionData.hittingDatas[hitIndex].ImpulseDirection);
    }

    public virtual void Play_SlashSound()
    {
        
    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag(rootObject.tag))
            return;
        if (hittedList.Contains(other.gameObject) == true)
            return;


        hittedList.Add(other.gameObject);

        IDamagable damage = other.gameObject.GetComponent<IDamagable>();

        if (damage == null)
            return;

        Vector3 hitPoint = Vector3.up;

        Collider enabledCollider = null;
        foreach (Collider collider in colliders)
        {
            if (collider.enabled == true)
            {
                enabledCollider = collider;

                break;
            }
        }

        Play_Impulse();

        if (type == SkillType.FistAir)
            lookRotate(other.gameObject);

        if (enabledCollider != null)
        {
            hitPoint = enabledCollider.ClosestPoint(other.transform.position);
            hitPoint = other.transform.InverseTransformPoint(hitPoint);
        }
        damage.OnDamage(rootObject, doActionData.weapon , hitPoint, doActionData.hittingDatas[hitIndex]);
    }
    private void lookRotate(GameObject target)
    {
        Vector3 direction = target.transform.position - rootObject.transform.position;
        direction.y = 0.0f;
        Quaternion q = Quaternion.FromToRotation(rootObject.transform.forward, direction.normalized);
        rootObject.transform.rotation *= q;
    }
}