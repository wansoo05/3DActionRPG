using UnityEngine;

public interface IStateUpdatable
{
    public void Save();
    public void Load(PlayerStateData data);
}