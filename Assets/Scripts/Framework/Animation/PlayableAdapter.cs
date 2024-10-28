using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;

public abstract class PlayableAdapter: IGamePool
{
    public abstract EnClassType ClassType { get; }

    public abstract void Initialization(PlayableGraphAdapter graph);
    public abstract ScriptPlayable<AdapterPlayable> GetPlayable();
    public abstract void ConnectInputTo(PlayableAdapter playableAdapter, int portID);
    public abstract void ConnectOutputTo(int portID, PlayableAdapter playableAdapter);
    public virtual void ConnectTo(int outputPortID, ScriptPlayable<AdapterPlayable> toPlayable, int inputPortID)
    {

    }
}