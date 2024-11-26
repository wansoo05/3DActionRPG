using UnityEngine;

public class Cross : Skill
{
    [SerializeField]
    private GameObject trailParticlePrefab;


    private GameObject[] trailParticleObjects = new GameObject[2];
    private Transform left;
    private Transform right;

    protected override void Reset()
    {
        base.Reset();

        type = SkillType.Cross;
    }
    protected override void Start()
    {
        base.Start();

        left = rootObject.transform.FindChildByName("LeftBlade");
        right = rootObject.transform.FindChildByName("RightBlade");
    }
    public override void Begin_Collision(AnimationEvent e)
    {
        hitIndex++;


        Collider[] colliders = Physics.OverlapSphere(rootObject.transform.position, 3.0f, (1 << 6));

        foreach (Collider collider in colliders)
        {
            if (collider.transform != rootObject.transform)
            {
                OnTriggerEnter(collider);
            }
        }
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        if (trailParticlePrefab == null) return;
        if (left == null || right == null) return;

        trailParticleObjects[0] = Instantiate(trailParticlePrefab, left);
        trailParticleObjects[1] = Instantiate(trailParticlePrefab, right);
    }

    public override void End_DoAction()
    {
        base.End_DoAction();

        if (trailParticleObjects[0] != null && trailParticleObjects[1] != null)
        {
            Destroy(trailParticleObjects[0]);
            Destroy(trailParticleObjects[1]);
        }
    }

    public override void Play_Particle()
    {
        base.Play_Particle();

        if (doActionData.hittingDatas[hitIndex].Particle == null)
            return;

        Vector3 particlePosition = rootObject.transform.position;
        Instantiate(doActionData.hittingDatas[hitIndex].Particle, particlePosition, Quaternion.identity);

        string name = "Cross_Explosion";
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

    public override void Play_SlashSound()
    {
        base.Play_SlashSound();

        string name = "Dual_Swing_";
        name += SoundManager.Instance.SoundRandomRange(1, 2);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }
}