using UnityEngine;

public class CharacterPool : ObjectPool
{

    public override GameObject GetObject(string spawnDataName)
    {
        return pool.TakeObject(spawnDataName);
    }

}