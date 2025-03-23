using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Numerics;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Playables;
using static UnityEditor.Experimental.GraphView.GraphView;

public delegate void PlayableMixerCompleteAction(PlayableMixerAdapter mixer, PlayableAdapter from, PlayableAdapter to);

public class PlayableMixerAdapterData : IPlayableAdapterCustomData
{
    public PlayableAdapter from;
    public PlayableAdapter to;
    public float time;
    public PlayableMixerCompleteAction complete;

    public void OnPoolDestroy()
    {
        from = null;
        to = null;
        time = -1;
        complete = null;
    }
}
public class PlayableMixerAdapter : PlayableAdapter
{
    private AnimationMixerPlayable m_MixerPlayable;
    private PlayableAdapter m_From = null;
    private PlayableAdapter m_To = null;
    private float m_Time;
    private float m_EndTime;
    private PlayableMixerCompleteAction m_CompleteAction = null;
    private bool m_IsComplete = true;
    protected override void OnDestroy()
    {
        if (!m_IsComplete)
            Complete();
        m_IsComplete = true;
        if (m_From != null && m_From.GetIsValid())
            PlayableAdapter.Destroy(m_From);
        if (m_To != null && m_To.GetIsValid())
            PlayableAdapter.Destroy(m_To);
        base.OnDestroy();
        m_MixerPlayable.Destroy();
        m_From = null;
        m_To = null;
        m_Time = -1;
        m_EndTime = 0;
        m_CompleteAction = null;
    }
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        var data = (PlayableMixerAdapterData)userData.customData;
        m_MixerPlayable = AnimationMixerPlayable.Create(m_Graph.GetGraph(), GlobalConfig.Int2);
        AddConnectRootAdapter(m_MixerPlayable);


        var mixerTime = Mathf.Min(data.time, data.to.GetUnitTime());
        m_CompleteAction = data.complete;
        m_From = data.from;
        m_To = data.to;
        m_Time = mixerTime;
        m_IsComplete = false;
        var curTimeSec = ABBUtil.GetGameTimeSeconds();
        m_EndTime = curTimeSec + mixerTime;

        m_MixerPlayable.ConnectInput(GlobalConfig.Int0, data.to.GetPlayable(), GlobalConfig.Int0, GlobalConfig.Int0);
        m_MixerPlayable.ConnectInput(GlobalConfig.Int1, data.from.GetPlayable(), GlobalConfig.Int0, GlobalConfig.Int1);
    }
    private void MixerComplete()
    {
        m_CompleteAction.Invoke(this, m_From, m_To);
    }
    public override void Complete()
    {
        base.Complete();
        m_IsComplete = true;
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
    public override PlayableAdapter GetMainPlayableAdapter()
    {
        return m_To.GetMainPlayableAdapter();
    }

    public override float GetUnitTime()
    {
        return m_To.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return m_To.GetPlayTime();
    }

    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;
        if (m_IsComplete)
            return false;
        var curTime = ABBUtil.GetGameTimeSeconds();
        if (curTime > m_EndTime)
        {
            Complete();
            MixerComplete();
            return true;
        }
        var residue = m_EndTime - curTime;
        var slider = Mathf.Clamp01(1 - (float)residue / m_Time);
        m_MixerPlayable.SetInputWeight(GlobalConfig.Int1, 1 - slider);
        m_MixerPlayable.SetInputWeight(GlobalConfig.Int0, slider);
        return true;
    }

    public override EnAnimLayer GetOutputLayer()
    {
        return m_To.GetOutputLayer();
    }
}
