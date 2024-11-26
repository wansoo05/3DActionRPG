using UnityEngine;

public class FistUpper : Skill
{
    protected override void Reset()
    {
        base.Reset();

        type = SkillType.FistUpper;
    }

    public override void Begin_Collision(AnimationEvent e)
    {
        hitIndex++;

        Vector3 startPosition = rootObject.transform.position + Vector3.up;
        Vector3 endPosition = startPosition + rootObject.transform.forward * 1.5f;

        Collider[] colliders = Physics.OverlapCapsule(startPosition, endPosition, 1.5f, (1 << 6));

        foreach (Collider collider in colliders)
        {
            if (collider.transform != rootObject.transform)
                OnTriggerEnter(collider);
        }
    }

    public override void Play_Particle()
    {
        base.Play_Particle();

        if (doActionData.hittingDatas[hitIndex].Particle == null)
            return;

        GameObject obj = Instantiate(doActionData.hittingDatas[hitIndex].Particle, rootObject.transform.position + rootObject.transform.forward * 0.5f, Quaternion.identity);
        obj.transform.localScale = doActionData.hittingDatas[hitIndex].ParticleScaleOffset;
    }

    public override void Play_SlashSound()
    {
        base.Play_SlashSound();

        string name;
        if (hitIndex == 3)
        {
            name = "Punch_Finisher";
        }
        else
        {
            name = "Fist_Swing_";
            name += SoundManager.Instance.SoundRandomRange(1, 4);
        }

        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }
}