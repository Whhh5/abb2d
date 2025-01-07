using System;
using System.Collections.Generic;
using UnityEngine;

public class JumpCmdPlayableAdapter : CmdPlayableAdapter
{
    private PlayableClipAdapter m_ClipAdapter = null;
    private List<int> m_JumpLinkAnim = null;
    private int m_Index = -1;
    private int m_InputPort = -1;
    protected override void OnDestroy()
    {
        DisconnectRootAdapter(m_InputPort);
        PlayableClipAdapter.Destroy(m_ClipAdapter);
        base.OnDestroy();
        m_ClipAdapter = null;
        m_JumpLinkAnim = null;
        m_Index = -1;
        m_InputPort = -1;
    }

    public override void OnPoolInit(CustomPoolData userData)
    {
        base.OnPoolInit(userData);
        var roleID = EntityMgr.Instance.EntityID2RoleID(m_Graph.GetEntityID());
        m_JumpLinkAnim = AnimMgr.Instance.GetJumpAnimClipList(roleID);

        m_Index = 0;
        m_ClipAdapter = m_Graph.CreateClipPlayableAdapter(m_JumpLinkAnim[m_Index]);
        m_InputPort = AddConnectRootAdapter(m_ClipAdapter);
    }

    public override float GetUnitTime()
    {
        return m_ClipAdapter.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return m_ClipAdapter.GetPlayTime();
    }

    public override void ReExecuteCmd()
    {
        base.ReExecuteCmd();

        if (m_Index >= m_JumpLinkAnim.Count)
            return;
        if (GetPlaySchedule01() < GlobalConfig.Float1)
            return;
        m_Index++;
        DisconnectRootAdapter(m_InputPort);
        PlayableClipAdapter.Destroy(m_ClipAdapter);
        m_ClipAdapter = m_Graph.CreateClipPlayableAdapter(m_JumpLinkAnim[m_Index]);
        ConnectRootAdapter(m_InputPort, m_ClipAdapter.GetPlayable());
    }
}
