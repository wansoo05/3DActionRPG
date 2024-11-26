using UnityEngine;

public class ChargingSlash : Skill
{
    [SerializeField]
    private string swordTransformName = "Hand_Sword";

    private Transform swordTransform;

    protected override void Reset()
    {
        base.Reset();

        type = SkillType.ChargingSlash;
    }

    protected override void Start()
    {
        base.Start();

        swordTransform = rootObject.transform.FindChildByName(swordTransformName);
        Debug.Assert(swordTransform != null);

        transform.SetParent(swordTransform, false);
    }

    public override void Play_Particle()
    {
        base.Play_Particle();

        if (doActionData.hittingDatas[0].Particle == null)
            return;

        Vector3 particlePosition = rootObject.transform.position + rootObject.transform.up + rootObject.transform.forward * 2.0f;
        Instantiate(doActionData.hittingDatas[0].Particle, particlePosition, Quaternion.identity);
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        string name = "FireWarmupExplosion";

        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

}