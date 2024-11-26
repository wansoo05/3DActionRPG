using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Rendering;

public class AIController_Fellow : AIController
{
    [SerializeField]
    private float currentCoolTime = 0.0f;

    public GameObject Owner;
    protected override void OnEnable()
    {
        base.OnEnable();

        Owner = GameManager.Instance.PlayerInstance.gameObject;

        GameObject obj = Factory.Instance.GetEffect("ExplosionSpawnData");
        obj.transform.position = transform.position;
        Factory.Instance.ReturnEffectDelay(obj, 1.0f);
        string name = "SmallExplosion";
        SoundManager.Instance.PlaySound(name, SoundType.Effect, transform);
    }

    protected override bool CheckMode()
    {
        bool bCheck = false;
        bCheck |= (GetState() == State.Wait);
        bCheck |= (GetState() == State.Approach);
        bCheck |= (GetState() == State.Follow);

        return bCheck;
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

    protected override void Update_State()
    {
        if (CheckCoolTime())
            return;

        if (!CheckMode())
            return;

        GameObject enemy = perception.GetPercievedTarget();

        if (enemy == null)
        {
            if (weapon.UnarmedMode == false)
                weapon.SetUnarmedMode();

            if (Owner == null)
                return;

            if (Vector3.Distance(Owner.transform.position, transform.position) < 2.0f)
            {
                SetState(State.Wait);
                return;
            }

            SetState(State.Follow);
            return;
        }

        if (weapon.UnarmedMode)
        {
            SetState(State.Equip);
            return;
        }

        if (Vector3.Distance(enemy.transform.position, transform.position) < attackRange)
        {
            SetState(State.Action);
            return;
        }

        SetState(State.Approach);
    }

    protected override void OnEndDoAction()
    {
        base.OnEndDoAction();
        currentCoolTime += 3.0f;
    }
}