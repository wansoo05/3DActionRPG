using System;
using System.Collections.Generic;
using UnityEngine;

public enum WeaponType
{
    Unarmed = 0, Fist, Warrior, Katana, Dual, Wand, Boss, Boss_Super, Max,
}
/// <summary>
/// �ش� ���⿡�� ������ �����ִ� ������Ʈ
/// </summary>
public class WeaponComponent : MonoBehaviour, IStateUpdatable
{
    /// <summary>
    /// ���� Prefab���� ���� �迭
    /// </summary>
    [SerializeField]
    private GameObject[] originPrefabs;

    private Animator animator;

    private StateComponent state;
    private SkillComponont skill;

    private WeaponType type = WeaponType.Unarmed;
    public WeaponType Type { get => type; }

    /// <summary>
    /// ���Ⱑ �ٲ��� �� �˷��ֱ� ���� �̺�Ʈ(������ ����)
    /// </summary>
    public event Action<WeaponType, WeaponType> OnWeaponTypeChanged;
    /// <summary>
    /// ������ ������ �� �˷��ֱ� ���� �̺�Ʈ
    /// </summary>
    public event Action OnEndEquip;
    /// <summary>
    /// ������ ������ �� �˷��ֱ� ���� �̺�Ʈ
    /// </summary>
    public event Action OnEndDoAction;


    public bool UnarmedMode { get => type == WeaponType.Unarmed; }
    public bool FistMode { get => type == WeaponType.Fist; }
    public bool WarriorMode { get => type == WeaponType.Warrior; }
    public bool KatanaMode { get => type == WeaponType.Katana; }
    public bool DualMode { get => type == WeaponType.Dual; }
    public bool WandMode { get => type == WeaponType.Wand; }
    public bool BossMode { get => type == WeaponType.Boss; }
    public bool Boss_SuperMode { get => type == WeaponType.Boss_Super; }

    private bool bCanHit;

    /// <summary>
    /// ���⸦ ���������� �˱����� �Լ�
    /// </summary>
    /// <returns></returns>
    public bool IsEquippingMode()
    {
        if (UnarmedMode)
            return false;

        Weapon weapon = weaponTable[type];
        if (weapon == null)
            return false;

        return weapon.Equipping;
    }

    /// <summary>
    /// ���� ���Ⱑ �����ϰ� �ִ� �������� �˱� ���� �Լ�
    /// </summary>
    /// <returns></returns>
    public bool IsAttacking()
    {
        if (UnarmedMode)
            return false;

        if (weaponTable[type] == null)
            return false;

        return bCanHit;
    }

    private Dictionary<WeaponType, Weapon> weaponTable;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<StateComponent>();
        skill = GetComponent<SkillComponont>();
        if (skill != null)
        {
            skill.OnPlaySlash += Play_Slash;
            skill.OnEndSlash += End_Slash;
        }

        weaponTable = new Dictionary<WeaponType, Weapon>();

        for (int i = 0; i < (int)WeaponType.Max; i++)
        {
            weaponTable.Add((WeaponType)i, null);
        }

        for (int i = 0; i < originPrefabs.Length; i++)
        {
            GameObject obj = Instantiate<GameObject>(originPrefabs[i], transform);
            Weapon weapon = obj.GetComponent<Weapon>();

            obj.name = weapon.Type.ToString();

            weaponTable[weapon.Type] = weapon;
        }
    }

    public void SetFistMode()
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.Fist);
    }

    public void SetWarriorMode()
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.Warrior);
    }

    public void SetKatanaMode()
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.Katana);

    }

    public void SetDualMode()
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.Dual);
        
    }

    public void SetWandMode()
    {
        if (state.IdleMode == false)
            return;

        SetMode(WeaponType.Wand);
    }

    public void SetBossMode()
    {
        if (state.IdleMode == false)
            return;

        print("SetBossMode");
        SetMode(WeaponType.Boss);
    }

    public void SetBoss_SuperMode()
    {
        if (state.IdleMode == false)
            return;

        print("SetBoss_SuperMode");
        SetMode(WeaponType.Boss_Super);
    }

    public void SetUnarmedMode()
    {
        if (state.IdleMode == false)
            return;

        animator.SetInteger("WeaponType", (int)WeaponType.Unarmed);

        if (weaponTable[type] != null)
            weaponTable[type].UnEquip();

        ChangeType(WeaponType.Unarmed);
    }

    /// <summary>
    /// ���� ��ü�� ���� �Լ�
    /// </summary>
    /// <param name="type"></param>
    private void SetMode(WeaponType type)
    {
        //��ü�Ϸ��� ���� ������ ���� ���� ���Ⱑ ���ٸ� ���� ������ �����Ѵ�.
        if (this.type == type)
        {
            SetUnarmedMode();
            return;
        }
        //���� ������ �� �� ���� ���Ⱑ ������ ���¶�� ���� ����� ���� ����.
        else if (UnarmedMode == false)
        {
            weaponTable[this.type].UnEquip();
        }

        //nullüũ
        if (weaponTable[type] == null)
        {
            SetUnarmedMode();
            return;
        }

        animator.SetBool("IsEquipping", true);
        animator.SetInteger("WeaponType", (int)type);
        ChangeType(type);
        weaponTable[type].Equip();
    }

    /// <summary>
    /// ���� Ÿ���� �����ϰ� ���� ������ ������ �̺�Ʈ�� �˷��ش�.
    /// </summary>
    /// <param name="type"></param>
    private void ChangeType(WeaponType type)
    {
        if (this.type == type)
            return;

        WeaponType prevType = this.type;
        this.type = type;
        OnWeaponTypeChanged?.Invoke(prevType, type);
    }

    public void Begin_Equip()
    {
        weaponTable[type].Begin_Equip();
    }

    public void End_Equip()
    {
        animator.SetBool("IsEquipping", false);

        weaponTable[type].End_Equip();
        OnEndEquip?.Invoke();
    }

    /// <summary>
    /// Ư���� ��Ȳ���� �����ϰ� ������ �������ִ� �Լ�
    /// </summary>
    public void Secure_EndEquip()
    {
        animator.SetBool("IsEquipping", false);

        weaponTable[type].End_Equip();
    }

    /// <summary>
    /// �ش� ���⿡�� ������ ���ɳ����ִ� �Լ�
    /// </summary>
    /// <param name="actionIndex">AI�� ���� ���� ����</param>
    public void DoAction(int actionIndex = 0)
    {
        if (weaponTable[type] == null)
            return;

        if (state.IdleMode == false && state.ActionMode == false)
            return;

        animator.SetTrigger("Attack");
        animator.SetInteger("AttackType", actionIndex);
        weaponTable[type].DoAction(actionIndex);
    }

    public void Begin_DoAction(AnimationEvent e)
    {
        if (e.stringParameter != "Attack")
            return;

        weaponTable[type].Begin_DoAction();
    }


    public void End_DoAction(AnimationEvent e)
    {
        if (e.stringParameter != "Attack")
            return;

        if (weaponTable[type] == null)
            return;

        weaponTable[type].End_DoAction();
        OnEndDoAction?.Invoke();
    }

    /// <summary>
    /// Ư���� ��Ȳ���� �����ϰ� ������ �������ִ� �Լ�
    /// </summary>
    public void Secure_EndDoAction()
    {
        if (weaponTable == null)
            return;

        if (weaponTable[type] == null)
            return;

        weaponTable[type].End_DoAction();

        if (IsAttacking())
        {
            bCanHit = false;
            Melee melee = weaponTable[type] as Melee;
            melee?.End_Collision();
            End_Slash();
        }
    }

    private void Begin_Collision(AnimationEvent e)
    {
        if (e.stringParameter != "Attack")
            return;

        bCanHit = true;
        Melee melee = weaponTable[type] as Melee;
        melee?.Begin_Collision(e);
        Play_Slash(e.intParameter);
        Play_SlashSound();
    }

    public void End_Collision(AnimationEvent e)
    {
        if (e.stringParameter != "Attack")
            return;

        bCanHit = false;
        Melee melee = weaponTable[type] as Melee;
        melee?.End_Collision();
        End_Slash();
    }

    private void Shoot_Projectile(AnimationEvent e)
    {
        if (e.stringParameter != "Attack")
            return;

        weaponTable[type].Shoot_Projectile(e);
    }

    public void DoEvade()
    {
        if (weaponTable[type] == null)
            return;

        state.SetEvadeMode();
        weaponTable[type].DoEvade();
    }

    private void Play_Slash(int i)
    {
        Melee melee = weaponTable[type] as Melee;
        melee?.Play_Slash(i);
    }

    public void End_Slash()
    {
        Melee melee = weaponTable[type] as Melee;
        melee?.End_Slash();
    }

    private void Play_SlashSound()
    {
        Melee melee = weaponTable[type] as Melee;
        melee?.Play_SlashSound();
    }

    private void Play_Particle(AnimationEvent e)
    {
        if (e.stringParameter != "Attack")
            return;
        weaponTable[type].Play_Particle();
    }

    private void PickUp()
    {
        Melee melee = weaponTable[type] as Melee;
        melee?.PickUp();
    }

    public Weapon GetWeapon(WeaponType type)
    {
        return weaponTable[type];
    }

    public void Save()
    {
        GameManager.Instance.PlayerState.Type = type;
    }

    public void Load(PlayerStateData data)
    {
        switch(data.Type)
        {
            case WeaponType.Unarmed:
            {
                SetUnarmedMode();
                break;
            }
            case WeaponType.Fist:
            {
                SetFistMode();
                break;
            }
            case WeaponType.Warrior:
            {
                SetWarriorMode();
                break;
            }
            case WeaponType.Katana:
            {
                SetKatanaMode();
                break;
            }
            case WeaponType.Dual:
            {
                SetDualMode();
                break;
            }
            case WeaponType.Wand:
            {
                SetWandMode();
                break;
            }
        }
    }
}