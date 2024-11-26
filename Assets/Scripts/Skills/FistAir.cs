using Cinemachine;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class FistAir : Skill
{
    protected override void Reset()
    {
        base.Reset();

        type = SkillType.FistAir;
    }

    protected override void Awake()
    {
        base.Awake();

    }
    public override void Begin_Collision(AnimationEvent e)
    {
        hitIndex++;

        Vector3 startPosition = rootObject.transform.position + Vector3.up;
        Vector3 endPosition = startPosition + rootObject.transform.forward * 1.5f;

        Stack<Collider> colliders = new Stack<Collider>(Physics.OverlapCapsule(startPosition, endPosition, 1.0f, (1 << 6)));

        while (colliders.Count > 0)
        {
            Collider target = colliders.Pop();
            if (!target.CompareTag(rootObject.tag))
            {
                OnTriggerEnter(target);
                return;
            }
        }

    }

    public override void End_DoAction()
    {
        base.End_DoAction();
        animator.SetBool("IsAirCombo", false);
    }

    public override void Play_SlashSound()
    {
        base.Play_SlashSound();

        string name = "Fist_Swing_";
        name += SoundManager.Instance.SoundRandomRange(1, 4);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

}