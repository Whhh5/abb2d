using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Animations;


public class RunCmdPlayableAdapterData : IPlayableAdapterCustomData
{
    public int[] arrClip;

    public void OnPoolDestroy()
    {
        arrClip = null;
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit<T>(ref T userData) where T : struct, IPoolUserData
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
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
    public override void OnPoolInit<T>(ref T userData)
    {
        base.OnPoolInit(ref userData);
        if (userData is not PlayableAdapterUserData playableData)
            return;
        var runData = playableData.customData as RunCmdPlayableAdapterData;
        m_RunAnimList = runData.arrClip.Copy();
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
