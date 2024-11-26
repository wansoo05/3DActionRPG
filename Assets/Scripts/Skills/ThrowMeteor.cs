using UnityEngine;

public class ThrowMeteor : Skill
{
    [SerializeField]
    private string muzzleTransformName = "Mage_Muzzle";

    [SerializeField]
    private GameObject[] projectilePrefab;

    [SerializeField]
    private GameObject cursorPrefab;
    private GameObject cursorObject;

    private SkillComponont skill;
    private Transform muzzleTransform;
    private PlayerCameraController cameraController;


    protected override void Reset()
    {
        base.Reset();

        type = SkillType.ThrowMeteor;
    }

    protected override void Awake()
    {
        base.Awake();

        cameraController = rootObject.GetComponent<PlayerCameraController>();
        skill = rootObject.GetComponent<SkillComponont>();

        muzzleTransform = rootObject.transform.FindChildByName(muzzleTransformName);
        Debug.Assert(muzzleTransform != null);
    }

    protected override void Start()
    {
        base.Start();

        if (cursorPrefab == null)
            return;

        cursorObject = Instantiate(cursorPrefab);
        cursorObject.SetActive(false);
        skill.OnSkillTypeChanged += OnSkillTypeChanged;

    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        string name = "WindMagicMisc_HighGust";
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

    public override void Shoot_Projectile(AnimationEvent e)
    {
        base.Shoot_Projectile(e);

        if (projectilePrefab[e.intParameter] == null)
            return;

        hitIndex++;
        Vector3 muzzlePosition = muzzleTransform.position;
        muzzlePosition += rootObject.transform.forward * 0.75f;
        GameObject ob = Instantiate(doActionData.hittingDatas[hitIndex].Particle, muzzlePosition, Quaternion.identity);
        ob.transform.localScale = doActionData.hittingDatas[hitIndex].ParticleScaleOffset;

        GameObject obj = Instantiate<GameObject>(projectilePrefab[e.intParameter], muzzlePosition, rootObject.transform.rotation);
        Projectile projectile = obj.GetComponent<Projectile>();
        {
            Vector3 startPosition = muzzleTransform.position;
            Vector3 endPosition = rootObject.transform.position + transform.forward * 5.0f;
            Vector3 direction = endPosition - startPosition;
            projectile.Shoot(1.0f, direction.normalized);
            projectile.OnProjectileHit += OnProjectileHit;
        }

        string name = "Meteor_Muzzle_";
        name += SoundManager.Instance.SoundRandomRange(1, 4);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

    private void OnProjectileHit(Collider self, Collider other, Vector3 point, DoActionData doActionData, GameObject projectile)
    {
        if (other.gameObject.tag == rootObject.gameObject.tag)
            return;

        IDamagable damage = other.GetComponent<IDamagable>();

        if (damage != null)
        {
            Vector3 hitPoint = self.ClosestPoint(other.transform.position);
            hitPoint = other.transform.InverseTransformPoint(hitPoint);

            damage?.OnDamage(rootObject, doActionData.weapon , hitPoint, doActionData.hittingDatas[0]);
        }
        if (doActionData.hittingDatas[0].HitParticleDataName != "")
        {
            GameObject obj = Factory.Instance.GetEffect(doActionData.hittingDatas[0].HitParticleDataName);
            obj.transform.position = point;
            Factory.Instance.ReturnEffectDelay(obj, 0.5f);
        }

        Destroy(projectile);
    }

    public override void End_DoAction()
    {
        base.End_DoAction();

    }

    private void OnSkillTypeChanged(SkillType prevType, SkillType newType)
    {
        switch (newType)
        {
            case SkillType.ThrowMeteor:
            {
                cursorObject.SetActive(true);
                break;
            }
            default:
            {
                cursorObject.SetActive(false);
                break;
            }
        }
    }
}