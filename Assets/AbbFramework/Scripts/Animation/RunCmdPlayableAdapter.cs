using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;


public class RunCmdPlayableAdapterData : IPlayableAdapterCustomData
{
    public int[] arrClip;
}
public class RunCmdPlayableAdapter : CmdPlayableAdapter
{
    protected int[] m_RunAnimList = null;
    private PlayableClipAdapter m_CurClipAdapter = null;
    protected override void OnDestroy()
    {
        PlayableAdapter.Destroy(m_CurClipAdapter);
        m_RunAnimList = null;
        base.OnDestroy();
    }
    protected override void Initialization(PlayableGraphAdapter graph, IPlayableAdapterCustomData customData)
    {
        base.Initialization(graph, customData);
        var runData = customData as RunCmdPlayableAdapterData;
        m_RunAnimList = runData.arrClip.Copy();
        m_CurClipAdapter = graph.CreateClipPlayableAdapter(m_RunAnimList[0]);
        AddConnectRootAdapter(m_CurClipAdapter);
    }
    public override float GetUnitTime()
    {
        return m_CurClipAdapter.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return m_CurClipAdapter.GetPlayTime();
    }
}
