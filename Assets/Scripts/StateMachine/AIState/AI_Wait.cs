using UnityEngine;
using UnityEngine.AI;

public class AI_Wait : AI_Base
{
    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);
        Debug.Log("Enter Wait");
        
        navMeshAgent.isStopped = true;
    }

    public override void Execute(AIController sender)
    {
        animator.SetFloat("SpeedY", 0.0f);
    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);
        Debug.Log("Exit Wait");
    }
}