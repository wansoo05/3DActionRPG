using UnityEngine;

public class AI_Retreat : AI_Base
{
    GameObject player;
    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);

        navMeshAgent.isStopped = true;
        Debug.Log($"{sender.name} : Enter Retreat");
        animator.SetTrigger("Warp");
    }
    public override void Execute(AIController sender)
    {

    }
    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);
        Debug.Log($"{sender.name} : Exit Retreat");
        if (CheckMode(nextState))
        {
            animator.Play($"{weapon.Type}.Blend Tree", 0);
            AIController_Range range = sender as AIController_Range;
            if (range != null)
            {
                range.End_DoWarp();
            }
        }
    }

    private bool CheckMode(IState<AIController> state)
    {
        bool bCheck = false;
        bCheck |= (state as AI_Dead != null);

        return bCheck;
    }

}