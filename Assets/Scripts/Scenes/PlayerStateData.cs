using UnityEngine;

public struct PlayerStateData
{
    public float Hp;
    public float Mp;
    public WeaponType Type;

    public void Initialize()
    {
        Hp = 0;
        Mp = 0;
        Type = WeaponType.Unarmed;
    }
}