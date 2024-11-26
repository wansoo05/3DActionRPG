using System;
using UnityEngine;
using UnityEngine.Timeline;

public enum MissionState
{
    None = 0, Active, Complete, Finished
}

[Serializable]
public class Mission
{
    //�̼� ���� ���̵�
    public int ID;
    public MissionState State = MissionState.None;

    //�̼� ���� NPC�޼���
    public string[] messages; //0 : �̼� ���� �޼���, 1 : �̼� ������ �޼���, 2 : �̼� �Ϸ� �޼���

    //�̼� â UI�޼���
    public string NoticeMsg;

    //�̼� �䱸 ����
    public int[] EnemyIds;
    public int RequireCount;
    private int currentCount = 0;
    public int CurrentCount {get => currentCount; set => currentCount = value; }

    //�̼� Accept Timeline
    public TimelineAsset AcceptAsset;
    //�̼� Complete Timeline
    public TimelineAsset CompleteAsset;

    public void Initialize()
    {
        currentCount = 0;
        State = MissionState.None;
    }

    public void EnemyKilled(int id)
    {
        foreach (int i in EnemyIds)
        {
            if (i == id)
            {
                currentCount++;
                currentCount = Mathf.Clamp(currentCount, 0, RequireCount);
                break;
            }
        }
    }

    public bool IsComplete()
    {
        return currentCount >= RequireCount;
    }
}