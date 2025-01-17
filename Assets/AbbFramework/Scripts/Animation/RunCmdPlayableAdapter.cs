using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;


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
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);
        if (userData is not PlayableAdapterUserData playableData)
            return;
        var runData = playableData.customData as AttackCmdPlayableAdapterData;
        m_RunAnimList = runData.arrParams.Copy();
        m_CurClipAdapter = m_Graph.CreateClipPlayableAdapter(m_RunAnimList[0]);
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
