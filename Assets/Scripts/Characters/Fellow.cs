using Cinemachine;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem.XR;

public class Fellow : Character, IDamagable
{
    [SerializeField]
    private Color damageColor = Color.red;


    [SerializeField]
    private float changeColorTime = 0.15f;

    private Color originColor;
    private Material skinMaterial;

    private AIController aiController;
    private NavMeshAgent navMeshAgent;


    protected override void Awake()
    {
        base.Awake();

        Transform surface = transform.FindChildByName("Surface");
        Debug.Assert(surface != null);
        skinMaterial = surface.GetComponent<SkinnedMeshRenderer>().material;
        originColor = skinMaterial.color;
        aiController = GetComponent<AIController>();
        navMeshAgent = GetComponent<NavMeshAgent>();
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

        weapon.Secure_EndDoAction();
        weapon.SetUnarmedMode();
    }

    public void OnDamage(GameObject attacker, Weapon causer, Vector3 hitPoint, HittingData data)
    {
        healthPoint.Damage(data.Power);

        transform.forward = -attacker.transform.forward;

        StartCoroutine(Change_Color(changeColorTime));

        if (data.HitParticleDataName != null)
        {
            GameObject obj = Factory.Instance.GetEffect(data.HitParticleDataName);
            if (obj != null)
            {
                obj.transform.position = transform.position;
                obj.transform.position += hitPoint + data.HitParticlePositionOffset;
                obj.transform.localScale = data.HitParticleScaleOffset;
            }
        }

        if (healthPoint.Dead == false)
        {
            if (GetComponent<CinemachineImpulseSource>() == true && state.ActionMode)
                return;


            animator.SetTrigger("Damaged");
            animator.SetInteger("ImpactType", (int)data.HitImpactIndex);


            rigidbody.isKinematic = false;
            float force = data.Distance * 10.0f;

            switch (data.HitImpactIndex)
            {
                case HitType.Down:
                {
                    rigidbody.AddForce(-transform.forward * force * 10.0f, ForceMode.Force);
                    aiController?.SetState(AIController.State.Down);
                    StartCoroutine(Change_IsKinematics(5));

                    break;
                }

                case HitType.Smash:
                {
                    aiController?.SetState(AIController.State.Down);
                    rigidbody.AddForce(Vector3.down * force, ForceMode.Impulse);
                    StartCoroutine(Change_IsKinematics(5));

                    break;
                }

                case HitType.Explosion:
                {
                    print("Explosion");
                    aiController?.SetState(AIController.State.Down);
                    rigidbody.AddExplosionForce(1000.0f, transform.position + transform.forward, 5.0f);
                    StartCoroutine(Change_IsKinematics(5));
                    break;
                }

                case HitType.StunDown:
                {
                    aiController?.SetState(AIController.State.Down);
                    StartCoroutine(Change_IsKinematics(5));
                    break;
                }

                default:
                {
                    rigidbody.AddForce(-transform.forward * force * 10.0f, ForceMode.Force);
                    aiController?.SetState(AIController.State.Damaged);
                    StartCoroutine(Change_IsKinematics(5));
                    break;
                }
            }

            return;
        }

        Collider collider = GetComponent<Collider>();
        collider.enabled = false;

        aiController?.SetState(AIController.State.Dead);
        animator.SetBool("IsDead", true);
        Factory.Instance.ReturnEffectDelay(gameObject, 3.0f);
    }

    private IEnumerator Change_Color(float time)
    {
        skinMaterial.color = damageColor;

        yield return new WaitForSeconds(time);

        skinMaterial.color = originColor;
    }
    private IEnumerator Change_IsKinematics(int frame)
    {
        for (int i = 0; i < frame; i++)
            yield return new WaitForFixedUpdate();

        rigidbody.isKinematic = true;
    }

}