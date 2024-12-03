using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public enum SkillType
{
    None = 0, FistAir, FistUpper, ChargingSlash, SmashingSlash, LightningStrike, Whirl, Summon, ThrowMeteor, BlowAround, Rolling, Cross, Heal, Max,
}

/// <summary>
/// 해당 Skill에 명령을 내려주는 컴포넌트
/// </summary>
public class SkillComponont : MonoBehaviour
{
    [SerializeField]
    private GameObject[] originPrefabs;

    private Animator animator;
    private StateComponent state;
    private MagicPointComponent magicPoint;
    private WeaponComponent weapon;

    private SkillType type = SkillType.None;
    public SkillType Type { get => type; }

    public event Action<int> OnPlaySlash;
    public event Action OnEndSlash;
    //무기가 변경되었을 때 알려주는 이벤트(옵저버 패턴)
    public event Action<SkillType, SkillType> OnSkillTypeChanged;
    //무기가 등록이 되었을 때 알려주는 이벤트
    public event Action<SkillType, SkillType> OnSkillRegisted;

    public bool NoneMode { get => type == SkillType.None; }
    public bool FistAirMode { get => type == SkillType.FistAir; }
    public bool FistUpperMode { get => type == SkillType.FistUpper; }
    public bool ChargingSlashMode { get => type == SkillType.ChargingSlash; }
    public bool SmashingSlashMode { get => type == SkillType.SmashingSlash; }
    public bool ThrowMeteorMode { get => type == SkillType.ThrowMeteor; }
    public bool BlowAroundMode { get => type == SkillType.BlowAround; }
    public bool LightningStrikeMode { get => type == SkillType.LightningStrike; }
    public bool WhirlMode { get => type == SkillType.Whirl; }
    public bool SummonMode { get => type == SkillType.Summon; }
    public bool RollingMode { get => type == SkillType.Rolling; }
    public bool CrossMode { get => type == SkillType.Cross; }
    public bool HealMode { get => type == SkillType.Heal; }

    private Dictionary<SkillType, Skill> skillTable;
    private SkillType[] skillRegisted = new SkillType[2];
    private bool bCanHit;

    private void Awake()
    {
        animator = GetComponent<Animator>();
        state = GetComponent<StateComponent>();
        magicPoint = GetComponent<MagicPointComponent>();
        weapon = GetComponent<WeaponComponent>();
        weapon.OnWeaponTypeChanged += OnWeaponTypeChanged;

        skillTable = new Dictionary<SkillType, Skill>();

        for (int i = 0; i < (int)SkillType.Max; i++)
        {
            skillTable.Add((SkillType)i, null);
        }

        for (int i = 0; i < originPrefabs.Length; i++)
        {
            GameObject obj = Instantiate<GameObject>(originPrefabs[i], transform);
            Skill skill = obj.GetComponent<Skill>();

            obj.name = skill.Type.ToString();

            skillTable[skill.Type] = skill;
        }
    }

    private void Start()
    {
        RegistSkill(SkillType.Summon, SkillType.Heal);
    }

    public bool IsAttacking()
    {
        if (NoneMode)
            return false;

        if (skillTable[type] == null)
            return false;

        return bCanHit;
    }

    private void SetNoneMode()
    {
        if (state.IdleMode == false)
            return;

        ChangeType(SkillType.None);
    }

    public void SetMode(int index)
    {
        if (state.IdleMode == false)
            return;

        if (index == 0)
        {
            SetNoneMode();
            return;
        }
        animator.SetInteger("SkillType", index - 1);
        ChangeType(skillRegisted[index - 1]);
    }

    /// <summary>
    /// 무기를 등록해주는 함수
    /// </summary>
    /// <param name="type1">첫 번째 스킬 타입</param>
    /// <param name="type2">두 번째 스킬 타입</param>
    private void RegistSkill(SkillType type1, SkillType type2)
    {
        SetNoneMode();
        skillRegisted[0] = type1;
        skillRegisted[1] = type2;

        OnSkillRegisted?.Invoke(type1, type2);
    }

    private void ChangeType(SkillType type)
    {
        if (this.type == type)
            return;

        SkillType prevType = this.type;
        this.type = type;

        OnSkillTypeChanged?.Invoke(prevType, type);
    }

    public void DoAction()
    {
        if (skillTable[type] == null)
            return;

        if (type == SkillType.None)
            return;

        if (state.IdleMode == false)
            return;

        if (skillTable[type].CheckCoolTime())
            return;

        if (magicPoint.Use(skillTable[type].UseMagicPoint) == false)
            return;

        animator.SetTrigger("Skill");
        skillTable[type].DoAction();
    }

    private void Begin_DoAction(AnimationEvent e)
    {
        if (e.stringParameter != "Skill")
            return;

        skillTable[type].Begin_DoAction();
        
    }

    public void End_DoAction(AnimationEvent e)
    {
        if (e.stringParameter != "Skill")
            return;

        skillTable[type].End_DoAction();
    }

    public void Secure_EndDoAction()
    {
        if (skillTable == null)
            return;

        if (skillTable[type] == null)
            return;

        skillTable[type].End_DoAction();
        if (IsAttacking())
        {
            bCanHit = false;
            skillTable[type].End_Collision();
            OnEndSlash?.Invoke();
        }
    }

    public void Shoot_Projectile(AnimationEvent e)
    {
        if (e.stringParameter != "Skill")
            return;

        skillTable[type].Shoot_Projectile(e);
    }

    private void Play_Particle(AnimationEvent e)
    {
        if (e.stringParameter != "Skill")
            return;

        skillTable[type].Play_Particle();
    }

    private void Begin_Collision(AnimationEvent e)
    {
        if (e.stringParameter != "Skill")
            return;

        bCanHit = true;
        OnPlaySlash?.Invoke(e.intParameter);
        skillTable[type].Begin_Collision(e);
        Play_SlashSound();
    }

    private void End_Collision(AnimationEvent e)
    {
        if (e.stringParameter != "Skill")
            return;

        bCanHit = false;
        OnEndSlash?.Invoke();
        skillTable[type].End_Collision();
    }

    /// <summary>
    /// 공기 저항값 돌려주는 애니메이션 이벤트 함수
    /// </summary>
    private void ResetDrag()
    {
        PlayerMovingComponent moving = GetComponent<PlayerMovingComponent>();

        moving.drag = 0.0f;
    }

    public void Play_SlashSound()
    {
        skillTable[type].Play_SlashSound();
    }

    public Skill GetSkill(SkillType type)
    {
        return skillTable[type];
    }

    /// <summary>
    /// 무기 변경에 따른 스킬 등록
    /// </summary>
    /// <param name="type1">이전 무기</param>
    /// <param name="type2">바뀐 무기</param>
    private void OnWeaponTypeChanged(WeaponType type1, WeaponType type2)
    {
        switch (type2)
        {
            case WeaponType.Fist:
            {
                RegistSkill(SkillType.FistAir, SkillType.FistUpper);
                break;
            }
            case WeaponType.Warrior:
            {
                RegistSkill(SkillType.ChargingSlash, SkillType.SmashingSlash);
                break;
            }
            case WeaponType.Katana:
            {
                RegistSkill(SkillType.LightningStrike, SkillType.Whirl);
                break;
            }
            case WeaponType.Dual:
            {
                RegistSkill(SkillType.Rolling, SkillType.Cross);
                break;
            }
            case WeaponType.Wand:
            {
                RegistSkill(SkillType.ThrowMeteor, SkillType.BlowAround);
                break;
            }
            case WeaponType.Unarmed:
            {
                RegistSkill(SkillType.Summon, SkillType.Heal);
                break;
            }
        }
    }
}