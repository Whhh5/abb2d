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
    public override void OnPoolInit<T>(ref T userData)
    {
        base.OnPoolInit(ref userData);

        var roleID = EntityMgr.Instance.EntityID2RoleID(m_Graph);
        m_ClipIDList = AnimMgr.Instance.GetInjuredAnimClipList(roleID);
        m_CurPlayableAdapter = m_Graph.CreateClipPlayableAdapter(m_ClipIDList[0]);
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
