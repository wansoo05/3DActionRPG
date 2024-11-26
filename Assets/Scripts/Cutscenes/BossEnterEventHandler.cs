using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class BossEnterEventHandler : MonoBehaviour
{
    [SerializeField]
    private TimelineAsset asset;

    private PlayableDirector pd;

    private void Start()
    {
        pd = GameManager.Instance.TimelinePlayerInstance.GetComponent<PlayableDirector>();
        pd.played += OnTimelineStarted;
    }

    private void OnDisable()
    {
        pd.played -= OnTimelineStarted;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            GameManager.Instance.TimelinePlayerInstance.PlayTimeline(asset);
        }
    }

    private void OnTimelineStarted(PlayableDirector director)
    {
        string currentAssetName = director.playableAsset.name;
        if (currentAssetName == "BossEnterTimeline")
            gameObject.SetActive(false);
    }
}