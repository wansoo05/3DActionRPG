using UnityEngine;

public class WorldCursor : MonoBehaviour
{
    private LayerMask mask = 1 << 7;
    private GameObject player;

    public LayerMask Mask { set => mask = value; }

    private void Awake()
    {
        player = GameManager.Instance.PlayerInstance.gameObject;
        transform.SetParent(player.transform, false);
    }

    private void Update()
    {
        Vector3 position;
        Vector3 normal;

        RaycastHit hit;
        Vector3 start = player.transform.position + player.transform.up;
        Vector3 end = player.transform.position + player.transform.forward * 6.0f;
        if (!Physics.Raycast(start, end - start, out hit, 10.0f, mask))
            return;

        position = hit.point;
        normal = hit.normal;
        Debug.DrawLine(start, end);
        position += normal * 0.05f;
        transform.position = position;

        Vector3 up = Quaternion.Euler(-90.0f, 0.0f, 0.0f) * Vector3.up;
        Quaternion rotation = Quaternion.FromToRotation(up, normal);
        transform.rotation = rotation;
    }
}