using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

public enum EnAnimLayer
{
    Base = 0,
    Layer1,
    Layer2,
    Layer3,
    EnumCount,
}
public class PlayableGraphAdapter: IGamePool
{
    public EnClassType ClassType => throw new System.NotImplementedException();

    public static PlayableGraphAdapter Create(Animator animator)
    {
        var data = GameUtil.PullClass<PlayableGraphAdapter>();
        data.Initialization(animator);
        return data;
    }
    public static void OnDestroy(PlayableGraphAdapter graph)
    {
        GameUtil.PushClass(graph);
    }
    private PlayableGraph m_Graph = default;
    private PlayableLayerMixerAdapter m_LayerMixerPlayable;
    private Dictionary<EnAnimLayer, PlayableAdapter> m_LayerAdapter = new();

    public void OnDestroy()
    {
        PlayableLayerMixerAdapter.Destroy(m_LayerMixerPlayable);
        m_LayerAdapter.Clear();
        m_Graph.Destroy();

    }
    public void Initialization(Animator anim)
    {
        m_Graph = PlayableGraph.Create($"{anim.name}");
        m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        var output = AnimationPlayableOutput.Create(m_Graph, $"{anim.name}-output", anim);
        m_LayerMixerPlayable = PlayableLayerMixerAdapter.Create(this);
        output.SetSourcePlayable(m_LayerMixerPlayable.GetPlayable());
        m_Graph.Play();
    }
    public void Connect(EnAnimLayer layer, PlayableAdapter playable)
    {
        m_LayerMixerPlayable.ConnectInputTo(playable, (int)layer);
    }
    public void Connect(PlayableAdapter playable)
    {
        Connect(EnAnimLayer.Base, playable);
    }
    public void DisConnect(PlayableAdapter playable)
    {
        
    }
    private void CheckoutLayerAdapter(EnAnimLayer layer)
    {
        if(!m_LayerAdapter.TryGetValue(layer, out var adapter))
        {

        }
    }
    public PlayableGraph GetGraph()
    {
        return m_Graph;
    }
}
