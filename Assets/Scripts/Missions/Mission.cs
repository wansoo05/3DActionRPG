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
    //미션 고유 아이디
    public int ID;
    public MissionState State = MissionState.None;

    //미션 관련 NPC메세지
    public string[] messages; //0 : 미션 제안 메세지, 1 : 미션 수행중 메세지, 2 : 미션 완료 메세지

    //미션 창 UI메세지
    public string NoticeMsg;

    //미션 요구 사항
    public int[] EnemyIds;
    public int RequireCount;
    private int currentCount = 0;
    public int CurrentCount {get => currentCount; set => currentCount = value; }

    //미션 Accept Timeline
    public TimelineAsset AcceptAsset;
    //미션 Complete Timeline
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