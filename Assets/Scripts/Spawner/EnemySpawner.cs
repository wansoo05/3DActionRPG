using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
using static Factory;

public class EnemySpawner : MonoBehaviour
{
    [SerializeField]
    private string[] enemyPrefabNames;

    [SerializeField]
    private GameObject[] spawnPoints;

    [SerializeField]
    private int maxCount;

    private int currentCount;

    public delegate Vector3 RandomPointDelegate();
    private RandomPointDelegate randomPointDelegate;

    private void Start()
    {
        Factory.Instance.OnEnemyDead += OnEnemyDead;

        if (spawnPoints.Length == 0)
            return;
        
        SetRandomPointDelegate(RandomSpawnPoint);
        for (int i = 0; i < maxCount; i++)
        {
            SpawnEnemy();
        }

        StartCoroutine(RespawnEnemy());
    }

    /// <summary>
    /// 적을 맵에 랜덤한 위치로 스폰 시키는 함수
    /// </summary>
    /// <returns></returns>
    public GameObject SpawnEnemy()
    {
        int random = UnityEngine.Random.Range(0, enemyPrefabNames.Length);
        GameObject obj = Factory.Instance.GetCharacter(enemyPrefabNames[random]);

        if (obj == null)
            return obj;

        Vector3 spawnPosition = randomPointDelegate();
        NavMeshHit hit = new NavMeshHit();
        float maxDistance = 1.0f;
        for (int i = 0; i < 10; i++)
        {
            if (NavMesh.SamplePosition(spawnPosition, out hit, maxDistance, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
                break;
            }
            else
            {
                spawnPosition = randomPointDelegate();
                maxDistance *= 2.0f;
            }
        }
        obj.transform.position = spawnPosition;
        obj.SetActive(true);

        currentCount++;
        return obj;
    }

    public GameObject SpawnEnemyWithEffect(string effectDataName)
    {
        int random = UnityEngine.Random.Range(0, enemyPrefabNames.Length);
        GameObject obj = Factory.Instance.GetCharacter(enemyPrefabNames[random]);

        if (obj == null)
            return obj;

        Vector3 spawnPosition = randomPointDelegate();
        NavMeshHit hit = new NavMeshHit();
        float maxDistance = 1.0f;
        for (int i = 0; i < 10; i++)
        {
            if (NavMesh.SamplePosition(spawnPosition, out hit, maxDistance, NavMesh.AllAreas))
            {
                spawnPosition = hit.position;
                break;
            }
            else
            {
                spawnPosition = randomPointDelegate();
                maxDistance *= 2.0f;
            }
        }
        obj.transform.position = spawnPosition;
        obj.SetActive(true);

        SoundManager.Instance.PlaySound("EnemySpawn", SoundType.Effect, obj.transform);

        GameObject effect = Factory.Instance.GetEffect(effectDataName);
        effect.transform.position = spawnPosition;
        Factory.Instance.ReturnEffectDelay(effect, 1.0f);

        currentCount++;
        return obj;
    }

    private void OnEnemyDead(int id)
    {
        if (id < 4)
            currentCount--;
    }

    private Vector3 RandomSpawnPoint()
    {
        int spawnPointIndex = UnityEngine.Random.Range(0, spawnPoints.Length);
        GameObject obj = spawnPoints[spawnPointIndex];
        Vector2 randomPoint = UnityEngine.Random.insideUnitCircle * 30.0f;
        Vector3 resultPoint = obj.transform.position + new Vector3(randomPoint.x, 0.0f, randomPoint.y);

        return resultPoint;
    }

    public void SetRandomPointDelegate(RandomPointDelegate randomPointDelegate)
    {
        this.randomPointDelegate = randomPointDelegate;
    }

    private IEnumerator RespawnEnemy()
    {
        while(true)
        {
            yield return new WaitForSeconds(5.0f);
            if (currentCount < maxCount)
            {
                SetRandomPointDelegate(RandomSpawnPoint);
                SpawnEnemyWithEffect("MagicCircle2SpawnData");
            }
        }
    }
    private void OnDisable()
    {
        Factory.Instance.OnEnemyDead -= OnEnemyDead;
    }
}