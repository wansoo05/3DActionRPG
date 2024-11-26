using UnityEngine;

public class AIController_Charge : AIController
{
    private float currentCoolTime = 0.0f;

    protected override bool CheckMode()
    {
        bool bCheck = false;
        bCheck |= (GetState() == State.Wait);
        bCheck |= (GetState() == State.Patrol);
        bCheck |= (GetState() == State.Approach);

        return bCheck;
    }

    protected override void Update_State()
    {
        if (CheckCoolTime())
            return;

        if (!CheckMode())
            return;

        GameObject player = perception.GetPercievedTarget();

        if (player == null)
        {
            if (weapon.UnarmedMode == false)
                weapon.SetUnarmedMode();

            if (patrolComponent != null)
            {
                SetState(State.Patrol);
                return;
            }
            SetState(State.Wait);
            return;
        }

        if (weapon.UnarmedMode)
        {
            SetState(State.Equip);
            return;
        }
        if (Vector3.Distance(player.transform.position, transform.position) < attackRange)
        {
            SetState(State.Action);
            return;
        }
        SetState(State.Approach);
    }

    private bool CheckCoolTime()
    {
        if (currentCoolTime <= 0.0f)
        {
            currentCoolTime = 0.0f;
            return false;
        }

        currentCoolTime -= Time.deltaTime;
        return true;
    }

    protected override void OnEndDoAction()
    {
        base.OnEndDoAction();
        currentCoolTime += attackDelay + Random.Range(-attackDelayRandom, +attackDelayRandom);
    }
}