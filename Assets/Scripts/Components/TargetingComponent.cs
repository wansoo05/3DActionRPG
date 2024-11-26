using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;

public class TargetingComponent : MonoBehaviour
{
    [SerializeField]
    private float angle = 45.0f;

    [SerializeField]
    private float radius = 10.0f;

    [SerializeField]
    private LayerMask layerMask;

    [SerializeField]
    private string targetMarkName;

    [SerializeField]
    private bool useTargeting = false;

    private PlayerMovingComponent moving;
    private WeaponComponent weapon;
    private StateComponent state;

    private Collider[] colliders;
    private GameObject originTarget = null;
    private InputActions input;
    
    private void Awake()
    {
        moving = GetComponent<PlayerMovingComponent>();
        weapon = GetComponent<WeaponComponent>();
        state = GetComponent<StateComponent>();
    }

    private void OnEnable()
    {

    }
    private void OnDisable()
    {
        input.Player.Targeting.started -= Input_Targeting;
        input.Player.Disable();
    }
    private void Start()
    {
        input = GameManager.Instance.input;

        input.Player.Enable();
        input.Player.Targeting.started += Input_Targeting;
    }
    private void Input_Targeting(InputAction.CallbackContext context)
    {
        if (weapon.UnarmedMode)
            return;

        if (useTargeting)
        {
            useTargeting = false;
            UIController.Instance.UpdateTargetingText("Targeting : Off");
            TargetMarkOnOff(false, originTarget);
        }
        else
        {
            useTargeting = true;
            UIController.Instance.UpdateTargetingText("Targeting : On");
        }
    }

    private void Update()
    {
        if (weapon.UnarmedMode)
            return;

        if (useTargeting == false)
            return;

        Targeting();

        if (originTarget == null)
            return;

        if (state.ActionMode)
            return;

        //Ÿ���� ���ؼ� �÷��̾� ȸ��
        Vector3 direction = originTarget.transform.position - transform.position;
        direction.y = 0.0f;
        transform.rotation = Quaternion.LookRotation(direction);
    }


    /// <summary>
    /// �÷��̾� �����ȿ� �� Ÿ�� �ĺ��� ����
    /// </summary>
    private void Targeting()
    {
        //���� ���� ���� Ÿ�� ������ ���� �ʴ´�.
        if (state.ActionMode)
            return;

        List<GameObject> condidates = new List<GameObject>();

        if (weapon.WandMode)
            radius = 10.0f;
        else
            radius = 5.0f;

        colliders = Physics.OverlapSphere(transform.position, radius, layerMask);
        foreach(Collider collider in colliders)
        {
            if (collider.transform.Equals(transform))
                continue;
            if (collider.transform.GetComponent<ITargetable>() == null)
                continue;
            Vector3 direction = collider.transform.position - transform.position;
            if (Vector3.Dot(transform.forward, direction.normalized) >= Mathf.Cos(angle))
                condidates.Add(collider.gameObject);
        }

        GameObject newTarget = GetNearlyTarget(condidates);
        if (originTarget == newTarget)
            return;

        TargetMarkOnOff(false, originTarget);
        TargetMarkOnOff(true, newTarget);

        originTarget = newTarget;
    }

    /// <summary>
    /// �÷��̾� ������ �¿� 45�� �����ȿ� �� �ĺ��� �߿��� ���� ���濡�� ����� �ĺ��� Ÿ������ ����
    /// </summary>
    /// <param name="targets">Ÿ�� �ĺ���</param>
    /// <returns></returns>
    private GameObject GetNearlyTarget(List<GameObject> condidates)
    {
        GameObject newTarget = null;
        Dictionary<float, GameObject> dic = new Dictionary<float, GameObject>();
        foreach (GameObject obj in condidates)
        {
            dic.TryAdd(Vector3.Distance(transform.position, obj.transform.position), obj);
        }

        //���� 90�� �� �ȿ� ���� ���� ���
        if (dic.Count <= 0)
            return null;

        var priority = from d in dic
                       orderby d.Key ascending
                       select d;

        if (priority != null)
            newTarget = priority.First().Value;

        return newTarget;
    }

    /// <summary>
    /// target�� TargetMarkUI�� Ű�ų� ���� �Լ�
    /// </summary>
    /// <param name="on">on�� true�̸� �ѱ�, false�̸� ����</param>
    /// <param name="target">���</param>
    private void TargetMarkOnOff(bool on, GameObject target)
    {
        if (target == null) return;

        ITargetable targetable = target.GetComponent<ITargetable>();
        if (targetable == null) return;

        if (on)
            targetable.OnSelected();
        else
            targetable.OnEliminated();
    }
}