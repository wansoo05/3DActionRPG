using UnityEngine;
using UnityEngine.UI;

public class NPC : MonoBehaviour
{
    [SerializeField]
    private int motionType = 0;

    [SerializeField]
    private string perceptionUIName = "NPC_Perception";

    [SerializeField]
    private Vector3 perceptionPositionOffset = new Vector3(1.0f, 1.2f, 0.0f);

    /// <summary>
    /// NPC�� ������ �ִ� �̼ǵ��� ���̵��
    /// </summary>
    [SerializeField]
    private int[] missionIds;

    /// <summary>
    /// NPC �⺻ ���� �޼���
    /// </summary>
    [SerializeField]
    private string defaultMessage = "�ȳ��ϼ���."; //NPC ���� �޼���

    /// <summary>
    /// �̼� ���¸� �˷��� ����Ʈ ��ũ ��������Ʈ��
    /// </summary>
    [SerializeField]
    private Sprite[] markSprites;

    private Animator animator;

    //UI���� ���
    private Canvas perceptionUI;
    private Canvas questMark;
    private Image questImg;

    //���� �������� �̼�����
    private int currentMissionId;


    private void Awake()
    {
        animator = GetComponent<Animator>();

        perceptionUI = UIHelpers.CreateBillBoardCanvas(perceptionUIName, transform, Camera.main);
        perceptionUI.gameObject.SetActive(false);

        questMark = UIHelpers.CreateBillBoardCanvas("QuestMark", transform, Camera.main);
        Transform t = questMark.transform.FindChildByName("Image");
        questImg = t.GetComponent<Image>();
        questImg.sprite = markSprites[0];
    }

    private void Start()
    {
        UIController.Instance.MissionMessage.OnAcceptBt += AcceptButton;
        UIController.Instance.MissionMessage.OnCompleteBt += CompleteButton;
        UIController.Instance.MissionMessage.OnConfirmBt += ConfirmButton;
    }

    private void Update()
    {
        //������ UI ī�޶� �������� ȸ����Ű��
        if (perceptionUI != null)
        {
            perceptionUI.transform.localPosition = perceptionPositionOffset;
            perceptionUI.transform.rotation = Camera.main.transform.rotation;
        }

        if (questMark != null)
        {
            questMark.transform.rotation = Camera.main.transform.rotation;
        }


        //Player ������ ���
        Transform player;
        if (DetectPlayer(out player))
        {
            perceptionUI.gameObject.SetActive(true);

            //�÷��̾ �ٶ󺸰� ȸ��
            Vector3 direction = player.transform.position - transform.position; 
            direction.y = 0.0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction.normalized), 5.0f * Time.deltaTime);

            //'T' Ű�� ������ �̼�UI �����ֱ�
            if (Input.GetKeyDown(KeyCode.T))
            {
                UIController.Instance.ShowMissionUI();
            }
        }
        //�������� ���
        else
        {
            perceptionUI.gameObject.SetActive(false);
        }

        //�̼� ���¿� ���� ���� ���� ������ �̼� ���̵� ã��
        currentMissionId = -1; //���� ���� ������ �̼��� ������ -1
        foreach (int id in missionIds)
        {
            if (MissionController.Instance.IsPossibleMission(id))
            {
                currentMissionId = id;
                break;
            }
        }
    }

    /// <summary>
    /// /// Player�� ���� ���� ���� ������ True ������ False
    /// </summary>
    /// <param name="player">���� �ȿ� �ִ� Player ��ȯ</param>
    /// <returns></returns>
    private bool DetectPlayer(out Transform player)
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + Vector3.up, new Vector3(3.0f, 1.5f, 3.0f), transform.rotation, 1 << 6);

        foreach (Collider collider in colliders)
        {
            //Player�̶��
            if (collider.transform.GetComponent<Player>() != null)
            {
                player = collider.transform;
                return true;
            }
        }
        player = null;
        return false;
    }

    /// <summary>
    /// �̼� ���¿� ���� UI�� ����
    /// </summary>
    private void LateUpdate()
    {
        Update_QuestUIByMission();
        Update_MissionMessage();
    }

    
    private void Update_QuestUIByMission()
    {
        //���� �ִ� �̼��� ������ ����Ʈ ��ũ�� ��Ȱ��ȭ ��Ű�� ����Ʈ ��ũ ������Ʈ�� ���� �ʴ´�.
        if (IsEmptyMission())
        {
            questMark.gameObject.SetActive(false);

            return;
        }

        //���� �̼��� �������̶��
        if (MissionController.Instance.GetMissionState(currentMissionId) == MissionState.Active)
        {
            //�̼��� �Ϸ����� �� ��ũ ������Ʈ
            if (MissionController.Instance.IsMissionComplete(currentMissionId))
            {
                questImg.sprite = markSprites[2];
            }
            //�̼����� �� ��ũ ������Ʈ
            else
            {
                questImg.sprite = markSprites[1];
            }
        }
        else
        {
            questImg.sprite = markSprites[0];
        }
    }

    private void Update_MissionMessage()
    {
        Transform player;
        //�÷��̾ �������� ������ �̼� �޼����� ������Ʈ ��Ű�� �ʴ´�.
        if (DetectPlayer(out player) == false)
            return;

        //���� �ִ� �̼��� �ִٸ�
        if (IsEmptyMission() == false)
            //���� �̼��� �޼����� ������Ʈ��Ų��.
            MissionController.Instance.UpdateMissionMessage(currentMissionId);
        else
            //���� �ִ� �̼��� ������ NPC ���� �޼����� ������Ʈ��Ų��.
            UIController.Instance.UpdateMissionMessage(defaultMessage);
    }

    /// <summary>
    /// ���� �ִ� �̼��� ������ false ������ true
    /// </summary>
    /// <returns></returns>
    private bool IsEmptyMission()
    {
        if (currentMissionId == -1)
            return true;
        return false;
    }

    //�ִϸ��̼� �̺�Ʈ : NPC ��� ���� �����Ͽ� ���� ��� ���
    private void End_Motion()
    {
        motionType = UnityEngine.Random.Range(0, 3);
        animator.SetInteger("MotionType", motionType);
    }

    /// <summary>
    /// �̼� UI�� accept button�� ������ �̺�Ʈ �Լ�
    /// </summary>
    public void AcceptButton()
    {
        MissionController.Instance.AcceptMission(currentMissionId);
    }

    /// <summary>
    /// �̼� UI�� complete button�� ������ �̺�Ʈ �Լ�
    /// </summary>
    private void CompleteButton()
    {
        if (MissionController.Instance.CompleteMission(currentMissionId))
        {
            SoundManager.Instance.PlaySound("MissionComplete", SoundType.Effect, transform);
        }
        else
        {
            UIController.Instance.HideMissionUI();
            SoundManager.Instance.PlaySound("MissionFail", SoundType.Effect, transform);
        }
    }

    /// <summary>
    /// �̼� UI�� confirm button�� ������ �̺�Ʈ �Լ�
    /// </summary>
    private void ConfirmButton()
    {
        if (currentMissionId != -1)
        {
            MissionController.Instance.FinishMission(currentMissionId);
        }
        UIController.Instance.HideMissionUI();
    }


    private void OnDisable()
    {
        UIController.Instance.MissionMessage.OnAcceptBt -= AcceptButton;
        UIController.Instance.MissionMessage.OnCompleteBt -= CompleteButton;
        UIController.Instance.MissionMessage.OnConfirmBt -= ConfirmButton;
    }
}