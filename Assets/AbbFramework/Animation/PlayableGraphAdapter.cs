using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms;

public enum EnAnimLayer
{
    None = -1,
    Base = 0,
    Layer1,
    Layer2,
    Layer3,
    EnumCount,
}
public class LayerMixerPlayableConnectInfo : IGamePool
{
    public EnAnimLayer layer;
    public int inputPort;
    public void OnPoolRecycle()
    {
        layer = EnAnimLayer.None;
        inputPort = -1;
    }
}
public class PlayableGraphAdapter : IGamePool, IUpdate
{

    public static PlayableGraphAdapter Create(int entityID, Animator animator)
    {
        var data = GameClassPoolMgr.Instance.Pull<PlayableGraphAdapter>();
        data.Initialization(entityID, animator);
        return data;
    }
    public static void OnDestroy(PlayableGraphAdapter graph)
    {
        graph.OnDestroy();
        GameClassPoolMgr.Instance.Push(graph);
    }
    private PlayableGraph m_Graph = default;
    private int m_EntityID = -1;
    private PlayableLayerMixerAdapter m_LayerMixerPlayable;
    public ScriptPlayable<AdapterPlayable> layerMixer => m_LayerMixerPlayable.GetPlayable();
    private Dictionary<EnAnimLayer, PlayableAdapter> m_LayerAdapter = new();
    private Dictionary<PlayableAdapter, LayerMixerPlayableConnectInfo> m_ConnectInfo = new();


    public void OnDestroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        PlayableLayerMixerAdapter.Destroy(m_LayerMixerPlayable);
        m_LayerAdapter.Clear();
        m_Graph.Destroy();
        m_EntityID = -1;
    }
    private void Initialization(int entityID, Animator anim)
    {
        m_EntityID = entityID;
        m_Graph = PlayableGraph.Create($"custom-{anim.name}");
        m_Graph.SetTimeUpdateMode(DirectorUpdateMode.GameTime);
        var output = AnimationPlayableOutput.Create(m_Graph, $"{anim.name}-output", anim);
        m_LayerMixerPlayable = PlayableLayerMixerAdapter.Create(entityID, this);
        output.SetSourcePlayable(m_LayerMixerPlayable.GetLayerMixerPlayable());
        UpdateMgr.Instance.Registener(this);
    }
    public bool Connect(EnAnimLayer layer, PlayableAdapter playable)
    {
        if (m_ConnectInfo.ContainsKey(playable))
            return false;

        var connectInfo = GameClassPoolMgr.Instance.Pull<LayerMixerPlayableConnectInfo>();
        m_ConnectInfo.Add(playable, connectInfo);
        if (!playable.IsLoop())
            m_NoLoopPlayableList.Add(playable);

        var inputPort = m_LayerMixerPlayable.ConnectLayerInput(playable, layer);

        connectInfo.layer = layer;
        connectInfo.inputPort = inputPort;
        return true;
    }
    public bool Connect(PlayableAdapter playable)
    {
        var result = Connect(EnAnimLayer.Layer1, playable);
        return result;
    }
    public bool ContainsConnect(PlayableAdapter playable)
    {
        var result = m_ConnectInfo.ContainsKey(playable);
        return result;
    }
    public bool DisConnect(PlayableAdapter playable)
    {
        if (!m_ConnectInfo.TryGetValue(playable, out var connectInfo))
            return false;
        m_ConnectInfo.Remove(playable);
        m_NoLoopPlayableList.Remove(playable);
        var layer = connectInfo.layer;
        m_LayerMixerPlayable.DisconnectLayerInput(layer, playable);
        GameClassPoolMgr.Instance.Push(connectInfo);
        return true;
    }
    public PlayableGraph GetGraph()
    {
        return m_Graph;
    }
    public void UpdtaeGraphEvaluate()
    {
        m_Graph.Evaluate(Time.deltaTime);
    }
    public bool IsPlaying()
    {
        return m_Graph.IsPlaying();
    }
    public void PlayGraph()
    {
        m_Graph.Play();
    }
    public void StopGraph()
    {
        m_Graph.Stop();
    }
    public void PauseGraph()
    {

    }

    private HashSet<PlayableAdapter> m_NoLoopPlayableList = new();
    public void Update()
    {
        UpdtaeGraphEvaluate();


        var disconnectList = new List<PlayableAdapter>();
        foreach (var item in m_NoLoopPlayableList)
        {
            var time = item.GetUnitTime();
            var playTime = item.GetPlayTime();
            if (playTime < time)
                break;
            disconnectList.Add(item);
        }
        foreach (var item in disconnectList)
        {
            DisConnect(item);
        }
    }
}
