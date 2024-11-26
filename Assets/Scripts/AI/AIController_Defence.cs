using UnityEngine;

public class AIController_Defence : AIController
{
    protected override bool CheckMode()
    {
        bool bCheck = false;
        bCheck |= (GetState() == State.Wait);
        bCheck |= (GetState() == State.Patrol);
        bCheck |= (GetState() == State.Approach);
        bCheck |= (GetState() == State.Alert);

        return bCheck;
    }

    protected override void Update_State()
    {
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
        if (GetState() == State.Approach)
        {
            if (Vector3.Distance(player.transform.position, transform.position) < attackRange)
            {
                print($"{transform.name} : ToAction");
                SetState(State.Action);
            }
            return;
        }
        SetState(State.Alert);
    }
}