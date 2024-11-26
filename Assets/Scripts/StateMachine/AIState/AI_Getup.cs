using UnityEngine;

public class AI_Getup : AI_Base
{

    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);

        Debug.Log($"{sender.name} : Enter Getup");
        navMeshAgent.enabled = true;
        character.OnEndDamaged += OnEndDamaged;
        animator.SetTrigger("GetUp");
    }
    public override void Execute(AIController sender)
    {

    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);
        Debug.Log($"{sender.name} : Exit Getup");
        character.OnEndDamaged -= OnEndDamaged;
    }
    private void OnEndDamaged()
    {
        Debug.Log($"{sender.name} : End_Damaged");
        sender.SetState(AIController.State.Approach);
    }
}