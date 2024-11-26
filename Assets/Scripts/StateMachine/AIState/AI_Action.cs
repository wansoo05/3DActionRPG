using UnityEngine;

public class AI_Action : AI_Base
{
    private float delayAction = 0.1f;
    private float delayTimer = 0.0f;
    private bool isComboActive = false;
    private int comboCount = 2;
    private AIContorller_Boss boss;
    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);

        Debug.Log($"{sender.name} : Enter Action");

        navMeshAgent.isStopped = true;
        delayTimer = 0.0f;
        delayAction = 0.1f;
        isComboActive = true;
        comboCount = 2;

        GameObject target = perception.GetPercievedTarget();

        if (target != null)
            sender.transform.LookAt(target.transform);

        boss = sender as AIContorller_Boss;
        if (boss != null)
        {
            if (boss.IsTargetInMeleeRange())
            {
                int randomAction = Random.Range(1, boss.MaxAction + 1);
                weapon.DoAction(randomAction);
            }
            else
            {
                int randomAction = Random.Range(0, boss.RangeAction);
                weapon.DoAction(boss.MaxAction + randomAction);
            }
        }
    }

    public override void Execute(AIController sender)
    {
        animator.SetFloat("SpeedY", 0.0f);

        if (boss != null)
            return;

        if (comboCount > 0)
        {
            if (isComboActive)
            {
                weapon.DoAction();
                comboCount--;
                isComboActive = false;
            }

            delayTimer += Time.deltaTime;
            if (delayTimer > delayAction)
            {
                isComboActive = true;
                delayTimer = 0.0f;
            }
        }
    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);

        Debug.Log($"{sender.name} : Exit Action");
        Debug.Log($"{sender.name} : NextState {nextState.ToString()}");
        if (CheckMode(nextState))
        {
            animator.Play($"{weapon.Type}.Blend Tree", 0);
            weapon.Secure_EndDoAction();
        }
    }

    private bool CheckMode(IState<AIController> state)
    {
        bool bCheck = false;
        bCheck |= (state as AI_Damaged != null);
        bCheck |= (state as AI_Air != null);
        bCheck |= (state as AI_Down != null);
        bCheck |= (state as AI_Dead != null);
        bCheck |= (state as AI_Retreat != null);

        return bCheck;
    }
}