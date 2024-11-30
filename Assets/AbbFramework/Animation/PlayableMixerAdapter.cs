using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class PlayableMixerAdapter : PlayableAdapter
{

    public override void ConnectInputTo(PlayableAdapter playableAdapter, int portID)
    {
        throw new System.NotImplementedException();
    }

    public override void ConnectOutputTo(int portID, PlayableAdapter playableAdapter)
    {
        throw new System.NotImplementedException();
    }

    public override ScriptPlayable<AdapterPlayable> GetPlayable()
    {
        throw new System.NotImplementedException();
    }

    public override void Initialization(PlayableGraphAdapter graph)
    {
        throw new System.NotImplementedException();
    }
}
