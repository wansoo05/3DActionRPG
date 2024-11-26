using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Summon : Skill
{
    [SerializeField]
    private float radius = 3.0f;

    [SerializeField]
    private int summonCount = 3;

    [SerializeField]
    private float lifeSpan = 30.0f;

    private GameObject[] summonCharacters;

    protected override void Reset()
    {
        base.Reset();
        type = SkillType.Summon;
    }

    protected override void Awake()
    {
        base.Awake();

        summonCharacters = new GameObject[summonCount];
    }

    public override void Begin_DoAction()
    {
        base.Begin_DoAction();

        StartCoroutine(SpawnHelp());
    }

    public override void Play_Particle()
    {
        base.Play_Particle();

        Vector3 particlePosition = rootObject.transform.position + rootObject.transform.up + rootObject.transform.forward * 1.0f;
        Instantiate(doActionData.hittingDatas[0].Particle, particlePosition, rootObject.transform.rotation);

        string name = "Summon_Effect";
        SoundManager.Instance.PlaySound(name, SoundType.Effect, rootObject.transform);
    }

    private Vector3 RandomPoint()
    {
        Vector3 origin = rootObject.transform.position;
        Vector2 randomPointXZ = Random.insideUnitCircle * radius;
        Vector3 randomPoint = origin + new Vector3(randomPointXZ.x, 0.0f, randomPointXZ.y);

        return randomPoint;
    }

    private IEnumerator SpawnHelp()
    {
        for (int i = 0; i < summonCount; ++i)
        {
            summonCharacters[i] = Factory.Instance.GetCharacter("TeamSpawnData");

            Vector3 spawnPosition = RandomPoint();
            NavMeshHit hit = new NavMeshHit();
            float maxDistance = 1.0f;
            for (int j = 0; j < 10; j++)
            {
                if (NavMesh.SamplePosition(spawnPosition, out hit, maxDistance, NavMesh.AllAreas))
                {
                    spawnPosition = hit.position;
                    break;
                }
                else
                {
                    spawnPosition = RandomPoint();
                    maxDistance *= 2.0f;
                }
            }

            summonCharacters[i].transform.position = spawnPosition;
            summonCharacters[i].SetActive(true);

            Factory.Instance.ReturnCharacterDelay(summonCharacters[i], lifeSpan);
            yield return new WaitForSeconds(0.3f);
        }
    }
}