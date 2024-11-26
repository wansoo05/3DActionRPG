using UnityEngine;
using UnityEngine.AI;

public class Test_CircleMovement : MonoBehaviour
{
    [SerializeField]
    float speed = 1.5f;

    private GameObject player;
    float currentRadius;
    Vector3 targetPosition;
    private Animator animator;
    private NavMeshAgent navMeshAgent;
    private NavMeshPath path;

    private void Awake()
    {
        player = GameObject.Find("Player");
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
    }
    private void Start()
    {
        currentRadius = Random.Range(3.5f, 5.0f);
        path = CreateNavMeshPath();
        navMeshAgent.SetPath(path);
    }

    private void Update()
    {
        //if (Mathf.Abs(Vector3.Distance(transform.position, player.transform.position) - currentRadius) > 0.2f)
        //{
        //    transform.position = Vector3.MoveTowards(transform.position, targetPosition, Time.deltaTime * 1.5f);
        //}
        //else
        //{
        //    transform.Translate(Vector3.right * speed * Time.deltaTime);
        //}
        transform.LookAt(player.transform);

        if (Vector3.Distance(transform.position, targetPosition) < navMeshAgent.stoppingDistance)
        {
            path = CreateNavMeshPath();
            print(path.status);
            navMeshAgent.SetPath(path);
        }
    }


    private void GetPoint()
    {


        // 플레이어 주변의 랜덤한 점 선택
        float angle = Random.Range(135, 45) * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle) * currentRadius;
        float z = Mathf.Sin(angle) * currentRadius;

        Vector3 direction = transform.position - player.transform.position;
        float rotationAngle = Quaternion.FromToRotation(Vector3.forward, direction).eulerAngles.y;
        Vector3 goalPosition = Quaternion.AngleAxis(rotationAngle, Vector3.up) * new Vector3(x, 0.0f, z);

        targetPosition = player.transform.position + goalPosition;

        Vector3 direction2 = targetPosition - transform.position;
        Vector3 v = Vector3.Cross(transform.forward, direction2.normalized);
        if (Vector3.Dot(Vector3.up, v) > 0.0f)
        {
            animator.SetFloat("SpeedX", 1.5f);
            speed = 1.0f;
        }
        else
        {
            animator.SetFloat("SpeedX", -1.5f);
            speed = -1.0f;        }

    }
    private NavMeshPath CreateNavMeshPath()
    {
        NavMeshPath path = null;

        while (true)
        {
            path = new NavMeshPath();

            GetPoint();
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition,out hit, 1.0f, NavMesh.AllAreas))
            {
                targetPosition = hit.position;
                if (navMeshAgent.CalculatePath(targetPosition, path) && path.status == NavMeshPathStatus.PathComplete)
                    return path;
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;

        // 목적지 랜더링
        Vector3 from = transform.position + new Vector3(0.0f, 0.1f, 0.0f);
        Vector3 to = targetPosition + new Vector3(0.0f, 0.1f, 0.0f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(from, to);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(targetPosition, 0.5f);

        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(targetPosition, 0.25f);

        //각도 랜더링
        float angle = 45.0f * Mathf.Deg2Rad;
        float x = Mathf.Cos(angle) * currentRadius;
        float z = Mathf.Sin(angle) * currentRadius;
        Vector3 direction = transform.position - player.transform.position;
        float rotationAngle = Quaternion.FromToRotation(Vector3.forward, direction).eulerAngles.y;
        Vector3 goalPosition = player.transform.position + Quaternion.AngleAxis(rotationAngle, Vector3.up) * new Vector3(x, 0.0f, z);
        Gizmos.DrawLine(player.transform.position, goalPosition);

        angle = 135.0f * Mathf.Deg2Rad;
        x = Mathf.Cos(angle) * currentRadius;
        z = Mathf.Sin(angle) * currentRadius;
        direction = transform.position - player.transform.position;
        rotationAngle = Quaternion.FromToRotation(Vector3.forward, direction).eulerAngles.y;
        goalPosition = player.transform.position + Quaternion.AngleAxis(rotationAngle, Vector3.up) * new Vector3(x, 0.0f, z);
        Gizmos.DrawLine(player.transform.position, goalPosition);
    }
#endif
}