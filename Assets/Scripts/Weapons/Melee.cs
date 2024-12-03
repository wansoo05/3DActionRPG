using System.Collections.Generic;
using Cinemachine;
using UnityEngine;

public class Melee : Weapon
{
    protected Collider[] colliders;
    private List<GameObject> hittedList;

    protected CinemachineImpulseSource impulse;
    protected PlayerMovingComponent moving;
    protected PlayerCameraController cameraController;

    protected override void Awake()
    {
        base.Awake();

        colliders = GetComponentsInChildren<Collider>();
        hittedList = new List<GameObject>();
        impulse = GetComponent<CinemachineImpulseSource>();

        moving = rootObject.GetComponent<PlayerMovingComponent>();
        cameraController = rootObject.GetComponent<PlayerCameraController>();
    }

    protected override void Start()
    {
        base.Start();

        End_Collision();
    }

    protected override void Update()
    {
        base.Update();
    }


    public virtual void Begin_Collision(AnimationEvent e)
    {
        foreach (Collider collider in colliders)
            collider.enabled = true;

        hitIndex++;
    }

    public virtual void End_Collision()
    {
        foreach (Collider collider in colliders)
            collider.enabled = false;

        hittedList.Clear();
    }

    private void Play_Impulse()
    {
        if (impulse == null)
            return;

        if (doActionDatas[actionIndex].hittingDatas[hitIndex].ImpulseSettings == null)
            return;

        if (doActionDatas[actionIndex].hittingDatas[hitIndex].ImpulseDirection.magnitude == 0)
            return;

        cameraController.CameraShaking(impulse, doActionDatas[actionIndex].hittingDatas[hitIndex].ImpulseSettings, doActionDatas[actionIndex].hittingDatas[hitIndex].ImpulseDirection);
    }

    public virtual void Play_Slash(int i)
    {
        if (trail != null)
            trail.enabled = true;
    }

    public virtual void End_Slash()
    {
        if (trail != null)
            trail.enabled = false;
    }

    public virtual void Play_SlashSound()
    {
        
    }

    public virtual void PickUp()
    {

    }

    protected void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == rootObject.tag)
            return;

        if (hittedList.Contains(other.gameObject) == true)
            return;

        hittedList.Add(other.gameObject);

        IDamagable damage = other.gameObject.GetComponent<IDamagable>();

        if (damage == null)
            return;

        Vector3 hitPoint = Vector3.zero;

        Collider enabledCollider = null;
        foreach(Collider collider in colliders)
        {
            if (collider.enabled == true)
            {
                enabledCollider = collider;

                break;
            }
        }


        if (enabledCollider != null)
        {
            hitPoint = enabledCollider.ClosestPoint(other.transform.position);
            hitPoint = other.transform.InverseTransformPoint(hitPoint);
        }

        //플레이어가 공격하였을 때
        if (isPlayer)
        {
            Play_Impulse();
        }

        Character character = rootObject.GetComponent<Character>();
        damage.OnDamage(rootObject, this, hitPoint, doActionDatas[actionIndex].hittingDatas[hitIndex]);
    }

    /// <summary>
    /// target을 향하여 바라보게 회전시켜주는 함수
    /// </summary>
    /// <param name="target"></param>
    private void lookRotate(GameObject target)
    {
        if (isPlayer == false)
            return;

        float angle = 5.0f;
        
        Vector3 direction = target.transform.position - rootObject.transform.position;
        direction.y = 0.0f;
        if (Vector3.Dot(rootObject.transform.forward, direction.normalized) >= Mathf.Cos(angle))
        {
            Quaternion q = Quaternion.FromToRotation(rootObject.transform.forward, direction.normalized);
            rootObject.transform.rotation *= q;
        }
    }
}