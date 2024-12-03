using System.Collections.Generic;
using UnityEngine;

public class PerceptionComponent : MonoBehaviour
{
    [SerializeField]
    private float distance = 8.0f;

    [SerializeField]
    private float angle = 45.0f;

    [SerializeField]
    private float lostTime = 2.0f;

    [SerializeField]
    private LayerMask layerMask;

    private Dictionary<GameObject, float> percievedTable;

    private void Reset()
    {
        layerMask = 1 << LayerMask.NameToLayer("Character");
    }

    private void Awake()
    {
        percievedTable = new Dictionary<GameObject, float>();
    }

    private void Update()
    {
        Collider[] colliders = Physics.OverlapSphere(transform.position, distance, layerMask);

        Vector3 forward = transform.forward;
        List<Collider> candidateList = new List<Collider>();

        foreach(Collider collider in colliders)
        {
            //감지된 캐릭터와의 방향을 얻기위한 변수
            Vector3 direction = collider.transform.position - transform.position;
            //감지된 캐릭터로부터 자신과 같은 높이에서 방향을 얻기위한 변수
            Vector3 direction2 = direction;
            direction2.y = 0.0f;
            float horizontalAngle = Vector3.Angle(transform.forward, direction2);
            float heightDiff = collider.transform.position.y - transform.position.y;
            heightDiff = Mathf.Abs(heightDiff);
            if (horizontalAngle <= angle && heightDiff <= 1.0f)
                candidateList.Add(collider);
        }

        foreach(Collider collider in candidateList)
        {
            
            if (percievedTable.ContainsKey(collider.gameObject) == false)
            {
                percievedTable.Add(collider.gameObject, Time.realtimeSinceStartup);
                continue;
            }

            percievedTable[collider.gameObject] = Time.realtimeSinceStartup;
        }

        List<GameObject> removeList = new List<GameObject>();

        foreach(var item in percievedTable)
        {
            if ((Time.realtimeSinceStartup - item.Value) >= lostTime)
                removeList.Add(item.Key);
        }

        removeList.RemoveAll(remove => percievedTable.Remove(remove));
    }

    public GameObject GetPercievedTarget()
    {
        string compositeTag = "Player";

        if (gameObject.CompareTag("Player"))
        {
            compositeTag = "Enemy";
        }

        foreach (var item in percievedTable)
        {
            if (item.Key == null)
                continue;

            if (item.Key.CompareTag(compositeTag))
                return item.Key;
        }

        return null;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, distance);

        Gizmos.color = Color.blue;

        Vector3 direction = Vector3.zero;
        Vector3 forward = transform.forward;

        direction = Quaternion.AngleAxis(+angle, Vector3.up) * forward;
        Gizmos.DrawLine(transform.position, transform.position + direction.normalized * distance);

        direction = Quaternion.AngleAxis(-angle, Vector3.up) * forward;
        Gizmos.DrawLine(transform.position, transform.position + direction.normalized * distance);

        GameObject player = GetPercievedTarget();
        if (player == null)
            return;

        Gizmos.color = Color.magenta;

        Vector3 position = transform.position;
        position.y += 1.0f;

        Vector3 playerPosition = player.transform.position;
        playerPosition.y += 1.0f;

        Gizmos.DrawLine(position, playerPosition);
        Gizmos.DrawSphere(playerPosition, 0.25f);
    }
#endif
}