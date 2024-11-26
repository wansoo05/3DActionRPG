using Cinemachine;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using StateType = StateComponent.StateType;

/// <summary>
/// ���� ������ ��Ÿ���� ���� ������
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
    /// �÷��̾��� ���� ���� ����
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
            //���� ���� ���°� Attack�̸� ���⿡ ���� ����� ������ Skill�̸� ��ų�� ���� ����� ������.
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

        //�׽�Ʈ�� ��� ��ȭ �� ���� ��ȭ Ŀ�ǵ�
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
    /// PlayableDirector�� ����� �̺�Ʈ �Լ�
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
    /// PlayableDirector�� ����� �̺�Ʈ �Լ�
    /// </summary>
    /// <param name="pd"></param>
    private void OnStopped(PlayableDirector pd)
    {
        input.Player.Enable();
    }

    /// <summary>
    /// �����κ��� �ǰ��� �޾��� �� �ҷ����� �Լ�
    /// </summary>
    /// <param name="attacker">������</param>
    /// <param name="causer">������ ����</param>
    /// <param name="hitPoint">���ݴ��� ��ġ</param>
    /// <param name="data">�ǰ� ������</param>
    public void OnDamage(GameObject attacker, Weapon causer, Vector3 hitPoint, HittingData data)
    {
        //���ϴ� ���̶��
        if (state.EvadeMode)
        {
            //�����ڰ� ������� ���ο� ��ǰ� �Բ� �ǰ� ���� �ʰ� �Ѵ�.
            if (attacker.GetComponent<AIContorller_Boss>() != null)
            {
                StartCoroutine(Slow(0.7f));
                return;
            }
            //������ �ƴ϶�� �׳� �ǰ� ���� �ʰ� �Ѵ�.
            else
            {
                return;
            }
        }

        //HP�� ��´�.
        healthPoint.Damage(data.Power - armor > 0 ? data.Power - armor : 0 );

        //Hit Stop�� �ش�.
        MovableStopper.Instance.Start_Delay(data.StopFrame);

        //�ǰݽ� ī�޶��� ������ ǥ���Ѵ�.
        Play_Impulse(data);

        //Ÿ���� ���
        if (data.HitSoundName != "")
        {
            string name = data.HitSoundName;
            name += SoundManager.Instance.SoundRandomRange(data.HitSoundRangeFrom, data.HitSoundRangeTo);
            SoundManager.Instance.PlaySound(name, SoundType.Effect, this.transform);
        }

        //�ǰ� ����Ʈ ����
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


        //�ǰ� �Ǿ��� �� ���� �ʾҴٸ�
        if (healthPoint.Dead == false)
        {
            //�ǰ� ����� ����Ұ����� �˻�
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
                    //�и����� �ܺ��� �߰�
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
    /// �����̸� �ɰ� ������ ������ ���� �ڷ�ƾ �Լ�
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
    /// ������ �� ������ȯ�� ���� �����ϰ� ���� ���¸� ������ ���� �Լ�
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
    /// �÷��̾� ���¿� ���� �ǰ� �ִϸ��̼� ��� ����
    /// </summary>
    /// <param name="curState">���� �÷��̾� ����</param>
    /// <param name="damageType">������ ����</param>
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
            //�ٿ� ������ �� �������̰� �Ѵ�.
            case StateType.Down:
            {
                moving.Stop();

                break;
            }
        }
    }

    Coroutine waitRoutine = null;

    /// <summary>
    /// �ǰ� ����� ������ �� �ҷ����� �̺�Ʈ �Լ�
    /// </summary>
    protected override void End_Damaged()
    {
        base.End_Damaged();

        switch (state.Type)
        {
            //�ٿ���¿��� ���� ��� �ð��� �ش�.
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
    /// ���ð��� �ֱ� ���� �ڷ�ƾ�Լ�
    /// </summary>
    /// <param name="time">����� ��</param>
    /// <returns></returns>
    private IEnumerator Wait(float time)
    {
        yield return new WaitForSeconds(time);

        state.SetGetUpMode();
        animator.SetTrigger("GetUp");
        waitRoutine = null;
    }

    /// <summary>
    /// ���ο츦 �ɾ��� �ڷ�ƾ �Լ�
    /// </summary>
    /// <param name="time">���ο��� �ð�(��)</param>
    /// <returns></returns>
    private IEnumerator Slow(float time)
    {
        Time.timeScale = 0.3f;
        yield return new WaitForSeconds(time);
        Time.timeScale = 1.0f;
    }

    /// <summary>
    /// �� �Ҹ� ����� ���� �ִϸ��̼� �̺�Ʈ �Լ�
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