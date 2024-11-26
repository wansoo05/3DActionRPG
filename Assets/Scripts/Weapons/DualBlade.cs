using UnityEngine;

public class DualBlade : Melee
{
    private enum PartType
    {
        LeftBlade = 0, RightBlade, Max,
    }

    private GameObject[] blades = new GameObject[(int)PartType.Max];
    private TrailRenderer[] trails = new TrailRenderer[(int)PartType.Max];

    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Dual;
    }

    protected override void Awake()
    {
        base.Awake();


        for (int i = 0; i < (int)PartType.Max; i++)
        {
            Transform t = colliders[i].transform;

            //t.DetachChildren();
            //t.localPosition = Vector3.zero;
            //t.localRotation = Quaternion.identity;

            Dual_Trigger trigger = t.GetComponent<Dual_Trigger>();
            trigger.OnTrigger += OnTriggerEnter;

            string partName = ((PartType)i).ToString();
            Transform parent = rootObject.transform.FindChildByName(partName);
            Debug.Assert(parent != null);

            t.SetParent(parent, false);
            blades[i] = t.gameObject;
            trails[i] = blades[i].GetComponentInChildren<TrailRenderer>();
            trails[i].enabled = false;
            blades[i].SetActive(false);
        }
    }

    public override void Play_Slash(int index)
    {
        for (int i = 0; i < (int)PartType.Max; i++)
        {
            if (trails[i] != null)
                trails[i].enabled = true;
        }
    }

    public override void End_Slash()
    {
        for (int i = 0; i < (int)PartType.Max; i++)
        {
            if (trails[i] != null)
                trails[i].enabled = false;
        }
    }

    public override void Begin_Equip()
    {
        base.Begin_Equip();

        for (int i = 0; i < (int)PartType.Max; i++)
        {
            blades[i].SetActive(true);
        }

        if (isPlayer)
            SoundManager.Instance.PlaySound("Dual_Drawing", SoundType.Effect, rootObject.transform);
    }

    public override void UnEquip()
    {
        base.UnEquip();

        for (int i = 0; i < (int)PartType.Max; i++)
        {
            blades[i].SetActive(false);
        }
    }

    public override void Play_SlashSound()
    {
        base.Play_SlashSound();

        string name = "Dual_Swing_";
        name += SoundManager.Instance.SoundRandomRange(1, 2);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }
}