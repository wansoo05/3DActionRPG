using System.Collections;
using UnityEngine;
using UnityEngine.AI;


[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
public class Enemy : Character, IDamagable, IAirable, ITargetable
{
    [SerializeField]
    private Color damageColor = Color.red;

    [SerializeField]
    private float changeColorTime = 0.15f;

    [SerializeField]
    private Material disolveMaterial;

    /// <summary>
    /// 적의 종류를 나타내기 위한 id
    /// </summary>
    public int id;

    private Color originColor;
    private Material skinMaterial;

    private AIController aiController;
    private NavMeshAgent navMeshAgent;
    private Coroutine kinematicsCoroutine;
    private Canvas uiTargetMark;

    protected override void Awake()
    {
        base.Awake();

        Transform surface = transform.FindChildByName("Surface");
        Debug.Assert(surface != null);

        skinMaterial = surface.GetComponent<SkinnedMeshRenderer>().material;
        originColor = skinMaterial.color;

        aiController = GetComponent<AIController>();
        navMeshAgent = GetComponent<NavMeshAgent>();

        AIContorller_Boss boss = aiController as AIContorller_Boss;
        if (boss != null )
            boss.OnAroused += OnAroused;
    }

    protected override void Start()
    {
        base.Start();

        uiTargetMark = UIHelpers.CreateBillBoardCanvas("TargetMark", transform, Camera.main);
        uiTargetMark.enabled = false;
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        Collider collider = GetComponent<Collider>();
        collider.enabled = true;
        navMeshAgent.enabled = true;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        print($"{transform.name} : Die");

        //머테리얼 오리지널로 되돌리기
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        Material[] materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
            materials[i].SetFloat("_Fade", 1.0f);
            materials[i] = skinMaterial;
            renderers[i].material = materials[i];
        }

        //targetMarket켜져있으면 끄기
        if (uiTargetMark.enabled)
            uiTargetMark.enabled = false;

        weapon.Secure_EndDoAction();
        weapon.SetUnarmedMode();
    }

    protected override void Update()
    {
        base.Update();

        if (uiTargetMark == null)
            return;

        if (uiTargetMark.enabled == false)
            return;

        uiTargetMark.transform.rotation = Camera.main.transform.rotation;
            
    }

    public void OnDamage(GameObject attacker, Weapon causer, Vector3 hitPoint, HittingData data)
    {
        Character ch = attacker.GetComponent<Character>();
        healthPoint.Damage(data.Power + ch.AddionalPower - armor);

        transform.forward = -attacker.transform.forward;

        StartCoroutine(Change_Color(changeColorTime));
        if (attacker.transform.GetComponent<Player>() != null )
            MovableStopper.Instance.Start_Delay(data.StopFrame);

        if (data.HitSoundName != "")
        {
            string name = data.HitSoundName;
            name += SoundManager.Instance.SoundRandomRange(data.HitSoundRangeFrom, data.HitSoundRangeTo);
            SoundManager.Instance.PlaySound(name, SoundType.Effect, this.transform);
        }

        if (data.HitParticleDataName != null)
        {
            GameObject obj = Factory.Instance.GetEffect(data.HitParticleDataName);
            obj.transform.position = transform.position;
            if (obj != null)
            {
                obj.transform.position += hitPoint + data.HitParticlePositionOffset;
                obj.transform.localScale = data.HitParticleScaleOffset;
                Factory.Instance.ReturnEffectDelay(obj, 0.5f);
            }
        }

        if (healthPoint.Dead == false)
        {
            if (aiController.DamageCheck(aiController.GetState(), data.HitImpactIndex) == false)
                return;

            if (navMeshAgent.enabled == true)
                navMeshAgent.isStopped = true;

            Begin_Damaged();
            animator.SetTrigger("Damaged");
            animator.SetInteger("ImpactType", (int)data.HitImpactIndex);

            if (kinematicsCoroutine != null)
            {
                StopCoroutine(kinematicsCoroutine);
                kinematicsCoroutine = null;
            }
            rigidbody.isKinematic = false;
            float force = data.Distance * 10.0f;

            switch (data.HitImpactIndex)
            {
                case HitType.Down:
                {
                    rigidbody.AddForce(-transform.forward * force * 10.0f, ForceMode.Force);
                    aiController?.SetState(AIController.State.Down);
                    kinematicsCoroutine = StartCoroutine(Change_IsKinematics(5));

                    break;
                }

                case HitType.Air:
                {
                    if (data.Distance <= 1.0f)
                        break;

                    OnAir(data.Distance);
                    IAirable player = attacker.GetComponent<IAirable>();
                    player.OnAir(data.Distance);
                    aiController?.SetState(AIController.State.Air);
                    break;
                }

                case HitType.Smash:
                {
                    aiController?.SetState(AIController.State.Down);
                    rigidbody.AddForce(Vector3.down * force, ForceMode.Impulse);
                    kinematicsCoroutine = StartCoroutine(Change_IsKinematics(4));
                    
                    break;
                }

                case HitType.Explosion:
                {
                    aiController?.SetState(AIController.State.Down);
                    rigidbody.AddExplosionForce(1000.0f, transform.position + transform.forward, 2.0f);
                    kinematicsCoroutine =StartCoroutine(Change_IsKinematics(5));
                    break;
                }

                case HitType.StunDown:
                {
                    aiController?.SetState(AIController.State.Down);
                    kinematicsCoroutine = StartCoroutine(Change_IsKinematics(5));
                    break;
                }

                default:
                {
                    print($"{transform.name} : ToDamaged");
                    rigidbody.AddForce(-transform.forward * force * 10.0f, ForceMode.Force);
                    aiController?.SetState(AIController.State.Damaged);
                    kinematicsCoroutine = StartCoroutine(Change_IsKinematics(5));
                    break;
                }
            }

            return;
        }

        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        AIContorller_Boss boss = aiController as AIContorller_Boss;
        if (boss != null)
            boss.Dead();
        aiController?.SetState(AIController.State.Dead);
        animator.SetBool("IsDead", true);
        StartCoroutine(FadeOutDisolve(2.0f));
    }

    /// <summary>
    /// 죽을 때 Disolve효과 부드럽게 해주는 코루틴 함수
    /// </summary>
    /// <param name="t">Disolve효과 몇초나오게 하는지 변수</param>
    /// <returns></returns>
    private IEnumerator FadeOutDisolve(float t)
    {
        //애니메이션 끝날 때 까지 대기
        yield return new WaitForSeconds(t);

        //셰이더 디졸브 셰이더로 바꾸기
        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        Material[] materials = new Material[renderers.Length];
        for (int i = 0; i < renderers.Length; i++)
        {
            materials[i] = renderers[i].material;
            materials[i] = disolveMaterial;
            renderers[i].material = materials[i];
        }    

        //서서히 fade값 바꾸기
        float value = 1.0f;
        while(value > 0.0f)
        {
            value -= Time.deltaTime;
            foreach (Material mat in materials)
            {
                mat.SetFloat("_Fade", value);
            }
            yield return null;
        }

        Factory.Instance.ReturnCharacterDelay(this.gameObject, 1.0f);
    }

    /// <summary>
    /// 적이 맞을 때 스킨 색 변하게 해주는 코루틴 함수
    /// </summary>
    /// <param name="time">몇초 동안 유지할지</param>
    /// <returns></returns>
    private IEnumerator Change_Color(float time)
    {
        skinMaterial.color = damageColor;

        yield return new WaitForSeconds(time);

        skinMaterial.color = originColor;
    }

    /// <summary>
    /// 몇 프레임 후에 IsKinematics를 되돌리기 위한 코루틴 함수
    /// </summary>
    /// <param name="frame">몇 프레임 후에 돌릴지</param>
    /// <returns></returns>
    private IEnumerator Change_IsKinematics(int frame)
    {
        for (int i = 0; i < frame; i++)
            yield return new WaitForFixedUpdate();

        rigidbody.isKinematic = true;
        kinematicsCoroutine = null;
    }

    protected override void End_Damaged()
    {
        base.End_Damaged();
    }


    public void OnAir(float force)
    {
        rigidbody.drag = 30.0f;
        rigidbody.AddForce(Vector3.up * force * rigidbody.drag, ForceMode.Impulse);
    }

    /// <summary>
    /// 보스가 각성될 때 호출되는 이벤트 함수
    /// </summary>
    private void OnAroused()
    {
        Armor = 5.0f;
        AddionalPower = 5.0f;
    }

    protected override void OnFootStep(AnimationEvent animationEvent)
    {
        base.OnFootStep(animationEvent);

        if (aiController as AIContorller_Boss == null)
            return;

        string name = "Giant";
        name += "_FootStep_";
        name += SoundManager.Instance.SoundRandomRange(1, 2);

        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            SoundManager.Instance.PlaySound(name, SoundType.Effect, transform);
        }
    }

    public void OnSelected()
    {
        if (uiTargetMark == null)
            return;

        uiTargetMark.enabled = true;
    }

    public void OnEliminated()
    {
        if (uiTargetMark == null)
            return;

        uiTargetMark.enabled = false;
    }
}