using System;
using System.Collections;
using UnityEngine;

public class GhostEffectComponent : MonoBehaviour
{
    public String ghostSpawnDataName; // 잔상으로 사용될 스폰 데이터 네임
    public float spawnInterval = 0.1f; // 잔상 생성 간격
    public float ghostLifeTime = 0.3f; // 잔상 지속 시간

    private SkinnedMeshRenderer skinnedMeshRenderer;
    private Mesh bakedMesh;

    [SerializeField]
    private Material material;

    private bool bPlay = false;
    public bool Play { get => bPlay; set => bPlay = value; }

    private void Start()
    {
        skinnedMeshRenderer = GetComponentInChildren<SkinnedMeshRenderer>();
        bakedMesh = new Mesh();
        StartCoroutine(SpawnGhosts());
    }


    public IEnumerator SpawnGhosts()
    {
        while (bPlay)
        {
            SpawnGhost();
            yield return new WaitForSeconds(spawnInterval);
        }
    }

    private void SpawnGhost()
    {
        skinnedMeshRenderer.BakeMesh(bakedMesh); // 스킨드 메시를 베이크하여 정적 메시로 변환

        GameObject ghost = Factory.Instance.GetEffect(ghostSpawnDataName);
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;

        MeshFilter ghostMeshFilter = ghost.GetComponent<MeshFilter>();
        MeshRenderer ghostMeshRenderer = ghost.GetComponent<MeshRenderer>();

        if (ghostMeshFilter != null)
        {
            ghostMeshFilter.sharedMesh = Instantiate(bakedMesh); // 베이크된 메시를 잔상에 할당
        }

        if (ghostMeshRenderer != null)
        {
            ghostMeshRenderer.material = material; // 원래 캐릭터의 머티리얼을 잔상에 할당
            
        }

        StartCoroutine(FadeAndDestroy(ghost, ghostLifeTime));
    }

    private IEnumerator FadeAndDestroy(GameObject ghost, float duration)
    {
        MeshRenderer ghostRenderer = ghost.GetComponent<MeshRenderer>();
        Color initialColor = ghostRenderer.material.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            Color newColor = initialColor;
            newColor.a = Mathf.Lerp(initialColor.a, 0, elapsed / duration);
            ghostRenderer.material.color = newColor;
            yield return new WaitForFixedUpdate();
            
        }
        Factory.Instance.ReturnEffectDelay(ghost);
    }
}