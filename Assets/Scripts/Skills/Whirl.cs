using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Whirl : Skill
{

    protected override void Reset()
    {
        base.Reset();

        type = SkillType.Whirl;
    }

    protected override void Awake()
    {
        base.Awake();


    }

    public override void Begin_Collision(AnimationEvent e)
    { 
        hitIndex++;

        Collider[] colliders = Physics.OverlapSphere(rootObject.transform.position, 2.0f, (1 << 6));

        SortedDictionary<float, Collider> selectTagets = new SortedDictionary<float, Collider>();
        foreach(Collider collider in colliders)
        {
            if (collider.transform == rootObject.transform)
                continue;

            Vector3 direction = collider.transform.position - rootObject.transform.position;
            Vector3 forward = rootObject.transform.forward;

            if (Vector3.Dot(direction.normalized, forward) >= 0.2f)
                OnTriggerEnter(collider);
        }


    }

    public override void Play_SlashSound()
    {
        base.Play_SlashSound();

        if (hitIndex == 5)
        {
            //검기 사운드 재생
            string name = "Slash_LightningStrike";
            SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
        }
        else
        {
            string name = "Katana_Swing_";
            name += SoundManager.Instance.SoundRandomRange(1, 2);
            SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform, 0.5f);
        }
    }

    public override void Play_Particle()
    {
        base.Play_Particle();

        if (doActionData.hittingDatas[hitIndex].Particle == null)
            return;

        Vector3 position = rootObject.transform.position + rootObject.transform.forward * 0.5f;
        position.y += 2.0f;
        Quaternion rotation = rootObject.transform.rotation;
        Quaternion q = Quaternion.Euler(0, 0, -33f);
        rotation *= q;
        GameObject slashEffect = Instantiate(doActionData.hittingDatas[hitIndex].Particle, position, rotation);
        slashEffect.transform.localScale = doActionData.hittingDatas[hitIndex].ParticleScaleOffset;
    }
    public override void End_DoAction()
    {
        base.End_DoAction();
        animator.SetBool("IsAirCombo", false);
    }
}