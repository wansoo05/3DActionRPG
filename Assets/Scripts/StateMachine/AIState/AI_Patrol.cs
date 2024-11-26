using UnityEngine;

public class AI_Patrol : AI_Base
{
    private float radius = 5.0f;
    private float waitDelay = 2.0f;
    private float waitDelayRandoem = 0.5f;

    private Vector3 destination;
    private Vector3 initPosition;

    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);

        //무기 장착 중이면 무기 장착 해제
        if (weapon.UnarmedMode == false)
            weapon.SetUnarmedMode();

        navMeshAgent.speed = 2.0f;
        navMeshAgent.updateRotation = true;
        navMeshAgent.angularSpeed = 360.0f;
        navMeshAgent.isStopped = false;

        initPosition = destination = sender.transform.position;
        patrol.WaitDelay = waitDelay;
        patrol.WaitDelayRandom = waitDelayRandoem;

        patrol.OnSetDestination += OnSetDestination;

        patrol.StartMove();
    }

    public override void Execute(AIController sender)
    {
        GameObject target = perception.GetPercievedTarget();
        if (target != null)
            sender.SetState(AIController.State.Equip);

        animator.SetFloat("SpeedY", navMeshAgent.velocity.magnitude);
    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);

        navMeshAgent.isStopped = true;
        navMeshAgent.ResetPath();
        patrol.StopMove();
        patrol.OnSetDestination -= OnSetDestination;

        animator.SetFloat("SpeedY", 0.0f);
    }

    private Vector3 OnSetDestination()
    {
        float randomX = Random.Range(initPosition.x - radius, initPosition.x + radius);
        float randomZ = Random.Range(initPosition.z - radius, initPosition.z + radius);

        destination = new Vector3(randomX, 0.0f, randomZ);
        return destination;
    }
}