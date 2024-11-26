using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Timeline;

public class MissionController : MonoBehaviour
{
    [SerializeField]
    private Mission[] missions;

    private static MissionController instance;
    public static MissionController Instance => instance;

    private Dictionary<int, Mission> missionDic;
    private List<int> acceptedMissionIds;
    private List<int> finishedMissionIds;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        Factory.Instance.OnEnemyDead += OnEnemyDead;

        missionDic = new Dictionary<int, Mission>();
        acceptedMissionIds = new List<int>();
        finishedMissionIds = new List<int>();

        foreach (Mission mission in missions)
            missionDic[mission.ID] = mission;
    }

    
    private void Update()
    {
        UIController.Instance.UpdateMissionNotice("");

        foreach (int id in acceptedMissionIds)
        {
            Mission mission = missionDic[id];
            string notice = mission.NoticeMsg;
            string progressCount = mission.CurrentCount.ToString();
            string requireCount = mission.RequireCount.ToString();
            UIController.Instance.UpdateMissionNotice(notice + " : " + progressCount + " / " + requireCount);
        }
    }

    public bool IsPossibleMission(int id)
    {
        if(finishedMissionIds.Contains(id))
            return false;

        return true;
    }

    public void AcceptMission(int id)
    {
        missionDic[id].State = MissionState.Active;
        acceptedMissionIds.Add(id);
        if (missionDic[id].AcceptAsset != null)
            GameManager.Instance.TimelinePlayerInstance.PlayTimeline(missionDic[id].AcceptAsset);
    }

    public bool CompleteMission(int id)
    {
        if (missionDic[id].IsComplete())
        {
            acceptedMissionIds.Remove(id);
            missionDic[id].State = MissionState.Complete;
            return true;
        }

        return false;
    }

    public void FinishMission(int id)
    {
        missionDic[id].State = MissionState.Finished;
        finishedMissionIds.Add(id);

    }

    public MissionState GetMissionState(int id)
    {
        return missionDic[id].State;
    }

    public bool IsMissionComplete(int id)
    {
        return missionDic[id].IsComplete();
    }

    public void UpdateMissionMessage(int id)
    {
        UIController.Instance.UpdateMissionMessage(missionDic[id].messages[(int)missionDic[id].State]);
        UIController.Instance.MissionButtonSetting(missionDic[id].State);
    }

    private void OnEnemyDead(int id)
    {
        foreach(int i in acceptedMissionIds)
        {
            missionDic[i].EnemyKilled(id);
        }
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
        Factory.Instance.OnEnemyDead -= OnEnemyDead;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "TitleScene")
        {
            acceptedMissionIds = new List<int>();
            finishedMissionIds = new List<int>();
            foreach (Mission mission in missions)
            {
                mission.Initialize();
            }
        }
    }
}