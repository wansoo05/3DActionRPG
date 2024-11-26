using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

public class TimelinePlayer : MonoBehaviour
{
    private PlayableDirector pd;
    private void Awake()
    {
        pd = GetComponent<PlayableDirector>();
    }

    public void PlayTimeline(TimelineAsset asset)
    {
        pd.playableAsset = asset;
        pd.time = 0;
        pd.Play();
    }
}