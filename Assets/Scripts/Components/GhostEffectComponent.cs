using System;
using System.Collections;
using UnityEngine;

public class GhostEffectComponent : MonoBehaviour
{
    public String ghostSpawnDataName; // �ܻ����� ���� ���� ������ ����
    public float spawnInterval = 0.1f; // �ܻ� ���� ����
    public float ghostLifeTime = 0.3f; // �ܻ� ���� �ð�

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
        skinnedMeshRenderer.BakeMesh(bakedMesh); // ��Ų�� �޽ø� ����ũ�Ͽ� ���� �޽÷� ��ȯ

        GameObject ghost = Factory.Instance.GetEffect(ghostSpawnDataName);
        ghost.transform.position = transform.position;
        ghost.transform.rotation = transform.rotation;

        MeshFilter ghostMeshFilter = ghost.GetComponent<MeshFilter>();
        MeshRenderer ghostMeshRenderer = ghost.GetComponent<MeshRenderer>();

        if (ghostMeshFilter != null)
        {
            ghostMeshFilter.sharedMesh = Instantiate(bakedMesh); // ����ũ�� �޽ø� �ܻ� �Ҵ�
        }

        if (ghostMeshRenderer != null)
        {
            ghostMeshRenderer.material = material; // ���� ĳ������ ��Ƽ������ �ܻ� �Ҵ�
            
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