using UnityEngine;

public class Fist : Melee
{
    private enum PartType
    {
        LeftHand = 0, RightHand, LeftFoot, RightFoot, Max,
    }

    private TrailRenderer[] trails = new TrailRenderer[(int)PartType.Max];

    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Fist;
    }

    protected override void Awake()
    {
        base.Awake();

        for (int i = 0; i < (int)PartType.Max; i++)
        {
            Transform t = colliders[i].transform;

            Fist_Trigger trigger = t.GetComponent<Fist_Trigger>();
            trigger.OnTrigger += OnTriggerEnter;

            string partName = ((PartType)i).ToString();
            Transform parent = rootObject.transform.FindChildByName(partName);
            Debug.Assert(parent != null);

            t.SetParent(parent, false);

            trails[i] = t.gameObject.GetComponentInChildren<TrailRenderer>();
            trails[i].enabled = false;
        }
    }

    public override void Play_Slash(int index)
    {
        if (trails[index] != null)
            trails[index].enabled = true;
    }

    public override void End_Slash()
    {
        for (int i = 0; i < (int)PartType.Max; i++)
        {
            if (trails[i] != null)
                trails[i].enabled = false;
        }
    }

    public override void Play_SlashSound()
    {
        base.Play_SlashSound();

        string name = "Fist_Swing_";
        name += SoundManager.Instance.SoundRandomRange(1, 4);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

    public override void Begin_Collision(AnimationEvent e)
    {
        hitIndex++;
        colliders[e.intParameter].enabled = true;
    }

}