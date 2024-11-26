using System.Collections.Generic;
using UnityEngine;

public class MovableStopper : MonoBehaviour
{
    private static MovableStopper instance;

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

    public static MovableStopper Instance => instance;

    private HashSet<IStoppable> stoppers = new HashSet<IStoppable>();

    public void Regist(IStoppable stopper)
    {
        stoppers.Add(stopper);
    }

    public void Start_Delay(int frame)
    {
        if (frame < 1)
            return;

        foreach(IStoppable stopper  in stoppers)
        {
            StartCoroutine(stopper.Start_FrameDelay(frame));
        }
    }

    public void Delete(IStoppable stopper)
    {
        stoppers.Remove(stopper);
        print($"movablestopper : {stoppers.Count}");
    }
}