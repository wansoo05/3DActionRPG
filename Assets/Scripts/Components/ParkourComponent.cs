using Unity.VisualScripting;
using UnityEngine;

public class ParkourComponent : MonoBehaviour
{
    [SerializeField]
    private float enableHeight = 1.0f;

    [SerializeField]
    private LayerMask layers;

    [SerializeField]
    private float climbSpeed = 1.5f;

    [SerializeField]
    private float climbOffset = 0.2f;

    private Animator animator;
    private CharacterController controller;
    private StateComponent state;
    private PlayerMovingComponent moving;

    private RaycastHit rayHitInfo;
    private bool isClimb = false;
    private float buildingHeight;
    public bool IsClimb { get => isClimb; }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        controller = GetComponent<CharacterController>();
        state = GetComponent<StateComponent>();
        moving = GetComponent<PlayerMovingComponent>();
    }

    private void Update()
    {
        Update_Climb();
    }

    public void Update_Climb()
    {
        if (state.ClimbMode == false)
            return;

        if (transform.position.y <= buildingHeight + climbOffset)
        {
            controller.Move(transform.up * climbSpeed * Time.deltaTime);
            return;
        }

        controller.Move(transform.forward * climbSpeed * Time.deltaTime);
    }

    public void TryClimb()
    {
        if (CanClimb() == false)
            return;
        
        state.SetClimbMode();
        buildingHeight = GetBuildingHeight(rayHitInfo.transform);
        float heightDiff = (rayHitInfo.transform.position.y - buildingHeight / 2) + buildingHeight - transform.position.y;
        float normalizedTime = 0.8f - heightDiff / enableHeight;
        normalizedTime = Mathf.Clamp(normalizedTime, 0.0f, 1.0f);
        animator.Play("WallClimb", 6, normalizedTime);
        moving.Stop();
    }

    private bool CanClimb()
    {
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out rayHitInfo, 1.0f, layers))
        {
            if (rayHitInfo.transform.tag == "Rock")
            {
                float buildingHeight = GetBuildingHeight(rayHitInfo.transform);
                return (rayHitInfo.transform.position.y - buildingHeight / 2) + buildingHeight - transform.position.y <= enableHeight;
            }
        }

        return false;
    }

    private float GetBuildingHeight(Transform buildingTransform)
    {
        Collider collider = buildingTransform.GetComponent<Collider>();
        float height = collider.bounds.size.y;

        return height;
    }

    private void End_Climb()
    {
        state.SetIdleMode();
        animator.SetBool("IsClimb", false);
        moving.Move();
    }
}