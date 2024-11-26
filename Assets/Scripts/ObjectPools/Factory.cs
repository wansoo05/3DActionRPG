using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Factory : MonoBehaviour
{
    public static Factory Instance { get; private set; }

    private CharacterPool character;
    private EffectPool effect;
    private ProjectilePool projectile;

    public event Action<int> OnEnemyDead;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (mode != LoadSceneMode.Additive)
        {
            OnInitialize();
        }
    }

    private void OnInitialize()
    {
        character = GetComponentInChildren<CharacterPool>();
        if (character != null)
            character.Initialize();

        effect = GetComponentInChildren<EffectPool>();
        if (effect != null)
            effect.Initialize();

        projectile = GetComponentInChildren<ProjectilePool>();
        if (projectile != null)
            projectile.Initialize();

    }

    public GameObject GetCharacter(string spawnDataName)
    {
        return character.GetObject(spawnDataName);
    }

    public GameObject GetEffect(string spawnDataName)
    {
        return effect.GetObject(spawnDataName);
    }

    public GameObject GetProjectile(string spawnDataName)
    {
        return projectile.GetObject(spawnDataName);
    }

    public void ReturnCharacterDelay(GameObject obj, float time = 0)
    {
        StartCoroutine(ReturnCharacterWaitForTime(obj, time));
    }

    public void ReturnEffectDelay(GameObject obj, float time = 0)
    {
        StartCoroutine(ReturnEffectWaitForTime(obj, time));
    }

    public void ReturnProjectileDelay(GameObject obj, float time = 0)
    {
        StartCoroutine(ReturnProjectileWaitForTime(obj, time));
    }

    private IEnumerator ReturnCharacterWaitForTime(GameObject obj, float time = 0)
    {
        yield return new WaitForSeconds(time);

        character.ReturnObject(obj);
        Enemy enemy = obj.GetComponent<Enemy>();
        if (enemy != null)
            OnEnemyDead?.Invoke(enemy.id);
    }

    private IEnumerator ReturnEffectWaitForTime(GameObject obj, float time = 0)
    {
        yield return new WaitForSeconds(time);

        effect.ReturnObject(obj);
    }

    private IEnumerator ReturnProjectileWaitForTime(GameObject obj, float time = 0)
    {
        yield return new WaitForSeconds(time);

        projectile.ReturnObject(obj);
    }
}