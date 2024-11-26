using UnityEngine;

/// <summary>
/// �������� ����� ������ �¾��� ���� ����
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

        //�������� ������� �� �÷��̾� �տ� ��ġ�ϰ� �ϱ� ���� ����
        //�÷��̾� ���� �� 2.0f�� ����� �ϳ� �����.
        Plane plane = new Plane(player.transform.forward, player.transform.position + player.transform.forward * 2.0f);
        //���� AI�� ��ġ�� ���� ��� ���� ���尡��� ������ �̵���Ų��.
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

        //������ �������� ���� �Ǵ�
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