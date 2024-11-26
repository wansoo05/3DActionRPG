using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem.XR;

public class AI_Equip : AI_Base
{

    public override void Enter(AIController sender, IState<AIController> prevState)
    {
        base.Enter(sender, prevState);

        this.sender = sender;
        navMeshAgent.isStopped = true;

        switch (sender.MyWeaponType)
        {
            case WeaponType.Warrior:
                weapon.SetWarriorMode();
                break;
            case WeaponType.Katana:
                weapon.SetKatanaMode();
                break;
            case WeaponType.Wand:
                weapon.SetWandMode();
                break;
            case WeaponType.Boss:
                weapon.SetBossMode();
                break;
            case WeaponType.Boss_Super:
                weapon.SetBoss_SuperMode();
                break;
            default:
                Debug.Assert(false);
                break;
        }
    }
    public override void Execute(AIController sender)
    {
    }

    public override void Exit(AIController sender, IState<AIController> nextState)
    {
        base.Exit(sender, nextState);

        if (CheckMode(nextState))
        {
            animator.Play("Unarmed_None", 1);

            if (weapon.IsEquippingMode() == false)
                weapon.Begin_Equip();

            weapon.Secure_EndEquip();
        }
    }

    private bool CheckMode(IState<AIController> state)
    {
        bool bCheck = false;
        bCheck |= (state as AI_Damaged != null);
        bCheck |= (state as AI_Air != null);
        bCheck |= (state as AI_Down != null);
        bCheck |= (state as AI_Dead != null);
        bCheck |= (state as AI_Retreat != null);

        return bCheck;
    }
}