using Unity.VisualScripting;
using UnityEngine;

public class EnemyBlockComponent : CharacterBlockComponent
{
    private Vector3 prevPosition;
    private Vector3 currPosition;
    private Rigidbody rb;

    protected override void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }
    private void Start()
    {
        prevPosition = transform.position;
    }

    protected override void Update()
    {
        Update_CanBlock();

        if (CanBlock == false)
            return;

        currPosition = transform.position;
        Vector3 direction = currPosition - prevPosition;
        Ray ray = new Ray(transform.position + Vector3.up, direction.normalized);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, offset, blockLayers))
        {
            if (!hit.transform.CompareTag(transform.tag))
            {
                transform.position -= direction;
            }

            if (hit.transform.gameObject.layer == 8)
            {
                transform.position -= direction * 2.0f;
            }
        }

        prevPosition = currPosition;
    }

    protected override void Update_CanBlock()
    {
        CanBlock = true;
    }

}