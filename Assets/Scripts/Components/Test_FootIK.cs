using UnityEngine;

public class Test_FootIK : MonoBehaviour
{
    [SerializeField]
    private float footRayHeightOffset = 0.5f;

    [SerializeField, Range(0.5f, 1.0f)]
    private float rayMaxdistance = 0.7f;

    [SerializeField, Range(0.0f, 0.3f)]
    private float footIKOffset = 0.15f;

    [SerializeField]
    private LayerMask groundLayer;

    [SerializeField]
    private float leftFootWeight = 1.0f;
    [SerializeField]
    private float rightFootWeight = 1.0f;

    [SerializeField]
    private bool enableIKPositioning = true;

    [SerializeField]
    private bool enableIKRotating = true;

    private Animator animator;

    private Vector3 leftFootRayPosition = new Vector3();
    private Vector3 rightFootRayPosition = new Vector3();
    private RaycastHit leftFootRayHitInfo = new RaycastHit();
    private RaycastHit rightFootRayHitInfo = new RaycastHit();

    private Vector3 leftFootIKTargetPosition = new Vector3();
    private Vector3 rightFootIKTargetPosition = new Vector3();
    private Vector3 leftFootIKPositionBuffer = new Vector3();
    private Vector3 rightFootIKPositionBuffer = new Vector3();
    private Vector3 leftFootTargetRotation;
    private Vector3 rightFootTargetRotation;
    private Quaternion leftFootIKRotationBuffer;
    private Quaternion rightFootIKRotationBuffer;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        if (groundLayer.value == 0)
            groundLayer = 1 << 7;
    }

    private void Update()
    {
        Update_IKSetting();
    }

    private void OnAnimatorIK(int layerIndex)
    {
        IKPositioning();
        IKRotating();
        ApplyFootIK();
    }

    private void Update_IKSetting()
    {
        Transform leftFootTransform = animator.GetBoneTransform(HumanBodyBones.LeftFoot);
        Transform rightFootTransform = animator.GetBoneTransform(HumanBodyBones.RightFoot);
        
        leftFootRayPosition = leftFootTransform.position;
        rightFootRayPosition = rightFootTransform.position;
        leftFootRayPosition.y = transform.position.y + footRayHeightOffset;
        rightFootRayPosition.y = transform.position.y + footRayHeightOffset;

        //LeftFoot RayCast
        Ray leftFootRay = new Ray(leftFootRayPosition, Vector3.down);
        if (Physics.Raycast(leftFootRay, out leftFootRayHitInfo, rayMaxdistance, groundLayer))
        {
            leftFootIKTargetPosition = leftFootRayHitInfo.point;
            leftFootIKTargetPosition.y += footIKOffset;

            leftFootTargetRotation = leftFootRayHitInfo.normal;
        }
        else
        {
            leftFootIKTargetPosition.y = transform.position.y;
            leftFootTargetRotation = transform.up;
        }

        //RightFoot RayCast
        Ray rightFootRay = new Ray(rightFootRayPosition, Vector3.down);
        if (Physics.Raycast(rightFootRay, out rightFootRayHitInfo, rayMaxdistance, groundLayer))
        {
            rightFootIKTargetPosition = rightFootRayHitInfo.point;
            rightFootIKTargetPosition.y += footIKOffset;

            rightFootTargetRotation = rightFootRayHitInfo.normal;
        }
        else
        {
            rightFootIKTargetPosition.y = transform.position.y;
            rightFootTargetRotation = transform.up;
        }
    }

    private void IKPositioning()
    {
        leftFootIKPositionBuffer = animator.GetIKPosition(AvatarIKGoal.LeftFoot);
        rightFootIKPositionBuffer = animator.GetIKPosition(AvatarIKGoal.RightFoot);
        if (animator.GetIKPosition(AvatarIKGoal.LeftFoot).y >= leftFootIKTargetPosition.y)
        {
            leftFootIKPositionBuffer.y = animator.GetIKPosition(AvatarIKGoal.LeftFoot).y;
        }
        else
        {
            leftFootIKPositionBuffer.y = leftFootIKTargetPosition.y;
        }

        if (animator.GetIKPosition(AvatarIKGoal.RightFoot).y >= rightFootIKTargetPosition.y)
        {
            rightFootIKPositionBuffer.y = animator.GetIKPosition(AvatarIKGoal.RightFoot).y;
        }
        else
        {
            rightFootIKPositionBuffer.y = rightFootIKTargetPosition.y;
        }
    }

    private void IKRotating()
    {
        leftFootIKRotationBuffer = Quaternion.FromToRotation(transform.up, leftFootTargetRotation) * animator.GetIKRotation(AvatarIKGoal.LeftFoot);
        rightFootIKRotationBuffer = Quaternion.FromToRotation(transform.up, rightFootTargetRotation) * animator.GetIKRotation(AvatarIKGoal.RightFoot);
    }

    private void ApplyFootIK()
    {
        animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKPositionWeight(AvatarIKGoal.RightFoot, rightFootWeight);

        animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, leftFootWeight);
        animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, rightFootWeight);

        if (enableIKPositioning)
        {
            animator.SetIKPosition(AvatarIKGoal.LeftFoot, leftFootIKPositionBuffer);
            animator.SetIKPosition(AvatarIKGoal.RightFoot, rightFootIKPositionBuffer);
        }

        if (enableIKRotating)
        {
            animator.SetIKRotation(AvatarIKGoal.LeftFoot, leftFootIKRotationBuffer);
            animator.SetIKRotation(AvatarIKGoal.RightFoot, rightFootIKRotationBuffer);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawRay(leftFootRayPosition, Vector3.down * rayMaxdistance);
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(rightFootRayPosition, Vector3.down * rayMaxdistance);
    }
}