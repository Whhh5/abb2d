using System.Collections.Generic;
using UnityEngine;

public class InjuredCmdPlayableAdapter : CmdPlayableAdapter
{
    private PlayableAdapter m_CurPlayableAdapter = null;
    private List<int> m_ClipIDList = null;
    protected override void OnDestroy()
    {
        DisconnectRootAdapter();
        PlayableAdapter.Destroy(m_CurPlayableAdapter);
        base.OnDestroy();
        m_CurPlayableAdapter = null;
        m_ClipIDList = null;
    }
    protected override void Initialization(PlayableGraphAdapter graph, IPlayableAdapterCustomData customData)
    {
        base.Initialization(graph, customData);
        var roleID = EntityMgr.Instance.EntityID2RoleID(graph);
        m_ClipIDList = AnimMgr.Instance.GetInjuredAnimClipList(roleID);
        m_CurPlayableAdapter = graph.CreateClipPlayableAdapter(m_ClipIDList[0]);
        AddConnectRootAdapter(m_CurPlayableAdapter);
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
        Entity3DMgr.Instance.AddEntityBuff(m_Graph, EnBuff.NoMove);
        Entity3DMgr.Instance.AddEntityBuff(m_Graph, EnBuff.NoJump);
    }
    public override void RemoveCmd()
    {
        Entity3DMgr.Instance.RemoveEntityBuff(m_Graph, EnBuff.NoMove);
        Entity3DMgr.Instance.RemoveEntityBuff(m_Graph, EnBuff.NoJump);
        base.RemoveCmd();
    }
    public override bool NextAnimLevelComdition()
    {
        return true;
    }
    public override float GetUnitTime()
    {
        return m_CurPlayableAdapter.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return m_CurPlayableAdapter.GetPlayTime();
    }
}
