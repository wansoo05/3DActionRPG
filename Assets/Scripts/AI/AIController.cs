using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(PerceptionComponent))]
[RequireComponent(typeof(NavMeshAgent))]
public abstract class AIController : MonoBehaviour
{
    public enum State
    {
        Wait = 0, Patrol, Follow, Alert, Approach, Equip, Retreat, Action, 
        Down, Air, Getup, Damaged, Dead,
    }

    /// <summary>
    /// ���� ����(radius)
    /// </summary>
    [SerializeField]
    protected float attackRange = 1.5f;

    /// <summary>
    /// ���� �� �⺻ ������ �ð�(��)
    /// </summary>
    [SerializeField]
    protected float attackDelay = 1.0f;

    /// <summary>
    /// attackDelay�� ���� ��ȭ ��(��)
    /// </summary>
    [SerializeField]
    protected float attackDelayRandom = 0.5f;

    /// <summary>
    /// �ڽ��� ���� ����
    /// </summary>
    [SerializeField]
    protected WeaponType myWeaponType;
    public WeaponType MyWeaponType { get => myWeaponType; }

    protected PerceptionComponent perception;
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    protected WeaponComponent weapon;
    protected PatrolComponent patrolComponent;
    private CapsuleCollider capsule;

    protected Dictionary<State, IState<AIController>> dicState = new Dictionary<State, IState<AIController>>();
    protected Dictionary<IState<AIController>, State> dicFind = new Dictionary<IState<AIController>, State>();
    protected StateMachine<AIController> stateMachine;

    /// <summary>
    /// �⺻ ���� ���� ����
    /// </summary>
    protected int maxAction = 1;
    public int MaxAction { get => maxAction; }


    public State GetState() => dicFind[stateMachine.CurrentState];
    public void SetState(State state) => stateMachine.SetState(dicState[state]);

    protected virtual void Awake()
    {
        perception = GetComponent<PerceptionComponent>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        weapon = GetComponent<WeaponComponent>();
        capsule = GetComponent<CapsuleCollider>();
        patrolComponent = GetComponent<PatrolComponent>();

        IState<AIController> wait = new AI_Wait();
        IState<AIController> equip = new AI_Equip();
        IState<AIController> alert = new AI_Alert();
        IState<AIController> approach = new AI_Approach();
        IState<AIController> action = new AI_Action();
        IState<AIController> damaged = new AI_Damaged();
        IState<AIController> patrol = new AI_Patrol();
        IState<AIController> down = new AI_Down();
        IState<AIController> air = new AI_Air();
        IState<AIController> getup = new AI_Getup();
        IState<AIController> dead = new AI_Dead();
        IState<AIController> follow = new AI_Follow();
        IState<AIController> retreat = new AI_Retreat();


        dicState.Add(State.Wait, wait);
        dicState.Add(State.Equip, equip);
        dicState.Add(State.Alert, alert);
        dicState.Add(State.Approach, approach);
        dicState.Add(State.Action, action);
        dicState.Add(State.Damaged, damaged);
        dicState.Add(State.Patrol, patrol);
        dicState.Add(State.Down, down);
        dicState.Add(State.Air, air);
        dicState.Add(State.Getup, getup);
        dicState.Add(State.Dead, dead);
        dicState.Add(State.Follow, follow);
        dicState.Add(State.Retreat, retreat);

        dicFind.Add(wait, State.Wait);
        dicFind.Add(equip, State.Equip);
        dicFind.Add(alert, State.Alert);
        dicFind.Add(approach, State.Approach);
        dicFind.Add(action, State.Action);
        dicFind.Add(damaged, State.Damaged);
        dicFind.Add(patrol, State.Patrol);
        dicFind.Add(down, State.Down);
        dicFind.Add(air, State.Air);
        dicFind.Add(getup, State.Getup);
        dicFind.Add(dead, State.Dead);
        dicFind.Add(follow, State.Follow);
        dicFind.Add(retreat, State.Retreat);

        stateMachine = new StateMachine<AIController>(this, wait);
    }

    protected virtual void OnEnable()
    {
        if (stateMachine != null)
            SetState(State.Wait);

        weapon.OnEndEquip += OnEndEquip;
        weapon.OnEndDoAction += OnEndDoAction;
    }

    protected virtual void OnDisable()
    {
        weapon.OnEndEquip -= OnEndEquip;
        weapon.OnEndDoAction -= OnEndDoAction;
    }

    protected virtual void Start()
    {

    }

    protected virtual void FixedUpdate()
    {
        stateMachine.Execute();
        Update_State();
    }

    /// <summary>
    /// Ÿ���� Ư�� ���� �ȿ� �ִ��� �˾Ƴ��� �Լ�
    /// </summary>
    /// <returns></returns>
    protected bool IsTargetInRange(float range)
    {
        GameObject target = perception.GetPercievedTarget();
        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (distance < range)
            return true;

        return false;
    }

    /// <summary>
    /// ���� ���¿��� ������Ʈ ������ ����� ������
    /// </summary>
    /// <returns></returns>
    protected abstract bool CheckMode();

    /// <summary>
    /// AI�� �⺻���� ���� ������Ʈ ����
    /// </summary>
    protected abstract void Update_State();

    protected virtual void OnEndDoAction()
    {
        print("EndDoAction");
        SetState(State.Wait);
    }

    private void OnEndEquip()
    {
        print("EndEquip");
        SetState(State.Wait);
    }

    public virtual bool DamageCheck(State curState, HitType damageType)
    {
        int curType = 0;

        switch (curState)
        {
            case State.Damaged:
                curType = 0;
                break;
            case State.Down:
                curType = 1;
                break;
            case State.Air:
                curType = 2;
                break;
            case State.Getup:
                curType = 3;
                break;
            case State.Retreat:
                curType = 4;
                break;
            default:
                curType = 5;
                break;
        }

        if ((int)damageType > 3)
            damageType = HitType.Down;

        return DamageHelpers.damageTable[curType, (int)damageType];
    }
}   