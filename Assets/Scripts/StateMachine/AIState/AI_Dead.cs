using UnityEngine;

public class AI_Dead : AI_Base
{
    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);
        Debug.Log($"{sender.name} : Enter Dead");
        if (navMeshAgent.enabled == true)
            navMeshAgent.isStopped = true;
    }

    public override void Execute(AIController sender)
    {

    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);
        Debug.Log($"{sender.name} : Exit Dead");
    }
}