using UnityEngine;

public class Boss : Melee
{
    private enum PartType
    {
        LeftArm = 0, RightArm, Max,
    }

    [SerializeField]
    private GameObject projectilePrefab;

    private GameObject projectileObj;

    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Boss;
    }

    protected override void Awake()
    {
        base.Awake();

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

    public override void Begin_Collision(AnimationEvent e)
    {
        hitIndex++;
        colliders[e.intParameter].enabled = true;
    }

    public override void Play_SlashSound()
    {
        base.Play_SlashSound();

        string name = "Boss_Swing_";
        name += SoundManager.Instance.SoundRandomRange(1, 3);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }
}