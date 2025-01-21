using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class SkillTypeRandomPlayableAdapter : SkillTypePlayableAdapter
{
    public override EnAnimLayer GetOutputLayer()
    {
        return EnAnimLayer.Base;
    }
    private int[] m_IdleAnimList = null;
    private PlayableClipAdapter m_CurClipAdapter = null;
    protected override void OnDestroy()
    {
        PlayableAdapter.Destroy(m_CurClipAdapter);
        base.OnDestroy();
        m_IdleAnimList = null;
        m_CurClipAdapter = null;
    }
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);

        var data = userData.customData as SkillTypeLinkPlayableAdapterCustomData;
        m_IdleAnimList = data.arrParams;
        m_CurClipAdapter = userData.graph.CreateClipPlayableAdapter(m_IdleAnimList[0]);
        AddConnectRootAdapter(m_CurClipAdapter, GlobalConfig.Int0, GlobalConfig.Int1);
    }
    public override float GetUnitTime()
    {
        return m_CurClipAdapter.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return m_CurClipAdapter.GetPlayTime();
    }
    public override bool IsLoop()
    {
        return true;
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
    }
    public override void RemoveCmd()
    {
        base.RemoveCmd();
    }
    public override void ReExecuteCmd()
    {
        base.ReExecuteCmd();
    }
    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;
        if(GetPlaySchedule01() == 1)
        {
            DisconnectRootAdapter();
            PlayableAdapter.Destroy(m_CurClipAdapter);
            var index = Random.Range(GlobalConfig.Int0, m_IdleAnimList.Length);
            m_CurClipAdapter = m_Graph.CreateClipPlayableAdapter(m_IdleAnimList[index]);
            ConnectRootAdapter(m_CurClipAdapter);
        }
        return true;
    }
}
