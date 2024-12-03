using UnityEngine;

/// <summary>
/// 공중으로 띄워진 공격을 맞았을 때의 상태
/// </summary>
public class AI_Air : AI_Base
{
    private float damageDelay = 1.0f;
    private float currentCoolTime = 0.0f;
    private bool bCheckCoolTime = false;

    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);

        Debug.Log($"{sender.name} : Enter Air");
        Debug.Log($"{sender.name} : {prevState.ToString()}");

        if (navMeshAgent.enabled)
        {
            navMeshAgent.isStopped = true;
            navMeshAgent.enabled = false;
        }

        bCheckCoolTime = false;

        character.OnEndDamaged += OnEndDamaged;
        character.OnBeginDamaged += OnBeginDamaged;

        Player player = GameManager.Instance.PlayerInstance;
        if (player == null)
            return;

        //공중으로 띄워졌을 때 플레이어 앞에 배치하게 하기 위한 로직
        //플레이어 전방 앞 2.0f에 평면을 하나 만든다.
        Plane plane = new Plane(player.transform.forward, player.transform.position + player.transform.forward * 2.0f);
        //현재 AI의 위치를 만든 평면 위의 가장가까운 점으로 이동시킨다.
        Vector3 position = sender.transform.position;
        Vector3 projectPosition = plane.ClosestPointOnPlane(position);
        sender.transform.position = projectPosition;
    }

    public override void Execute(AIController sender)
    {
        if (bCheckCoolTime == false)
            return;

        currentCoolTime -= Time.fixedDeltaTime;

        if (currentCoolTime > 0.0f)
            return;

        //땅으로 떨어지는 것을 판단
        if (Physics.Raycast(sender.transform.position + Vector3.up * 0.3f, Vector3.down, 0.4f, 1 << 7))
        {
            sender.SetState(AIController.State.Getup);
        }

    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);
        Debug.Log($"{sender.name} : Exit Air");
        character.OnBeginDamaged -= OnBeginDamaged;
        character.OnEndDamaged -= OnEndDamaged;
        rigidbody.drag = 0.0f;
    }

    private void OnBeginDamaged()
    {
        bCheckCoolTime = false;
    }

    private void OnEndDamaged()
    {
        Debug.Log($"{sender.name} : End_Damaged");
        currentCoolTime = damageDelay;
        rigidbody.drag = 0.0f;
        bCheckCoolTime = true;
    }
}