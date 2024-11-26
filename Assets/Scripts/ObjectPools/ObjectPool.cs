using System.Collections;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [SerializeField]
    private SpawnData[] spawnDatas;

    protected ObjectPoolCreator pool;

    public virtual void Initialize()
    {
        if (pool == null)
        {
            pool = new ObjectPoolCreator(spawnDatas, transform);
        }
        else
        {
            pool.ReturnAllObject();
        }
    }
    
    public virtual GameObject GetObject(string spawnDataName)
    {
        GameObject obj = pool.TakeObject(spawnDataName);
        obj.SetActive(true);
        return obj;
    }

    public virtual void ReturnObject(GameObject obj)
    {
        pool.ReturnObject(obj);
    }
}