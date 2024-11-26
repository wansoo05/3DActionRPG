using UnityEngine;

public class Heal : Skill
{
    [SerializeField]
    private float duration = 2.0f;

    [SerializeField]
    private GameObject healCirclePrefab;

    private GameObject healCircleObject;

    protected override void Reset()
    {
        base.Reset();
        type = SkillType.Heal;
    }


    public override void Begin_DoAction()
    {
        SoundManager.Instance.PlaySound("Heal", SoundType.Effect, rootObject.transform);
        healCircleObject = Instantiate(healCirclePrefab, rootObject.transform.position + Vector3.up * 0.1f, Quaternion.identity);
        Destroy(healCircleObject, duration);
    }


}