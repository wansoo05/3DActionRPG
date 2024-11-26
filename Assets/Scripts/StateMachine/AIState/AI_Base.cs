using UnityEngine;
using UnityEngine.AI;

public abstract class AI_Base : IState<AIController>
{
    protected NavMeshAgent navMeshAgent;
    protected Animator animator;
    protected PerceptionComponent perception;
    protected WeaponComponent weapon;
    protected StateMachine<AIController> stateMachine;
    protected PatrolComponent patrol;
    protected Character character;
    protected AIController sender;
    protected Rigidbody rigidbody;

    public virtual void Enter(AIController sender, IState<AIController> prevState)
    {
        if (animator == null)
        {
            animator = sender.GetComponent<Animator>();
        }

        if (perception == null)
        {
            perception = sender.GetComponent<PerceptionComponent>();
        }

        if (navMeshAgent == null)
        {
            navMeshAgent = sender.GetComponent<NavMeshAgent>();
        }

        if (weapon == null)
        {
            weapon = sender.GetComponent<WeaponComponent>();
        }

        if (character == null)
        {
            character = sender.GetComponent<Character>();
        }

        if (patrol == null)
        {
            patrol = sender.GetComponent<PatrolComponent>();
        }

        if (rigidbody == null)
            rigidbody = sender.GetComponent<Rigidbody>();

        if (this.sender == null)
        {
            this.sender = sender;
        }
    }

    public abstract void Execute(AIController sender);

    public virtual void Exit(AIController sender, IState<AIController> nextState)
    {

    }
}