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

    //��ư �̺�Ʈ�� �������� �������ֱ� ���� event
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

    //�̼� ���� ��ư
    public void AcceptButton()
    {
        OnAcceptBt?.Invoke();
        UIController.Instance.HideMissionUI();
    }

    //�̼� ���� ��ư
    public void RejectButton()
    {
        UIController.Instance.HideMissionUI();
    }

    //�̼� �Ϸ� ��ư
    public void CompleteButton()
    {
        OnCompleteBt?.Invoke();
    }

    //Ȯ�� ��ư
    public void ConfirmButton()
    {
        OnConfirmBt?.Invoke();
    }

    /// <summary>
    /// �̼� ���¿� ���� ��ư ����
    /// </summary>
    /// <param name="state">�̼� ���� ������ ����</param>
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