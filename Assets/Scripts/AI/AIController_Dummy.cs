using UnityEngine;

public class AIController_Dummy : AIController
{
    protected override bool CheckMode()
    {
        bool bCheck = false;
        bCheck |= (GetState() == State.Wait);
        bCheck |= (GetState() == State.Approach);

        return bCheck;
    }

    protected override void Update_State()
    {
        if (!CheckMode())
            return;

        SetState(State.Wait);
    }
}