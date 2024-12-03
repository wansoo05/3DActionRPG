using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPoolCreator
{
    private SpawnData[] spawnDatas;

    private Dictionary<string, List<GameObject>> poolingTable;
    private Dictionary<string, SpawnData> spawnDataDic;

    public ObjectPoolCreator(SpawnData[] spawnDatas, Transform parentTransform)
    {
        this.spawnDatas = spawnDatas;

        poolingTable = new Dictionary<string, List<GameObject>>();
        spawnDataDic = new Dictionary<string, SpawnData>();

        for (int i = 0; i < spawnDatas.Length; i++)
        {
            GameObject obj = new GameObject(spawnDatas[i].name);
            obj.transform.SetParent(parentTransform, false);

            List<GameObject> pool = CreateObjectPool(obj, spawnDatas[i], 0, spawnDatas[i].SpawnCount);
            poolingTable.Add(spawnDatas[i].name, pool);

            spawnDataDic.Add(spawnDatas[i].name, spawnDatas[i]);
        }
    }

    private List<GameObject> CreateObjectPool(GameObject parent, SpawnData spawnData, int start, int end)
    {
        List<GameObject> result = new List<GameObject>(spawnData.SpawnCount);

        for (int i = start; i < end; i++)
        {
            GameObject obj = GameObject.Instantiate<GameObject>(spawnData.SpawnPrefab, parent.transform, false);
            obj.name = $"{spawnData.SpawnPrefab.name}_{i:000}";

            obj.SetActive(false);
            result.Add(obj);
        }

        return result;
    }
        
    public GameObject TakeObject(string spawnDataName)
    {
        List<GameObject> possiblePool = poolingTable[spawnDataName];

        foreach (GameObject obj in possiblePool)
        {
            if (obj.activeSelf == false)
                return obj;
        }

        //비활성화된 게임 오브젝트가 없다면
        ExpandPool(spawnDataName);  //풀 늘리기
        return TakeObject(spawnDataName); //다시 오브젝트 꺼내기
    }
    
    public void ReturnObject(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void ReturnAllObject()
    {
        foreach (List<GameObject> pool in poolingTable.Values)
        {
            foreach(GameObject obj in pool)
            {
                obj.SetActive(false);
            }
        }
    }

    public void ExpandPool(string name)
    {
        List<GameObject> pool = poolingTable[name];
        SpawnData spawnData = spawnDataDic[name];
        int newSize = pool.Count + spawnData.SpawnCount;

        List<GameObject> temp = CreateObjectPool(pool[0].transform.parent.gameObject, spawnData, pool.Count, newSize);
        foreach (GameObject obj in temp)
            pool.Add(obj);
    }
}