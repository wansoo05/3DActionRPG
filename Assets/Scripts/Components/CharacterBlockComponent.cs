
using UnityEngine;

public class CharacterBlockComponent : MonoBehaviour
{
    [SerializeField]
    protected float offset = 0.1f;

    [SerializeField]
    protected LayerMask blockLayers;

    public bool CanBlock;

    private CharacterController controller;
    private StateComponent state;

    protected virtual void Awake()
    {
        controller = GetComponent<CharacterController>();
        state = GetComponent<StateComponent>();
    }

    protected virtual void Update()
    {
        Update_CanBlock();
        if (CanBlock == false)
            return;

        Update_Blocking();
    }

    public void Update_Blocking()
    {
        if (controller.enabled == false) return;

        Vector3 direction = new Vector3(controller.velocity.x, 0.0f, controller.velocity.z);
        Ray ray = new Ray(transform.position + Vector3.up, direction.normalized);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, offset, blockLayers))
        {
            if (!hit.transform.CompareTag(transform.tag))
            {
                controller.Move(-direction * Time.deltaTime);
            }
        }
    }

    protected virtual void Update_CanBlock()
    {
        if (state.FallingMode || state.LandMode || state.EvadeMode)
            CanBlock = false;

        if (state.IdleMode)
            CanBlock = true;
    }
}