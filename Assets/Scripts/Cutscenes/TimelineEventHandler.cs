using System;
using UnityEngine;
using UnityEngine.Playables;

public class TimelineEventHandler : MonoBehaviour
{
    [SerializeField]
    private GameObject portal;

    private PlayableDirector pd;

    private void Start()
    {
        pd = GameManager.Instance.TimelinePlayerInstance.GetComponent<PlayableDirector>();
        pd.played += OnTimelineStarted;
    }

    private void OnTimelineStarted(PlayableDirector director)
    {
        string currentAssetName = director.playableAsset.name;
        if (currentAssetName == "MissionAcceptTimeline" || currentAssetName == "BossEndTimeline")
            portal.SetActive(true);
            
    }

    private void OnDisable()
    {
        pd.played -= OnTimelineStarted;
    }
}