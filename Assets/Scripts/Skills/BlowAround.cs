using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BlowAround : Skill
{
    [SerializeField]
    private float maxDistance = 10.0f;

    [SerializeField]
    private float maxRadius = 3.0f;

    protected override void Reset()
    {
        base.Reset();

        type = SkillType.BlowAround;
    }

    public override void Play_Particle()
    {
        base.Play_Particle();

        if (doActionData.hittingDatas[0].Particle == null)
            return;

        Vector3 particlePosition = rootObject.transform.position;
        Instantiate(doActionData.hittingDatas[0].Particle, particlePosition, Quaternion.identity);

        string name = "Magic_Spell_Electronic";
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        Vector3 start = rootObject.transform.position + Vector3.up;
        Vector3 end = start + rootObject.transform.forward * maxDistance;
        List<Collider> colliders = new List<Collider>(Physics.OverlapCapsule(start, end, 5.0f, 1 << 6));

        colliders.RemoveAll(RemoveCondition);
        GameObject target = GetNearlyTarget(colliders);

        if (target != null)
        {
            Vector2 randomPosition = Random.insideUnitCircle * 2.0f;
            Vector3 position = new Vector3(target.transform.position.x + randomPosition.x, target.transform.position.y, target.transform.position.z + randomPosition.y);

            NavMeshHit hit;

            if (NavMesh.SamplePosition(position, out hit, 1.0f, NavMesh.AllAreas))
                position = hit.position;

            Vector3 movement = position - rootObject.transform.position;
            CharacterController controller = rootObject.GetComponent<CharacterController>();
            controller.Move(movement);
        }
    }

    public override void Begin_Collision(AnimationEvent e)
    {
        //base.Begin_Collision(e);
        hitIndex++;
        Vector3 position = rootObject.transform.position;
        Collider[] colliders = Physics.OverlapSphere(position, maxRadius, 1 << 6);

        foreach(Collider collider in colliders)
        {
            if (collider.CompareTag(rootObject.tag))
                continue;

            OnTriggerEnter(collider);
        }
    }

    private bool RemoveCondition(Collider collider)
    {
        if (collider.CompareTag(rootObject.tag))
            return true;

        Vector3 position = rootObject.transform.position;
        Vector3 direction = collider.transform.position - position;
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up, direction, out hit))
        {
            if (hit.transform != collider.transform)
            {
                print(hit.transform.name);
                return true;
            }
        }

        return false;
    }

    private GameObject GetNearlyTarget(List<Collider> colliders)
    {
        if (colliders.Count == 0)
            return null;

        GameObject target = null;
        float maxDot = 0;
        foreach(Collider collider in colliders)
        {
            Vector3 direction = collider.transform.position - rootObject.transform.position;
            float dot = Vector3.Dot(rootObject.transform.forward, direction.normalized);

            if (maxDot < dot)
            {
                maxDot = dot;
                target = collider.gameObject;
            }
        }

        return target;
    }

    private void OnDrawGizmos()
    {
        Vector3 position = rootObject.transform.position + Vector3.up;
        Gizmos.DrawLine(position, position + rootObject.transform.forward * maxDistance);
    }
}