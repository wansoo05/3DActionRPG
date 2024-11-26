using UnityEngine;

public class None : Skill
{

    protected override void Reset()
    {
        base.Reset();

        type = SkillType.None;
    }
}