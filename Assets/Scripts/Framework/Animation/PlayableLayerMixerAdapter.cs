using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


public class PlayableLayerMixerAdapter : PlayableAdapter
{
    private AnimationLayerMixerPlayable m_LayerMixer;
    private ScriptPlayable<AdapterPlayable> m_Adapter;

    public override EnClassType ClassType => EnClassType.PlayableLayerMixerAdapter;

    public override ScriptPlayable<AdapterPlayable> GetPlayable()
    {
        return m_Adapter;
    }

    public override void Initialization(PlayableGraphAdapter graph)
    {
        m_Adapter = ScriptPlayable<AdapterPlayable>.Create(graph.GetGraph());
        m_LayerMixer = AnimationLayerMixerPlayable.Create(graph.GetGraph(), 5);
        m_Adapter.ConnectInput(0, m_LayerMixer, 0, 1);
    }

    public override void ConnectInputTo(PlayableAdapter playableAdapter, int portID)
    {
        
    }

    public override void ConnectOutputTo(int portID, PlayableAdapter playableAdapter)
    {
        
    }
}
