using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using StateType = StateComponent.StateType;

/// <summary>
/// 공격 종류를 나타내기 위한 열거형
/// </summary>
public enum ActionType
{
    Attack = 0, FirstSkill, SecondSkill,
}

[RequireComponent(typeof(PlayerMovingComponent))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerMovingComponent))]
public class Player : Character, IDamagable, IAirable
{
    /// <summary>
    /// 플레이어의 현재 공격 상태
    /// </summary>
    private ActionType curActionType = ActionType.Attack;
    public ActionType CurActionType
    { 
        get
        { 
            return curActionType;
        }
        private set
        {
            if (curActionType == value)
                return;
            if (curActionType == ActionType.SecondSkill)
            {
                value = 0;
            }
            skill.SetMode((int)value);
            curActionType = value;
        }
    }

    private PlayableDirector pd;
    private PlayerMovingComponent moving;
    private SkillComponont skill;
    private CharacterController controller;
    private CharacterBlockComponent blocking;
    private CinemachineImpulseSource impulse;
    private PlayerCameraController cameraController;
    private InputActions input;

    protected override void Awake()
    {
        base.Awake();

        moving = GetComponent<PlayerMovingComponent>();
        skill = GetComponent<SkillComponont>();
        controller = GetComponent<CharacterController>();
        blocking = GetComponent<CharacterBlockComponent>();
        cameraController = GetComponent<PlayerCameraController>();
        impulse = GetComponent<CinemachineImpulseSource>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        CurActionType = ActionType.Attack;
    }

    protected override void OnDisable()
    {
        base.OnDisable();
        input.Player.Skill.started -= Input_Skill;

        input.Player.Action.started -= Input_Action;

        input.Player.Wand.started -= Input_Equip_Wand;

        input.Player.Dual.started -= Input_Equip_Dual;

        input.Player.Katana.started -= Input_Equip_Katana;

        input.Player.Warrior.started -= Input_Equip_Warrior;

        input.Player.Fist.started -= Input_Equip_Fist;

        input.Player.Disable();
        if (pd != null)
        {
            pd.played -= OnPlayed;
            pd.stopped -= OnStopped;
        }
    }

    private void Input_Equip_Fist(InputAction.CallbackContext context) => weapon.SetFistMode();
    private void Input_Equip_Warrior(InputAction.CallbackContext context) => weapon.SetWarriorMode();
    private void Input_Equip_Katana(InputAction.CallbackContext context) => weapon.SetKatanaMode();
    private void Input_Equip_Dual(InputAction.CallbackContext context) => weapon.SetDualMode();
    private void Input_Equip_Wand(InputAction.CallbackContext context) => weapon.SetWandMode();
    private void Input_Action(InputAction.CallbackContext context)
    {
        if (state.IdleMode || state.ActionMode)
        {
            //현재 공격 상태가 Attack이면 무기에 공격 명령을 내리고 Skill이면 스킬에 공격 명령을 내린다.
            if (CurActionType == ActionType.Attack)
                weapon.DoAction();
            else
                skill.DoAction();
        }
    }

    private void Input_Skill(InputAction.CallbackContext context)
    {
        if (state.IdleMode)
        {
            CurActionType++;
        }
    }

    protected override void Start()
    {
        base.Start();

        if (GameManager.Instance.TimelinePlayerInstance != null)
        {
            pd = GameManager.Instance.TimelinePlayerInstance.GetComponent<PlayableDirector>();

            pd.played += OnPlayed;
            pd.stopped += OnStopped;
        }

        weapon.OnWeaponTypeChanged += OnWeaponTypeChanged;
        state.OnStateTypeChanged += OnStateTypeChanged;

        input = GameManager.Instance.input;

        input.Player.Enable();

        input.Player.Fist.started += Input_Equip_Fist;

        input.Player.Warrior.started += Input_Equip_Warrior;

        input.Player.Katana.started += Input_Equip_Katana;

        input.Player.Dual.started += Input_Equip_Dual;

        input.Player.Wand.started += Input_Equip_Wand;

        input.Player.Action.started += Input_Action;

        input.Player.Skill.started += Input_Skill;
    }

    protected override void Update()
    {
        base.Update();

        //테스트용 방어 강화 및 공격 강화 커맨드
        if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.K))
        {
            Armor += 10.0f;
        }
        if (Input.GetKey(KeyCode.C) && Input.GetKeyDown(KeyCode.M))
        {
            AddionalPower += 10.0f;
        }
    }

    /// <summary>
    /// PlayableDirector에 등록한 이벤트 함수
    /// </summary>
    /// <param name="pd"></param>
    private void OnPlayed(PlayableDirector pd)
    {
        PlayableAsset currentAsset = pd.playableAsset;

        if (currentAsset.name == "BossEndTimeline")
            StartCoroutine(Slow(1.0f));
        else
            input.Player.Disable();
    }

    /// <summary>
    /// PlayableDirector에 등록한 이벤트 함수
    /// </summary>
    /// <param name="pd"></param>
    private void OnStopped(PlayableDirector pd)
    {
        input.Player.Enable();
    }

    /// <summary>
    /// 적으로부터 피격을 받았을 때 불려지는 함수
    /// </summary>
    /// <param name="attacker">공격자</param>
    /// <param name="causer">공격한 무기</param>
    /// <param name="hitPoint">공격당한 위치</param>
    /// <param name="data">피격 데이터</param>
    public void OnDamage(GameObject attacker, Weapon causer, Vector3 hitPoint, HittingData data)
    {
        //피하는 중이라면
        if (state.EvadeMode)
        {
            //공격자가 보스라면 슬로우 모션과 함께 피격 받지 않게 한다.
            if (attacker.GetComponent<AIContorller_Boss>() != null)
            {
                StartCoroutine(Slow(0.7f));
                return;
            }
            //보스가 아니라면 그냥 피격 받지 않게 한다.
            else
            {
                return;
            }
        }

        //HP를 깎는다.
        healthPoint.Damage(data.Power - armor > 0 ? data.Power - armor : 0 );

        //Hit Stop을 준다.
        MovableStopper.Instance.Start_Delay(data.StopFrame);

        //피격시 카메라의 떨림을 표현한다.
        Play_Impulse(data);

        //타격음 재생
        if (data.HitSoundName != "")
        {
            string name = data.HitSoundName;
            name += SoundManager.Instance.SoundRandomRange(data.HitSoundRangeFrom, data.HitSoundRangeTo);
            SoundManager.Instance.PlaySound(name, SoundType.Effect, this.transform);
        }

        //피격 이펙트 생성
        if (data.HitParticleDataName != null)
        {
            GameObject obj = Factory.Instance.GetEffect(data.HitParticleDataName);
            if (obj != null)
            {
                obj.transform.position = transform.position;
                obj.transform.position += hitPoint + data.HitParticlePositionOffset;
                obj.transform.localScale = data.HitParticleScaleOffset;
                Factory.Instance.ReturnEffectDelay(obj, 0.5f);
            }
        }


        //피격 되었을 때 죽지 않았다면
        if (healthPoint.Dead == false)
        {
            //피격 모션을 재생할것인지 검사
            if (DamageCheck(state.Type, data.HitImpactIndex) == false)
                return;

            animator.SetTrigger("Damaged");
            animator.SetInteger("ImpactType", (int)data.HitImpactIndex);

            float force = data.Distance * 5.0f;

            Vector3 direction = transform.position - attacker.transform.position;
            switch (data.HitImpactIndex)
            {
                case HitType.Down:
                {
                    if (state.Type == StateType.Action)
                    {
                        Secure_End_DoAction();
                    }
                    //밀리도록 외부힘 추가
                    moving.ExternalForce = direction.normalized * force;
                    state.SetDownMode();

                    break;
                }

                case HitType.StunDown:
                {
                    if (state.Type == StateType.Action)
                    {
                        Secure_End_DoAction();
                    }
                    state.SetDownMode();
                    break;
                }

                default:
                {
                    moving.ExternalForce = direction.normalized * force;
                    break;
                }
            }
            return;
        }
        animator.SetBool("IsDead", true);
        controller.enabled = false;
        StartCoroutine(DelayGameOver());
    }

    /// <summary>
    /// 딜레이를 걸고 게임을 끝내기 위한 코루틴 함수
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayGameOver()
    {
        yield return new WaitForSeconds(3.0f);

        GameManager.Instance.GameOver();
    }

    private void Play_Impulse(HittingData data)
    {
        if (impulse == null)
            return;

        if (data.ImpulseSettings == null)
            return;

        if (data.ImpulseDirection.magnitude == 0)
            return;

        cameraController.CameraShaking(impulse, data.ImpulseSettings, data.ImpulseDirection);
    }

    /// <summary>
    /// 공격할 때 상태전환을 위해 안전하게 공격 상태를 끝내기 위한 함수
    /// </summary>
    private void Secure_End_DoAction()
    {
        if (weapon.UnarmedMode)
            return;

        animator.Play($"{weapon.Type}.Blend Tree", 0);

        if (CurActionType == 0)
        {
            weapon.Secure_EndDoAction();
        }
        else
        {
            skill.Secure_EndDoAction();
        }
    }

    public void OnAir(float force)
    {
        if (animator.GetBool("IsAirCombo") == true)
            return;

        moving.drag = 30.0f;
        moving.VerticalVelocity = 0.4f * force * moving.drag;
        animator.SetBool("IsAirCombo", true);
    }


    private void OnWeaponTypeChanged(WeaponType prevType,  WeaponType newType)
    {
        CurActionType = ActionType.Attack;
    }

    /// <summary>
    /// 플레이어 상태에 따른 피격 애니메이션 재생 여부
    /// </summary>
    /// <param name="curState">현재 플레이어 상태</param>
    /// <param name="damageType">데미지 상태</param>
    /// <returns></returns>
    private bool DamageCheck(StateType curState, HitType damageType)
    {
        int curType = 0;

        switch (curState)
        {
            case StateType.Damaged:
                curType = 0;
                break;
            case StateType.Down:
                curType = 1;
                break;
            case StateType.Air:
                curType = 2;
                break;
            case StateType.GetUp:
                curType = 3;
                break;
            case StateType.Action:
            {
                curType = 1;
                if (curActionType > 0)
                    curType = 4;
                break;
            }
            case StateType.Evade:
                curType = 4;
                break;
            case StateType.Jump:
            case StateType.Falling:
            case StateType.Land:
                curType = 4;
                break;
            default:
                curType = 5;
                break;
        }

        if ((int)damageType > 3)
            damageType = HitType.Down;

        return DamageHelpers.damageTable[curType, (int)damageType];
    }

    private void OnStateTypeChanged(StateType prevType, StateType newType)
    {
        switch (newType)
        {
            //다운 당했을 시 못움직이게 한다.
            case StateType.Down:
            {
                moving.Stop();

                break;
            }
        }
    }

    Coroutine waitRoutine = null;

    /// <summary>
    /// 피격 모션이 끝났을 때 불려지는 이벤트 함수
    /// </summary>
    protected override void End_Damaged()
    {
        base.End_Damaged();

        switch (state.Type)
        {
            //다운상태였을 때는 대기 시간을 준다.
            case StateType.Down:
            {
                if (waitRoutine != null)
                    StopCoroutine(waitRoutine);

                waitRoutine = StartCoroutine(Wait(0.7f));
                break;
            }
            case StateType.GetUp:
            {
                moving.Move();
                state.SetIdleMode();
                break;
            }
        }
    }

    /// <summary>
    /// 대기시간을 주기 위한 코루틴함수
    /// </summary>
    /// <param name="time">대기할 초</param>
    /// <returns></returns>
    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);

        state.SetGetUpMode();
        animator.SetTrigger("GetUp");
        waitRoutine = null;
    }

    /// <summary>
    /// 슬로우를 걸어줄 코루틴 함수
    /// </summary>
    /// <param name="time">슬로우할 시간(초)</param>
    /// <returns></returns>
    private IEnumerator Slow(float time)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSeconds(time);
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// 발 소리 재생을 위한 애니메이션 이벤트 함수
    /// </summary>
    /// <param name="animationEvent"></param>
    protected override void OnFootStep(AnimationEvent animationEvent)
    {
        base.OnFootStep(animationEvent);

        string name = "Footsteps_";
        string groundName = GroundHelpers.GetGroundName(transform);
        name += groundName;
        name += "_Walk_";
        name += SoundManager.Instance.SoundRandomRange(1, 5);

        if (groundName == null)
            return;

        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            SoundManager.Instance.PlaySound(name, SoundType.Effect, transform);
        }
    }

    protected override void OnAnimatorMove()
    {
        if (controller.enabled == false)
            return;

        if (state.ActionMode || state.DownMode)
        {
            Vector3 deltaPosition = animator.deltaPosition;
            deltaPosition.y = 0.0f;
            controller.Move(deltaPosition);
            blocking.Update_Blocking();
        }
    }
}