using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

//[AssetFileNameExtension("signal")]
//[TimelineHelpURL(typeof(SignalAsset))]
//[TimelineHelpURL(typeof(TestSignal))]
[TrackColor(0,1,0)]
[TrackClipType(typeof(TestSignalAsset))]
//[TrackBindingType(typeof(TestSignalAsset))]
public class TestSignal : MarkerTrack
{
    void OnEnable()
    {
        //Debug.Log("TestSignal OnEnable");
    }

    private void OnDisable()
    {
        //Debug.Log("TestSignal OnDisable");
    }

    
}


[System.Serializable]
public class TestSignalAsset : Marker //, INotification, INotificationOptionProvider
{
    public string TTTT;

    public NotificationFlags flags => NotificationFlags.TriggerOnce;

    private PropertyName m_PropertyName = new("TestSignal");
    public PropertyName id => m_PropertyName;

    public override void OnInitialize(TrackAsset aPent)
    {
        base.OnInitialize(aPent);
    }


    //public override void OnInitialize(TrackAsset aPent)
    //{
    //    base.OnInitialize(aPent);
    //    Debug.Log($"OnInitialize");
    //}

}

public class TestReceiver : INotificationReceiver
{
    public void OnNotify(Playable origin, INotification notification, object context)
    {
        Debug.Log(11111);
    }
}

public class TestSignalPlayable: PlayableBehaviour
{
    public override void OnBehaviourPlay(Playable playable, FrameData info)
    {
        base.OnBehaviourPlay(playable, info);
        Debug.Log("OnBehaviourPlay");
    }
    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
        Debug.Log("OnBehaviourPause");
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        base.PrepareFrame(playable, info);
        Debug.Log("PrepareFrame");
    }
    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        Debug.Log("ProcessFrame");
    }
    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
        Debug.Log("OnGraphStart");
    }
    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);
        Debug.Log("OnPlayableCreate");
    }
    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
        Debug.Log("OnGraphStop");
    }
}