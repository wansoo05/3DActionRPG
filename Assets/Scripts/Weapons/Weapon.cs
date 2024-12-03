using System;
using UnityEngine;

/// <summary>
/// 무기의 공격 행동 데이터
/// </summary>
[Serializable]
public class DoActionData
{
    public HittingData[] hittingDatas;
    public Weapon weapon;
    public WeaponType weaponType;
    public bool bCanMove;
}

public abstract class Weapon : MonoBehaviour
{
    [SerializeField]
    protected WeaponType type;

    [SerializeField]
    protected DoActionData[] doActionDatas;

    public Sprite iconImage;

    public WeaponType Type { get => type; }

    private bool bEquipping;
    public bool Equipping { get => bEquipping; }

    protected GameObject rootObject;

    protected StateComponent state;
    protected Animator animator;

    private SkillComponont skill;

    protected int hitIndex = -1;
    protected int actionIndex = 0;

    protected TrailRenderer trail;
    /// <summary>
    /// 플레이어인지 아닌지 구분해주는 변수
    /// </summary>
    protected bool isPlayer;

    protected virtual void Reset()
    {

    }

    protected virtual void Awake()
    {
        //무기 주인 캐릭터 찾기
        rootObject = transform.parent.gameObject;
        while (rootObject.GetComponent<Character>() == null)
        {
            rootObject = transform.parent.gameObject;
        }
        Debug.Assert(rootObject != null);

        animator = rootObject.GetComponent<Animator>();
        state = rootObject.GetComponent<StateComponent>();
        isPlayer = rootObject.GetComponent<Player>() != null ? true : false;
    }

    protected virtual void Start()
    {
        trail = GetComponentInChildren<TrailRenderer>();
        if (trail != null)
            trail.enabled = false;
    }

    protected virtual void Update()
    {
    }

    public void Equip()
    {
        state.SetEquipMode();
    }
    public virtual void Begin_Equip()
    {
        bEquipping = true;
    }

    public virtual void End_Equip()
    {
        bEquipping = false;
        state.SetIdleMode();
    }

    public virtual void UnEquip()
    {

    }

    public virtual void DoAction(int actionIndex = 0)
    {
        if (actionIndex != 0)
            this.actionIndex = actionIndex - 1;

        if (state.IdleMode == false && state.ActionMode == false)
            return;

        CheckMovable(this.actionIndex);

        state.SetActionMode();
    }

    public virtual void Begin_DoAction()
    {

    }

    public virtual void End_DoAction()
    {
        state.SetIdleMode();

        PlayerMovingComponent moving = rootObject.GetComponent<PlayerMovingComponent>();
        if (moving != null)
        {
            moving.Move();
        }
        animator.ResetTrigger("Attack");
        hitIndex = -1;
    }
    
    public virtual void Shoot_Projectile(AnimationEvent e)
    {

    }

    public virtual void Play_Particle()
    {

    }

    /// <summary>
    /// 해당 공격 행동에 맞는 움직임을 제어
    /// </summary>
    /// <param name="actionIndex"></param>
    protected void CheckMovable(int actionIndex)
    {
        PlayerMovingComponent moving = rootObject.GetComponent<PlayerMovingComponent>();
        
        if (moving == null)
            return;

        if (doActionDatas[actionIndex].bCanMove == false)
            moving.Stop();
        else
            moving.Move();
    }

    public virtual void DoEvade()
    {
        state.SetEvadeMode();
    }
}