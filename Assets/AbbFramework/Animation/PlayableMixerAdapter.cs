using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Playables;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayableMixerAdapter : PlayableAdapter, IUpdate
{
    public static PlayableMixerAdapter Create(int entityID, PlayableGraphAdapter graph, EnAnimLayer layer, PlayableAdapter from, PlayableAdapter to, float time)
    {
        var adapter = GameClassPoolMgr.Instance.Pull<PlayableMixerAdapter>();
        adapter.Initialization(entityID, graph);
        adapter.SetMixerPlayable(layer, from, to, time);
        return adapter;
    }
    private AnimationMixerPlayable m_MixerPlayable;
    private ScriptPlayable<AdapterPlayable> m_AdapterPlayable;
    private EnAnimLayer m_Layer = EnAnimLayer.None;
    private PlayableAdapter m_From = null;
    private PlayableAdapter m_To = null;
    private float m_Time;
    private float m_EndTime;
    private UnityAction<EnAnimLayer, PlayableMixerAdapter, PlayableAdapter, PlayableAdapter> m_CompleteAction = null;
    private bool m_IsComplete = true;
    private int m_FromPort = GlobalConfig.Int1;
    private int m_ToPort = GlobalConfig.Int0;
    public override void OnDestroy()
    {
        if (!m_IsComplete)
            Complete();
        m_IsComplete = true;
        if (m_From != null && m_From.GetIsValid())
            PlayableAdapter.Destroy(m_From);
        if (m_To != null && m_To.GetIsValid())
            PlayableAdapter.Destroy(m_To);
        m_From = null;
        m_To = null;
        m_Time = -1;
        m_EndTime = 0;
        m_CompleteAction = null;
        m_Layer = EnAnimLayer.None;
        base.OnDestroy();
        m_MixerPlayable.Destroy();
        m_AdapterPlayable.Destroy();
    }
    public override void Initialization(int entityID, PlayableGraphAdapter graph)
    {
        base.Initialization(entityID, graph);
        m_AdapterPlayable = ScriptPlayable<AdapterPlayable>.Create(graph.GetGraph(), GlobalConfig.Int0);
        m_MixerPlayable = AnimationMixerPlayable.Create(graph.GetGraph(), GlobalConfig.Int2);
        m_AdapterPlayable.AddInput(m_MixerPlayable, GlobalConfig.Int0, GlobalConfig.Int1);
    }
    public void SetMixerPlayable(EnAnimLayer layer, PlayableAdapter from, PlayableAdapter to, float time)
    {
        m_From = from;
        m_To = to;
        m_Time = time;
        m_IsComplete = false;
        m_Layer = layer;
        var curTimeSec = ABBUtil.GetSystemTimeSeconds();
        m_EndTime = curTimeSec + time;

        m_MixerPlayable.ConnectInput(GlobalConfig.Int0, to.GetPlayable(), GlobalConfig.Int0, GlobalConfig.Int0);
        m_MixerPlayable.ConnectInput(GlobalConfig.Int1, from.GetPlayable(), GlobalConfig.Int0, GlobalConfig.Int1);

        UpdateMgr.Instance.Registener(this);
    }
    public void SetCompleteAction(UnityAction<EnAnimLayer, PlayableMixerAdapter, PlayableAdapter, PlayableAdapter> action)
    {
        m_CompleteAction = action;
    }
    private void Complete()
    {
        UpdateMgr.Instance.Unregistener(this);
        m_IsComplete = true;
        m_CompleteAction?.Invoke(m_Layer, this, m_From, m_To);
    }
    public override void Stop()
    {
        base.Stop();
        Complete();
    }
    public void DisconnectAll()
    {
        m_From = null;
        m_To = null;
        for (int i = 0; i < m_MixerPlayable.GetInputCount(); i++)
        {
            m_MixerPlayable.DisconnectInput(i);
        }
    }

    public override ScriptPlayable<AdapterPlayable> GetPlayable()
    {
        return m_AdapterPlayable;
    }
    public override PlayableAdapter GetMainPlayableAdapter()
    {
        //return base.GetMainPlayableAdapter();
        return m_To;
    }

    public void Update()
    {
        var curTime = ABBUtil.GetSystemTimeSeconds();
        if (curTime > m_EndTime)
        {
            Complete();
            return;
        }
        var residue = m_EndTime - curTime;
        var slider = Mathf.Clamp01(1 - (float)residue / m_Time);
        m_MixerPlayable.SetInputWeight(GlobalConfig.Int1, 1 - slider);
        m_MixerPlayable.SetInputWeight(GlobalConfig.Int0, slider);
    }
}
