using UnityEngine;

public class AI_Down : AI_Base
{
    private float damageDelay = 1.0f;
    private float damageDelayRandom = 0.5f;
    private float currentCoolTime = 0.0f;
    private bool bCheckCoolTime = false;

    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);
        Debug.Log("Enter Down");

        if (navMeshAgent.enabled)
            navMeshAgent.enabled = false;

        bCheckCoolTime = false;
        character.OnEndDamaged += OnEndDamaged;
        character.OnBeginDamaged += OnBeginDamaged;
    }

    public override void Execute(AIController sender)
    {
        animator.SetFloat("SpeedY", 0.0f);

        if (bCheckCoolTime == false)
            return;

        currentCoolTime -= Time.fixedDeltaTime;

        if (currentCoolTime > 0.0f)
            return;

        sender.SetState(AIController.State.Getup);
    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);
        Debug.Log("Exit Down");
        character.OnBeginDamaged -= OnBeginDamaged;
        character.OnEndDamaged -= OnEndDamaged;
    }

    private void OnBeginDamaged()
    {
        bCheckCoolTime = false;
    }

    private void OnEndDamaged()
    {
        Debug.Log($"{sender.name} : End_Damaged");
        currentCoolTime = damageDelay + Random.Range(-damageDelayRandom, +damageDelayRandom);
        bCheckCoolTime = true;
    }
}