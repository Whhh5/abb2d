using System.Collections.Generic;
using UnityEngine;

public class JumpDownCmdPlayableAdapter : CmdPlayableAdapter
{
    private List<int> m_JumpDownAnimList = null;
    private PlayableClipAdapter m_Clipadapter = null;
    private int m_DownType = -1;
    protected override void OnDestroy()
    {
        PlayableAdapter.Destroy(m_Clipadapter);
        base.OnDestroy();
        m_JumpDownAnimList = null;
        m_Clipadapter = null;
        m_DownType = -1;
    }
    protected override void Initialization(PlayableGraphAdapter graph, IPlayableAdapterCustomData customData)
    {
        base.Initialization(graph, customData);


        var roleID = EntityMgr.Instance.EntityID2RoleID(graph.GetEntityID());
        m_JumpDownAnimList = AnimMgr.Instance.GetJumpDownAnimClipList(roleID);


        var velocity = Entity3DMgr.Instance.GetEntityVerticalVelocity(graph.GetEntityID());
        var isHeight = velocity < -10;
        SetDownType(isHeight ? 1 : 0);
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
        if (m_DownType == 1)
        {
            Entity3DMgr.Instance.AddEntityBuff(m_Graph.GetEntityID(), EnBuff.NoJump);
            Entity3DMgr.Instance.AddEntityBuff(m_Graph.GetEntityID(), EnBuff.NoMove);
        }
    }
    public override void RemoveCmd()
    {
        if (m_DownType == 1)
        {
            Entity3DMgr.Instance.RemoveEntityBuff(m_Graph.GetEntityID(), EnBuff.NoJump);
            Entity3DMgr.Instance.RemoveEntityBuff(m_Graph.GetEntityID(), EnBuff.NoMove);
        }
        base.RemoveCmd();
    }
    public override bool NextAnimLevelComdition()
    {
        return m_DownType == 0 ? true : GetPlaySchedule01() > GlobalConfig.Float08;
    }
    private void SetDownType(int type)
    {
        m_DownType = type;
        m_Clipadapter = m_Graph.CreateClipPlayableAdapter(m_JumpDownAnimList[type]);
        AddConnectRootAdapter(m_Clipadapter);
    }

    public override float GetUnitTime()
    {
        return m_Clipadapter.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return m_Clipadapter.GetPlayTime();
    }
}
