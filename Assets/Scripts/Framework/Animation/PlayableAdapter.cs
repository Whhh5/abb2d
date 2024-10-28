using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public abstract class PlayableAdapter: IGamePool
{
    public abstract EnClassType ClassType { get; }

    public static void Destroy(PlayableAdapter layerMaskAdapter)
    {
        layerMaskAdapter.OnDestroy();
        GameUtil.PushClass(layerMaskAdapter);
    }
    public virtual void Initialization(PlayableGraphAdapter graph)
    {

    }
    public virtual void OnDestroy()
    {

    }
    public abstract ScriptPlayable<AdapterPlayable> GetPlayable();
    public abstract void ConnectInputTo(PlayableAdapter playableAdapter, int portID);
    public abstract void ConnectOutputTo(int portID, PlayableAdapter playableAdapter);
    public virtual void ConnectTo(int outputPortID, ScriptPlayable<AdapterPlayable> toPlayable, int inputPortID)
    {

    }
}