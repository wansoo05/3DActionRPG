using UnityEngine;

public class Sword : Melee
{
    [SerializeField]
    private string holsterName = "Holster_Sword";

    [SerializeField]
    private string swordTransformName = "Hand_Sword";

    [SerializeField]
    private string shieldTransformName = "Hand_Shield";

    [SerializeField]
    private GameObject shieldPrefab;
    private GameObject shield;

    private Transform holsterTransform;
    private Transform swordTransform;
    private Transform shieldTransform;

    private Transform start;
    private Transform end;


    protected override void Reset()
    {
        base.Reset();

        type = WeaponType.Warrior;
    }
    protected override void Awake()
    {
        base.Awake();

        
        start = transform.FindChildByName("Start");
        end = transform.FindChildByName("End");
    }
    Vector3 to;
    protected override void Start()
    {
        base.Start();

        holsterTransform = rootObject.transform.FindChildByName(holsterName);
        Debug.Assert(holsterTransform != null);

        swordTransform = rootObject.transform.FindChildByName(swordTransformName);
        Debug.Assert(swordTransform != null);

        shieldTransform = rootObject.transform.FindChildByName(shieldTransformName);
        Debug.Assert(shieldTransform != null);

        if (shieldPrefab != null)
        {
            shield = Instantiate<GameObject>(shieldPrefab, shieldTransform);
            shield.SetActive(false);
        }

        transform.SetParent(holsterTransform, false);
    }
    public override void Begin_Equip()
    {
        base.Begin_Equip();

        transform.parent.DetachChildren();
        transform.position = Vector3.zero;
        transform.rotation = Quaternion.identity;

        transform.SetParent(swordTransform, false);

        shield.SetActive(true);
        if (isPlayer)
            SoundManager.Instance.PlaySound("Sword_Drawing", SoundType.Effect, rootObject.transform);
    }

    public override void UnEquip()
    {
        base.UnEquip();

        transform.SetParent(holsterTransform, false);

        shield.SetActive(false);
    }

    public override void DoEvade()
    {
        base.DoEvade();
    }


    public override void Play_SlashSound()
    {
        base.Play_SlashSound();
        string name = "Sword_Swing_";
        name += SoundManager.Instance.SoundRandomRange(1, 4);
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform, 0.3f);
    }
}