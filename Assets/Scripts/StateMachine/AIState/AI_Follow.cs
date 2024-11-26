using UnityEngine;

public class AI_Follow : AI_Base
{
    private GameObject player;
    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);

        navMeshAgent.isStopped = false;
        AIController_Fellow fellow = sender as AIController_Fellow;
        player = fellow.Owner;
    }

    public override void Execute(AIController sender)
    {
        if (player == null)
            return;

        navMeshAgent.SetDestination(player.transform.position);
        animator.SetFloat("SpeedY", navMeshAgent.velocity.magnitude);
    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);
    }
}