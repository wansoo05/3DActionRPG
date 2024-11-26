using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.AI;

public class PatrolComponent : MonoBehaviour
{
    [SerializeField]
    private float waitDelay = 2.0f;
    public float WaitDelay { set { waitDelay = value; } }

    [SerializeField]
    private float waitDelayRandom = 0.5f;

    public float WaitDelayRandom { set { waitDelayRandom = value; } }

    private NavMeshAgent navMeshAgent;
    private NavMeshPath navMeshPath = null;

    private Vector3 destination;
    private Vector3 initPosition;

    private bool bArrived;
    private bool bFindPath = true;
    private Coroutine waitCoroutine;

    public event Func<Vector3> OnSetDestination;

    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        initPosition = destination = transform.position;
        bArrived = false;
    }

    private void Update()
    {
        if (navMeshPath == null)
            return;
        
        if (bArrived == true)
            return;

        if (Vector3.Distance(transform.position, destination) >= navMeshAgent.stoppingDistance)
            return;

        print($"{transform.name} : ¸ñÀûÁö µµÂø");
        bArrived = true;
        float waitTime = waitDelay + UnityEngine.Random.Range(-waitDelayRandom, +waitDelayRandom);

        waitCoroutine = StartCoroutine(WaitTime(waitTime));
    }

    public void StartMove()
    {
        bFindPath = true;
        bArrived = false;
        navMeshPath = CreateNavMeshPath();
        navMeshAgent.SetPath(navMeshPath);
    }

    public void StopMove()
    {
        bFindPath = false;
        navMeshAgent.ResetPath();
        if (waitCoroutine != null)
            StopCoroutine(waitCoroutine);
    }

    public NavMeshPath CreateNavMeshPath()
    {
        NavMeshPath path = null;

        while (true)
        {
            if (bFindPath == false)
                break;
            path = new NavMeshPath();

            Debug.Assert(OnSetDestination != null);
            destination = OnSetDestination.Invoke();


            NavMeshHit hit;
            if (NavMesh.SamplePosition(destination, out hit, 1.0f, 1))
            {
                destination = hit.position;

                if (navMeshAgent.CalculatePath(destination, path) && path.status == NavMeshPathStatus.PathComplete)
                {
                    return path;
                }
            }
        }
        return path;
    }

    private IEnumerator WaitTime(float time)
    {
        yield return new WaitForSeconds(time);

        print($"{transform.name} : ¼øÂû ´ë±â");
        navMeshPath = CreateNavMeshPath();
        if (navMeshPath != null)
        {
            navMeshAgent.SetPath(navMeshPath);
            bArrived = false;
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        if (Application.isPlaying == false)
            return;

        Vector3 from = transform.position + new Vector3(0.0f, 0.1f, 0.0f);
        Vector3 to = destination + new Vector3(0.0f, 0.1f, 0.0f);

        Gizmos.color = Color.red;
        Gizmos.DrawLine(from, to);

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(destination, 0.5f);
    }
#endif
}