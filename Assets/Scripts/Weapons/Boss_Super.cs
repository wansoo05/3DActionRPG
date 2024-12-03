using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss_Super : Melee
{
    private enum PartType
    {
        LeftArm = 0, RightArm, Max,
    }

    private enum AttackType
    {
        LeftPunch = 0, Clap, Dash, ThrowStone, Ground
    }

    [SerializeField]
    private GameObject projectilePrefab;

    private GameObject projectileObj;

    private Transform projectileSpawnTransform;
    private bool bPickUp;
    private Coroutine dashCollisionRoutine;

    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Boss;
    }

    protected override void Awake()
    {
        base.Awake();

        projectileSpawnTransform = rootObject.transform.FindChildByName("Muzzle");
        Debug.Assert(projectileSpawnTransform != null);

        for (int i = 0; i < (int)PartType.Max; i++)
        {
            Transform t = colliders[i].transform;
            t.DetachChildren();
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;

            Fist_Trigger trigger = t.GetComponent<Fist_Trigger>();
            trigger.OnTrigger += OnTriggerEnter;

            string partName = ((PartType)i).ToString();
            Transform parent = rootObject.transform.FindChildByName(partName);
            Debug.Assert(parent != null);

            t.SetParent(parent, false);
        }
    }

    protected override void Update()
    {
        base.Update();

        if (bPickUp == false)
            return;

        GameObject player = GameManager.Instance.PlayerInstance.gameObject;
        if (player == null) return;

        Vector3 direction = (player.transform.position + Vector3.up * 2.0f) - projectileSpawnTransform.position;
        Quaternion q = Quaternion.LookRotation(direction);

        rootObject.transform.rotation = Quaternion.Slerp(rootObject.transform.rotation, q, 5.0f * Time.deltaTime);
        projectileSpawnTransform.localRotation = Quaternion.Euler(q.eulerAngles.x, 0.0f, 0.0f);
    }

    public override void Begin_Collision(AnimationEvent e)
    {
        //base.Begin_Collision();

        hitIndex++;
        //colliders[e.intParameter].enabled = true;

        List<Collider> colliders = new List<Collider>();

        switch((AttackType)actionIndex)
        {
            case AttackType.LeftPunch:
            case AttackType.Clap:
            {
                colliders = new List<Collider>(Physics.OverlapSphere(rootObject.transform.position, 3.0f, 1 << 6));
                colliders.RemoveAll(RemoveCondition);
                break;
            }
            case AttackType.Dash:
            {
                dashCollisionRoutine = StartCoroutine(DashCollision(colliders));
                break;
            }
            case AttackType.Ground:
            {
                colliders = new List<Collider>(Physics.OverlapSphere(rootObject.transform.position, 20.0f, 1 << 6));
                colliders.RemoveAll(collider => Mathf.Abs(collider.transform.position.y - rootObject.transform.position.y) > 0.2f);
                SoundManager.Instance.PlaySound("Boss_Ground", SoundType.Effect, rootObject.transform);
                break;
            }
        }

        foreach (Collider collider in colliders)
        {
            if (collider.transform.tag == "Player")
            {
                OnTriggerEnter(collider);
            }
        }
    }

    private IEnumerator DashCollision(List<Collider> colliders)
    {
        while (true)
        {
            yield return new WaitForFixedUpdate();

            Collider[] origins = Physics.OverlapSphere(rootObject.transform.position, 3.0f, 1 << 6);
            foreach (Collider collider in origins)
            {
                colliders.Add(collider);
            }

            colliders.RemoveAll(RemoveCondition);

            foreach (Collider collider in colliders)
            {
                if (collider.transform.tag == "Player")
                {
                    OnTriggerEnter(collider);
                }
            }
        }
    }

    private bool RemoveCondition(Collider collider)
    {
        Vector3 direction = collider.transform.position - rootObject.transform.position;
        float dot = Vector3.Dot(rootObject.transform.forward, direction);

        return dot < 0;
    }

    Transform projectileParent;

    public override void PickUp()
    {
        base.PickUp();

        bPickUp = true;
        projectileObj = Instantiate(projectilePrefab, projectileSpawnTransform, false);
        projectileParent = projectileObj.transform.parent;
    }

    public override void DoAction(int actionIndex = 0)
    {
        base.DoAction(actionIndex);

        string name = "BossAction";
        name += actionIndex.ToString();
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }


    public override void End_Collision()
    {
        base.End_Collision();

        if ((AttackType)actionIndex == AttackType.Dash)
            StopCoroutine(dashCollisionRoutine);
    }
   

    public override void Shoot_Projectile(AnimationEvent e)
    {
        base.Shoot_Projectile(e);

        bPickUp = false;
        Transform t = projectileObj.transform;
        Vector3 originScale = t.localScale;
        t.parent = null;


        //투사체 위치 선정
        Vector3 muzzlePosition = projectileSpawnTransform.position;
        muzzlePosition += rootObject.transform.forward * 0.75f;
        t.position = muzzlePosition;
        t.rotation = rootObject.transform.rotation * projectileSpawnTransform.localRotation;
        t.localScale = originScale;

        Projectile projectile = projectileObj.GetComponent<Projectile>();
        projectile.Shoot(1.0f, t.forward);
        projectile.OnProjectileHit += OnProjectileHit;
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

        Destroy(projectile);
    }

}