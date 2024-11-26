using UnityEngine;
using UnityEngine.Rendering;

public class Mage : Weapon
{
    [SerializeField]
    private string wandTransformName = "Hand_Wand";

    [SerializeField]
    private string muzzleTransformName = "Mage_Muzzle";

    private Transform wandTransform;

    [SerializeField]
    private string[] projectileDataName;

    private Transform muzzleTransform;


    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Wand;
    }

    protected override void Awake()
    {
        base.Awake();

        wandTransform = rootObject.transform.FindChildByName(wandTransformName);
        Debug.Assert(wandTransform != null);
        transform.SetParent(wandTransform, false);

        muzzleTransform = rootObject.transform.FindChildByName(muzzleTransformName);
        Debug.Assert(muzzleTransform != null);

        gameObject.SetActive(false);
    }

    public override void Begin_Equip()
    {
        base.Begin_Equip();

        gameObject.SetActive(true);
    }

    public override void UnEquip()
    {
        base.UnEquip();

        gameObject.SetActive(false);
    }
    public override void Shoot_Projectile(AnimationEvent e)
    {
        base.Shoot_Projectile(e);

        if (projectileDataName[e.intParameter] == "")
            return;

        hitIndex++;
        Vector3 muzzlePosition = muzzleTransform.position;
        muzzlePosition += rootObject.transform.forward * 0.75f;

        GameObject ob = Instantiate(doActionDatas[actionIndex].hittingDatas[hitIndex].Particle, muzzlePosition, Quaternion.identity);
        ob.transform.localScale = doActionDatas[actionIndex].hittingDatas[hitIndex].ParticleScaleOffset;

        GameObject obj = Factory.Instance.GetProjectile(projectileDataName[e.intParameter]);
        obj.transform.position = muzzlePosition;
        obj.transform.rotation = rootObject.transform.rotation;
        Projectile projectile = obj.GetComponent<Projectile>();
        {
            projectile.Shoot(1.0f, rootObject.transform.forward);
            projectile.OnProjectileHit += OnProjectileHit;
        }

        string name = "Default_Muzzle_";
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

            damage?.OnDamage(rootObject, this, hitPoint, doActionData.hittingDatas[0]);
        }


        Factory.Instance.ReturnProjectileDelay(projectile);
    }
}