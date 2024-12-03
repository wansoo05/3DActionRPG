using UnityEngine;
using UnityEngine.AI;

public class AIController_Range : AIController
{
    [SerializeField]
    private float retreatDistance = 5.0f;

    [SerializeField]
    private Vector2 backDistance = new Vector2(5, 10);

    [SerializeField]
    private float backAngleRange = 30.0f;

    [SerializeField]
    private GameObject portalEffectPrefab;

    private float currentCoolTime = 0.0f;

    private Vector3 warpPosition;

    protected override bool CheckMode()
    {
        bool bCheck = false;

        bCheck |= (GetState() == State.Wait);
        bCheck |= (GetState() == State.Patrol);
        bCheck |= (GetState() == State.Approach);

        return bCheck;
    }

    protected override void Update_State()
    {
        if (CheckCoolTime())
            return;

        if (!CheckMode())
            return;

        GameObject player = perception.GetPercievedTarget();

        //player 감지가 안되었다면
        if (player == null)
        {
            //무기 장착이 되어있다면 장착 해제
            if (weapon.UnarmedMode == false)
                weapon.SetUnarmedMode();

            SetState(State.Patrol);
            return;
        }
        //player가 감지가 됐다면

        //무기 장착을 안하고 있다면
        if (weapon.UnarmedMode)
        {
            SetState(State.Equip);
            return;
        }

        //player를 볼 수 없다면
        if (CanSeePlayer(player) == false)
        {
            SetState(State.Approach);
            return;
        }

        //player와 거리가 너무 가깝다면
        if (Vector3.Distance(player.transform.position, transform.position) < retreatDistance)
        {
            SetWarpPosition(player);
            SetState(State.Retreat);
            return;
        }

        SetState(State.Action);
    }

    private bool CheckCoolTime()
    {
        if (currentCoolTime <= 0.0f)
        {
            currentCoolTime = 0.0f;
            return false;
        }

        currentCoolTime -= Time.deltaTime;
        return true;
    }

    private bool CanSeePlayer(GameObject player)
    {
        RaycastHit hit;
        Vector3 position = transform.position + Vector3.up;
        Vector3 direction = player.transform.position - transform.position;
        if (Physics.Raycast(position, direction, out hit))
        {
            if (hit.transform.gameObject == player)
                return true;
        }

        return false;
    }

    protected override void OnEndDoAction()
    {
        base.OnEndDoAction();
        SetState(State.Wait);
        currentCoolTime += attackDelay + Random.Range(-attackDelayRandom, +attackDelayRandom);
    }

    private void DoWarp()
    {
        Vector3 position = transform.position + Vector3.up;
        Instantiate(portalEffectPrefab, position, Quaternion.identity);
        Vector3 direction = transform.position - warpPosition;
        transform.position = warpPosition;
        transform.rotation = Quaternion.LookRotation(direction);
    }

    void SetWarpPosition(GameObject player)
    {
        // 1. 랜덤 각도 계산 (도 단위)
        float randomAngle = Random.Range(-backAngleRange, +backAngleRange);

        // 2. 플레이어의 뒤 방향 벡터 (-playerForward)를 랜덤 각도만큼 회전
        Vector3 direction = Quaternion.Euler(0, randomAngle, 0) * -player.transform.forward;
        float distance = Random.Range(backDistance.x, backDistance.y);

        warpPosition = player.transform.position + direction.normalized * distance;

        NavMeshHit hit;

        if (NavMesh.SamplePosition(warpPosition, out hit, 1.0f, NavMesh.AllAreas))
            warpPosition = hit.position;
    }

    public void End_DoWarp()
    {
        Debug.Log($"{transform.name} : End_DoWarp");
        SetState(State.Wait);
        currentCoolTime += attackDelay + Random.Range(-attackDelayRandom, +attackDelayRandom);
    }

    private void OnDrawGizmos()
    {
        Vector3 position = transform.position;
        Gizmos.DrawWireSphere(position, 5.0f);
    }
}