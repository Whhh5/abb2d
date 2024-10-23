using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public class PlayableGraphAdapter
{
    private PlayableGraph m_Graph = default;
    private PlayableLayerMixerAdapter m_MixerLayerPlayable = null;
    public void Initialization(Animator anim)
    {
        m_Graph = PlayableGraph.Create($"{anim.name}");
        m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        var output = AnimationPlayableOutput.Create(m_Graph, $"{anim.name}-output", anim);
        m_MixerLayerPlayable = new PlayableLayerMixerAdapter();
    }
    public void Connect(PlayableAdapter playable)
    {

    }
    public PlayableGraph GetGraph()
    {
        return m_Graph;
    }
}
