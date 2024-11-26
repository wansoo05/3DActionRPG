using UnityEngine;

public enum HitType
{
    Basic = 0, Down = 1, Air = 2, Smash = 3, Explosion = 4, StunDown = 5,
}

[CreateAssetMenu(fileName = "HittingData", menuName = "ScriptableObjects/HittingData", order =1)]
public class HittingData : ScriptableObject
{
    public float Power;
    public float Distance;
    public int StopFrame;

    public GameObject Particle;
    public Vector3 ParticlePositionOffset;
    public Vector3 ParticleScaleOffset = Vector3.one;

    public Vector3 ImpulseDirection;
    public Cinemachine.NoiseSettings ImpulseSettings;

    public HitType HitImpactIndex;

    public string HitParticleDataName;
    public Vector3 HitParticlePositionOffset;
    public Vector3 HitParticleScaleOffset = Vector3.one;

    public string HitSoundName;
    public int HitSoundRangeFrom;
    public int HitSoundRangeTo;
}