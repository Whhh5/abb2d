using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;


public class PlayableLayerMixerAdapter : PlayableAdapter
{
    public static PlayableLayerMixerAdapter Create(PlayableGraphAdapter graph)
    {
        var data = GameUtil.GetClass<PlayableLayerMixerAdapter>();
        data.Initialization(graph);
        return data;
    }
    private AnimationLayerMixerPlayable m_LayerMixer;
    private ScriptPlayable<AdapterPlayable> m_Adapter;

    public override void OnDestroy()
    {
        m_LayerMixer.Destroy();
        m_Adapter.Destroy();
        base.OnDestroy();
    }

    public override ScriptPlayable<AdapterPlayable> GetPlayable()
    {
        return m_Adapter;
    }

    public override void Initialization(PlayableGraphAdapter graph)
    {
        base.Initialization(graph);
        m_Adapter = ScriptPlayable<AdapterPlayable>.Create(graph.GetGraph());
        m_LayerMixer = AnimationLayerMixerPlayable.Create(graph.GetGraph(), 5);
        m_Adapter.AddInput(m_LayerMixer, 0, 1);
    }

    public override void ConnectInputTo(PlayableAdapter playableAdapter, int portID)
    {
        m_LayerMixer.AddInput(playableAdapter.GetPlayable(), portID, 1);
    }

    public override void ConnectOutputTo(int portID, PlayableAdapter playableAdapter)
    {
        
    }
}
