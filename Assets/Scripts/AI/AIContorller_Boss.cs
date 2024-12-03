using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class AIContorller_Boss : AIController
{
    [SerializeField]
    private float meleeAttackRange = 3.5f;

    [SerializeField]
    private TimelineAsset[] assets;

    /// <summary>
    /// 원거리 공격 개수
    /// </summary>
    private int rangeAction = 3;
    public int RangeAction { get => rangeAction; }

    private float currentCoolTime = 0.0f;
    private PlayableDirector pd;

    private EnemySpawner enemySpawner;
    private CinemachineImpulseSource impulse;
    private HealthPointComponent health;
    private GameObject[] subordinates = new GameObject[3];
    /// <summary>
    /// 움직임 제어 변수
    /// </summary>
    private bool bCanMove = false;

    /// <summary>
    /// 각성했을 때 발생하는 이벤트
    /// </summary>
    public event Action OnAroused;

    protected override void Awake()
    {
        base.Awake();
        impulse = GetComponent<CinemachineImpulseSource>();
        health = GetComponent<HealthPointComponent>();

        enemySpawner = FindObjectOfType<EnemySpawner>();
    }
    protected override void Start()
    {
        base.Start();

        pd = GameManager.Instance.TimelinePlayerInstance.GetComponent<PlayableDirector>();
        pd.played += OnPlayed;
        pd.stopped += OnStopped;
    }

    protected override void OnDisable()
    {
        base.OnDisable();

        pd.played -= OnPlayed;
        pd.stopped -= OnPlayed;
    }

    protected override bool CheckMode()
    {
        bool bCheck = false;
        bCheck |= (GetState() == State.Wait);
        bCheck |= (GetState() == State.Patrol);
        bCheck |= (GetState() == State.Approach);

        return bCheck;
    }

    protected override void Update_State()
    {
        if (bCanMove == false)
            return;

        if (CheckCoolTime())
            return;

        if (!CheckMode())
            return;

        //체력이 절반이하이고 무기가 각성 무기가 아니라면 각성모드로 변한다.
        if (health.Half && myWeaponType != WeaponType.Boss_Super)
        {
            SuperSetting();
            return;
        }

        GameObject target = perception.GetPercievedTarget();

        if (target == null)
        {
            if (weapon.UnarmedMode == false)
            {
                weapon.SetUnarmedMode();
            }

            SetState(State.Wait);
            return;
        }

        if (weapon.UnarmedMode)
        {
            SetState(State.Equip);
            return;
        }

        float distance = Vector3.Distance(transform.position, target.transform.position);

        if (weapon.Boss_SuperMode)
        {
            if (distance < meleeAttackRange || distance > attackRange)
            {
                SetState(State.Action);
                return;
            }
        }
        else
        {
            if (distance < meleeAttackRange)
            {
                SetState(State.Action);

                return;
            }
        }

        SetState(State.Approach);
    }

    private bool CheckCoolTime()
    {
        if (currentCoolTime <= 0.0f)
        {
            currentCoolTime = 0.0f;
            return false;
        }

        currentCoolTime -= Time.deltaTime;

        bool bCoolTimeZero = false;
        bCoolTimeZero |= (perception.GetPercievedTarget() == null);
        bCoolTimeZero |= (health.Half && myWeaponType != WeaponType.Boss_Super);

        if (bCoolTimeZero)
        {
            currentCoolTime = 0.0f;
            return false;
        }

        return true;
    }

    public bool IsTargetInMeleeRange()
    {
        return IsTargetInRange(meleeAttackRange);
    }

    private void OnPlayed(PlayableDirector pd)
    {
    }

    private void OnStopped(PlayableDirector pd)
    {
        if (pd.playableAsset.name == "BossEnterTimeline")
        {
            bCanMove = true;
            UIController.Instance.ShowBossHpBar();
        }
    }

    protected override void OnEndDoAction()
    {
        base.OnEndDoAction();
        currentCoolTime += attackDelay + UnityEngine.Random.Range(-attackDelayRandom, +attackDelayRandom);
    }

    private void Play_Impulse()
    {
        if (impulse == null)
            return;

        impulse.GenerateImpulse();
        string name = "Boss_Footstep_";
        name += SoundManager.Instance.SoundRandomRange(1, 4);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, transform);
    }

    /// <summary>
    /// 각성 모드 세팅
    /// </summary>
    private void SuperSetting()
    {
        transform.localScale *= 1.5f;

        myWeaponType = WeaponType.Boss_Super;
        maxAction = 3;

        SetState(State.Equip);

        SpawnEnemy();

        OnAroused?.Invoke();

        GameManager.Instance.TimelinePlayerInstance.PlayTimeline(assets[0]);
    }

    private void SpawnEnemy()
    {
        enemySpawner.SetRandomPointDelegate(RandomPoint);
        StartCoroutine(SpawnHelp());
    }
    
    private IEnumerator SpawnHelp()
    {
        for (int i = 0; i < 3; i++)
        {
            subordinates[i] = enemySpawner.SpawnEnemyWithEffect("MagicCircle2SpawnData");
            yield return new WaitForSeconds(0.3f);
        }
    }

    private Vector3 RandomPoint()
    {
        Vector3 origin = transform.position;
        Vector2 randomPointXZ = UnityEngine.Random.insideUnitCircle * 3.0f;
        Vector3 randomPoint = origin + new Vector3(randomPointXZ.x, 0.0f, randomPointXZ.y);

        return randomPoint;
    }

    public override bool DamageCheck(State curState, HitType damageType)
    {
        int curType = 0;

        switch (curState)
        {
            case State.Damaged:
                curType = 0;
                break;
            case State.Down:
                curType = 1;
                break;
            case State.Air:
                curType = 2;
                break;
            case State.Approach:
                curType = 5;
                break;
            default:
                curType = 4;
                break;
        }

        if ((int)damageType > 3)
            damageType = HitType.Down;

        return DamageHelpers.damageTable[curType, (int)damageType];
    }

    /// <summary>
    /// 죽을 때 실행되는 함수
    /// </summary>
    public void Dead()
    {
        UIController.Instance.HideBossHpBar();
        GameManager.Instance.TimelinePlayerInstance.PlayTimeline(assets[1]);

        foreach (GameObject obj in subordinates)
        {
            if (obj.activeSelf == true)
            {
                IDamagable damage = obj.GetComponent<IDamagable>();
                HittingData data = ScriptableObject.CreateInstance<HittingData>();
                data.Power = 100;
                data.HitImpactIndex = 0;
                data.HitSoundName = "";
                damage.OnDamage(gameObject, null, Vector3.zero, data);
            }
        }
    }
}