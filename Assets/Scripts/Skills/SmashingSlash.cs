using UnityEngine;

public class SmashingSlash : Skill
{
    protected override void Reset()
    {
        base.Reset();

        type = SkillType.SmashingSlash;
    }

    public override void Begin_Collision(AnimationEvent e)
    {
        hitIndex++;
        Collider[] colliders = Physics.OverlapSphere(rootObject.transform.position, 3.0f, 1 << 6);

        foreach(Collider collider in colliders)
        {
            if (collider.transform == rootObject.transform)
                continue;

            OnTriggerEnter(collider);
        }
    }


    public override void Play_Particle()
    {
        base.Play_Particle();

        if (doActionData.hittingDatas[0].Particle == null)
            return;

        Vector3 particlePosition = rootObject.transform.position + rootObject.transform.forward;
        Instantiate(doActionData.hittingDatas[0].Particle, particlePosition, Quaternion.identity);
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        string name = "FireWarmupExplosion2";
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

}