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
    /// NPC가 가지고 있는 미션들의 아이디들
    /// </summary>
    [SerializeField]
    private int[] missionIds;

    /// <summary>
    /// NPC 기본 고유 메세지
    /// </summary>
    [SerializeField]
    private string defaultMessage = "안녕하세요."; //NPC 고유 메세지

    /// <summary>
    /// 미션 상태를 알려줄 퀘스트 마크 스프라이트들
    /// </summary>
    [SerializeField]
    private Sprite[] markSprites;

    private Animator animator;

    //UI관련 멤버
    private Canvas perceptionUI;
    private Canvas questMark;
    private Image questImg;

    //현재 진행중인 미션정보
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
        //빌보드 UI 카메라 방향으로 회전시키기
        if (perceptionUI != null)
        {
            perceptionUI.transform.localPosition = perceptionPositionOffset;
            perceptionUI.transform.rotation = Camera.main.transform.rotation;
        }

        if (questMark != null)
        {
            questMark.transform.rotation = Camera.main.transform.rotation;
        }


        //Player 감지한 경우
        Transform player;
        if (DetectPlayer(out player))
        {
            perceptionUI.gameObject.SetActive(true);

            //플레이어를 바라보게 회전
            Vector3 direction = player.transform.position - transform.position; 
            direction.y = 0.0f;

            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(direction.normalized), 5.0f * Time.deltaTime);

            //'T' 키를 누르면 미션UI 보여주기
            if (Input.GetKeyDown(KeyCode.T))
            {
                UIController.Instance.ShowMissionUI();
            }
        }
        //감지못한 경우
        else
        {
            perceptionUI.gameObject.SetActive(false);
        }

        //미션 상태에 따른 현재 진행 가능한 미션 아이디 찾기
        currentMissionId = -1; //현재 진행 가능한 미션이 없으면 -1
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
    /// /// Player가 범위 범위 내에 있으면 True 없으면 False
    /// </summary>
    /// <param name="player">범위 안에 있는 Player 반환</param>
    /// <returns></returns>
    private bool DetectPlayer(out Transform player)
    {
        Collider[] colliders = Physics.OverlapBox(transform.position + Vector3.up, new Vector3(3.0f, 1.5f, 3.0f), transform.rotation, 1 << 6);

        foreach (Collider collider in colliders)
        {
            //Player이라면
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
    /// 미션 상태에 따른 UI들 변경
    /// </summary>
    private void LateUpdate()
    {
        Update_QuestUIByMission();
        Update_MissionMessage();
    }

    
    private void Update_QuestUIByMission()
    {
        //남아 있는 미션이 없으면 퀘스트 마크를 비활성화 시키고 퀘스트 마크 업데이트를 하지 않는다.
        if (IsEmptyMission())
        {
            questMark.gameObject.SetActive(false);

            return;
        }

        //현재 미션을 진행중이라면
        if (MissionController.Instance.GetMissionState(currentMissionId) == MissionState.Active)
        {
            //미션을 완료했을 때 마크 업데이트
            if (MissionController.Instance.IsMissionComplete(currentMissionId))
            {
                questImg.sprite = markSprites[2];
            }
            //미션중일 때 마크 업데이트
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
        //플레이어가 감지되지 않으면 미션 메세지를 업데이트 시키지 않는다.
        if (DetectPlayer(out player) == false)
            return;

        //남아 있는 미션이 있다면
        if (IsEmptyMission() == false)
            //현재 미션의 메세지를 업데이트시킨다.
            MissionController.Instance.UpdateMissionMessage(currentMissionId);
        else
            //남이 있는 미션이 없으면 NPC 고유 메세지를 업데이트시킨다.
            UIController.Instance.UpdateMissionMessage(defaultMessage);
    }

    /// <summary>
    /// 남아 있는 미션이 있으면 false 없으면 true
    /// </summary>
    /// <returns></returns>
    private bool IsEmptyMission()
    {
        if (currentMissionId == -1)
            return true;
        return false;
    }

    //애니메이션 이벤트 : NPC 모션 끝을 감지하여 랜덤 모션 재생
    private void End_Motion()
    {
        motionType = UnityEngine.Random.Range(0, 3);
        animator.SetInteger("MotionType", motionType);
    }

    /// <summary>
    /// 미션 UI의 accept button에 연결한 이벤트 함수
    /// </summary>
    public void AcceptButton()
    {
        MissionController.Instance.AcceptMission(currentMissionId);
    }

    /// <summary>
    /// 미션 UI의 complete button에 연결한 이벤트 함수
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
    /// 미션 UI의 confirm button에 연결한 이벤트 함수
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