using UnityEngine;

public class AI_Damaged : AI_Base
{
    private float damageDelay = 1.0f;
    private float damageDelayRandom = 0.5f;
    private float currentCoolTime = 0.0f;
    private bool bCheckCoolTime = false;
    
    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);

        Debug.Log($"{sender.name} : Enter Damaged");
        navMeshAgent.isStopped = true;
        character.OnEndDamaged += OnEndDamaged;
        character.OnBeginDamaged += OnBeginDamaged;
        
        bCheckCoolTime = false;
    }

    public override void Execute(AIController sender)
    {
        animator.SetFloat("SpeedY", 0.0f);

        if (bCheckCoolTime == false)
            return;

        currentCoolTime -= Time.deltaTime;

        if (currentCoolTime > 0.0f)
            return;

        Debug.Log($"{sender.name} : ToApproach");
        sender.SetState(AIController.State.Approach);
    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);
        Debug.Log($"{sender.name} : Exit Damaged");
        character.OnBeginDamaged -= OnBeginDamaged;
        character.OnEndDamaged -= OnEndDamaged;
    }

    /// <summary>
    /// 데미지 모드일 때 또 피격 당하면 다시 쿨타임을 주기위한 애니메이션 이벤트
    /// </summary>
    private void OnBeginDamaged()
    {
        bCheckCoolTime = false;
    }

    private void OnEndDamaged()
    {
        Debug.Log($"{sender.name} : OnEndDamaged");
        currentCoolTime = damageDelay + Random.Range(-damageDelayRandom, +damageDelayRandom);
        Debug.Log($"{sender.name} CoolTime : {currentCoolTime}");
        bCheckCoolTime = true;
    }

}