using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;

public class Katana : Melee
{
    [SerializeField]
    private string katanaTransformName = "Hand_Katana";

    private Transform katanaTransform;

    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Katana;
    }

    protected override void Awake()
    {
        base.Awake();

        katanaTransform = rootObject.transform.FindChildByName(katanaTransformName);
        Debug.Assert(katanaTransform != null);
        transform.SetParent(katanaTransform, false);
        gameObject.SetActive(false);
    }

    public override void Begin_Equip()
    {
        base.Begin_Equip();

        gameObject.SetActive(true);

        if (isPlayer)
            SoundManager.Instance.PlaySound("Katana_Drawing", SoundType.Effect, rootObject.transform);
    }

    public override void UnEquip()
    {
        base.UnEquip();

        gameObject.SetActive(false);
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

    }

    public override void Play_Particle()
    {
        base.Play_Particle();

    }

    public override void Play_SlashSound()
    {
        base.Play_SlashSound();

        //검기 사운드 재생
        string name = "Katana_Swing_";
        name += SoundManager.Instance.SoundRandomRange(1, 2);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform, 0.3f);
    }
}