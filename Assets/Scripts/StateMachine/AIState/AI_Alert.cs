using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;

public class AI_Alert : AI_Base
{
    private GameObject target;

    private Vector3 targetPosition;
    private float currentRadius;
    private float approachModeCooltime = 0.0f;

    private Coroutine[] routines = new Coroutine[2];

    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);

        this.sender = sender;
        Debug.Log($"{sender.name} : Enter Alert");
        target = perception.GetPercievedTarget();
        if (target == null)
            return;

        navMeshAgent.speed = 1.0f;
        navMeshAgent.updateRotation = false;
        navMeshAgent.isStopped = false;

        animator.SetBool("IsGuard", true);

        currentRadius = Random.Range(2.0f, 4.0f);
        approachModeCooltime = Random.Range(6.0f, 12.0f);

        patrol.WaitDelay = 1.0f;
        patrol.WaitDelayRandom = 1.0f;
        //목적지 정하는 로직 설정
        patrol.OnSetDestination += OnSetDestination;
        patrol.StartMove();     
        
        routines[0] = sender.StartCoroutine(ChangeRadius());
        routines[1] = sender.StartCoroutine(SetApporachMode());
    }

    public override void Execute(AIController sender)
    {
        target = perception.GetPercievedTarget();
        if (target == null)
            return;

        sender.transform.LookAt(target.transform);

        animator.SetFloat("SpeedX", Mathf.Abs(navMeshAgent.velocity.x));
        animator.SetFloat("SpeedY", Mathf.Abs(navMeshAgent.velocity.z));
    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);
        foreach (Coroutine routine in routines)
            sender.StopCoroutine(routine);

        navMeshAgent.updateRotation = true;
        patrol.StopMove();
        patrol.OnSetDestination -= OnSetDestination;
        animator.SetFloat("SpeedX", 0.0f);
        animator.SetFloat("SpeedY", 0.0f);
        animator.SetBool("IsGuard", false);
    }

    private Vector3 OnSetDestination()
    {
        Debug.Log(currentRadius);
        //플레이어 주변의 랜덤한 점 선택
        float angle = Random.Range(150, 30) * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle) * currentRadius;
        float z = Mathf.Sin(angle) * currentRadius;

        Vector3 position = sender.transform.position;
        if (target != null)
            position = target.transform.position;

        position.y = sender.transform.position.y;
        Vector3 direction = sender.transform.position - position;
        Vector3 goalPosition = Quaternion.LookRotation(direction.normalized) * new Vector3(x , 0.0f, z);

        targetPosition = position + goalPosition;
        return targetPosition;
    }

    /// <summary>
    /// 특정 시간 마다 타겟으로 부터 떨어진 정도를 랜덤하게 조절한다.
    /// </summary>
    /// <returns></returns>
    private IEnumerator ChangeRadius()
    {
        while (true)
        {
            yield return new WaitForSeconds(3.0f);
            currentRadius = Random.Range(2.0f, 4.0f);
        }
    }

    /// <summary>
    /// 랜덤한 대기 시간이 지나면 Approach상태로 전환
    /// </summary>
    /// <returns></returns>
    private IEnumerator SetApporachMode()
    {
        yield return new WaitForSeconds(approachModeCooltime);
        sender.SetState(AIController.State.Approach);
    }
}