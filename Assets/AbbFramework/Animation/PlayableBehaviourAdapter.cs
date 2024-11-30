using System;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableBehaviourAdapter : PlayableBehaviour
{

}

public class PlayableScriptBrhaviour : IPlayableBehaviour
{
    public void OnBehaviourPause(Playable playable, FrameData info)
    {
        Debug.Log("OnBehaviourPause");
    }

    public void OnBehaviourPlay(Playable playable, FrameData info)
    {
        Debug.Log("OnBehaviourPlay");
    }

    public void OnGraphStart(Playable playable)
    {
        Debug.Log("OnGraphStart");
    }

    public void OnGraphStop(Playable playable)
    {
        Debug.Log("OnGraphStop");
    }

    public void OnPlayableCreate(Playable playable)
    {
        Debug.Log("OnPlayableCreate");
    }

    public void OnPlayableDestroy(Playable playable)
    {
        Debug.Log("OnPlayableDestroy");
    }

    public void PrepareFrame(Playable playable, FrameData info)
    {
        Debug.Log("PrepareFrame");
    }

    public void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        Debug.Log("ProcessFrame");
    }
}


