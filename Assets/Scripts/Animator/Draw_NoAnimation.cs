using UnityEngine;

public class Draw_NoAnimation : StateMachineBehaviour
{
    private bool bFirstExecution;
    private WeaponComponent weapon;

    public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateEnter(animator, stateInfo, layerIndex);

        if (weapon == null)
            weapon = animator.gameObject.GetComponent<WeaponComponent>();

        weapon.Begin_Equip();
    }

    public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        base.OnStateExit(animator, stateInfo, layerIndex);

        weapon.End_Equip();
    }
}