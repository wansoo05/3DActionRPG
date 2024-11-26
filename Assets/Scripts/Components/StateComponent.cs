using System;
using UnityEngine;

public class StateComponent : MonoBehaviour
{
    public enum StateType
    {
        Idle = 0, Equip, Action, Evade, Jump, Falling, Land, Damaged, Air, Down, GetUp, Climb, Dead,
    }

    private StateType type = StateType.Idle;
    public StateType Type { get => type; }

    public event Action<StateType, StateType> OnStateTypeChanged;

    public bool IdleMode { get => type == StateType.Idle; }
    public bool EquipMode { get => type == StateType.Equip; }
    public bool ActionMode { get => type == StateType.Action; }
    public bool EvadeMode { get => type == StateType.Evade; }
    public bool JumpMode { get => type == StateType.Jump; }
    public bool FallingMode { get => type == StateType.Falling; }
    public bool LandMode { get => type == StateType.Land; }
    public bool DamagedMode { get => type == StateType.Damaged; }
    public bool AirMode { get => type == StateType.Air; }
    public bool DownMode { get => type == StateType.Down; }
    public bool GetUpMode { get => type == StateType.GetUp; }
    public bool DeadMode { get => type == StateType.Dead; }
    public bool ClimbMode { get => type == StateType.Climb; }

    public void SetIdleMode() => ChangeType(StateType.Idle);
    public void SetEquipMode() => ChangeType(StateType.Equip);
    public void SetActionMode() => ChangeType(StateType.Action);
    public void SetEvadeMode() => ChangeType(StateType.Evade);
    public void SetJumpMode() => ChangeType(StateType.Jump);
    public void SetFallingMode() => ChangeType(StateType.Falling);
    public void SetLandMode() => ChangeType(StateType.Land);
    public void SetDamagedMode() => ChangeType(StateType.Damaged);
    public void SetAirMode() => ChangeType(StateType.Air);
    public void SetDownMode() => ChangeType(StateType.Down);
    public void SetGetUpMode() => ChangeType(StateType.GetUp);
    public void SetDeadMode() => ChangeType(StateType.Dead);
    public void SetClimbMode() => ChangeType(StateType.Climb);

    public void ChangeType(StateType type)
    {
        if (this.type == type)
            return;

        StateType prevType = this.type;
        this.type = type;

        OnStateTypeChanged?.Invoke(prevType, type);
    }

    public bool EntireJumpMode()
    {
        bool bCheck = false;
        bCheck |= JumpMode;
        bCheck |= FallingMode;
        bCheck |= LandMode;
        return bCheck;
    }
}