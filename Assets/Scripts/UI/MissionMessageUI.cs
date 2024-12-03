using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MissionMessageUI : MonoBehaviour
{
    private TextMeshProUGUI missionMessage;
    private Button acceptBt;
    private Button rejectBt;
    private Button completeBt;
    private Button confirmBt;

    //버튼 이벤트를 이중으로 연결해주기 위한 event
    public event Action OnAcceptBt;
    public event Action OnCompleteBt;
    public event Action OnConfirmBt;

    private void Awake()
    {
        Transform t1 = transform.FindChildByName("Mission");
        Button[] bts = transform.GetComponentsInChildren<Button>();
        missionMessage = t1.GetComponent<TextMeshProUGUI>();
        Debug.Assert(missionMessage != null);
        acceptBt = bts[0];
        rejectBt = bts[1];
        completeBt = bts[2];
        confirmBt = bts[3];

        acceptBt.onClick.AddListener(AcceptButton);
        rejectBt.onClick.AddListener(RejectButton);
        completeBt.onClick.AddListener(CompleteButton);
        confirmBt.onClick.AddListener(ConfirmButton);
    }

    public void NoticeMessage(string msg)
    {
        missionMessage.text = msg;
    }

    //미션 수락 버튼
    public void AcceptButton()
    {
        OnAcceptBt?.Invoke();
        UIController.Instance.HideMissionUI();
    }

    //미션 거절 버튼
    public void RejectButton()
    {
        UIController.Instance.HideMissionUI();
    }

    //미션 완료 버튼
    public void CompleteButton()
    {
        OnCompleteBt?.Invoke();
    }

    //확인 버튼
    public void ConfirmButton()
    {
        OnConfirmBt?.Invoke();
    }

    /// <summary>
    /// 미션 상태에 따른 버튼 세팅
    /// </summary>
    /// <param name="state">미션 상태 열거형 변수</param>
    public void MissionButtonSetting(MissionState state)
    {
        switch (state)
        {
            case MissionState.None:
            {
                acceptBt.gameObject.SetActive(true);
                rejectBt.gameObject.SetActive(true);
                completeBt.gameObject.SetActive(false);
                confirmBt.gameObject.SetActive(false);
                break;
            }
            case MissionState.Active:
            {
                acceptBt.gameObject.SetActive(false);
                rejectBt.gameObject.SetActive(false);
                completeBt.gameObject.SetActive(true);
                confirmBt.gameObject.SetActive(false);
                break;
            }
            case MissionState.Complete:
            {
                acceptBt.gameObject.SetActive(false);
                rejectBt.gameObject.SetActive(false);
                completeBt.gameObject.SetActive(false);
                confirmBt.gameObject.SetActive(true);
                break;
            }
        }
    }
}