using UnityEngine;

public class AI_Approach : AI_Base
{
    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);
        Debug.Log($"{sender.name} : Enter Apporach");
        navMeshAgent.speed = 3.5f;
        navMeshAgent.isStopped = false;
        navMeshAgent.updateRotation = true;
    }

    public override void Execute(AIController sender)
    {
        GameObject target = perception.GetPercievedTarget();
        if (target == null)
            return;

        animator.SetFloat("SpeedY", navMeshAgent.velocity.magnitude);
        
        navMeshAgent.SetDestination(target.transform.position);
    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);

        Debug.Log($"{sender.name} : Exit Apporach");
        animator.SetFloat("SpeedY", 0.0f);
    }



}