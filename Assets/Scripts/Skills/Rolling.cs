using UnityEngine;

public class Rolling : Skill
{
    [SerializeField]
    private GameObject trailParticlePrefab;

    private GameObject[] trailParticleObjects = new GameObject[2];
    private Transform left;
    private Transform right;

    protected override void Reset()
    {
        base.Reset();

        type = SkillType.Rolling;
    }

    protected override void Start()
    {
        base.Start();

        left = rootObject.transform.FindChildByName("LeftBlade");
        right = rootObject.transform.FindChildByName("RightBlade");
    }

    public override void Begin_Collision(AnimationEvent e)
    {
        //base.Begin_Collision(e);
        hitIndex++;
        Collider[] colliders = Physics.OverlapSphere(rootObject.transform.position, 3.0f);
        
        foreach (Collider collider in colliders)
        {
            if (collider.transform == rootObject.transform)
                continue;

            OnTriggerEnter(collider);
        }
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        if (trailParticlePrefab == null) return;
        if (left == null || right == null) return;

        trailParticleObjects[0] = Instantiate(trailParticlePrefab, left);
        trailParticleObjects[1] = Instantiate(trailParticlePrefab, right);

        string name = "Roll_Effect";
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
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

    public override void Play_SlashSound()
    {
        base.Play_SlashSound();

        string name = "Dual_Swing_";
        name += SoundManager.Instance.SoundRandomRange(1, 2);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }
}