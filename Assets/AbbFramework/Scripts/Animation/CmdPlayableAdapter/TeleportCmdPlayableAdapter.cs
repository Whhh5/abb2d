using System.Collections.Generic;
using UnityEngine;

public class TeleportCmdPlayableAdapter : CmdPlayableAdapter
{
    private List<int> m_ClipList = null;
    private PlayableAdapter m_PlayableAdapter = null;
    protected override void OnDestroy()
    {
        PlayableAdapter.Destroy(m_PlayableAdapter);
        base.OnDestroy();
        m_PlayableAdapter = null;
        m_ClipList = null;
    }
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);
        var roleID = Entity3DMgr.Instance.EntityID2RoleID(m_Graph);
        m_ClipList = AnimMgr.Instance.GetTeleportAnimClipList(roleID);
        m_PlayableAdapter = m_Graph.CreateClipPlayableAdapter(m_ClipList[0]);
        AddConnectRootAdapter(m_PlayableAdapter);
        SetSpeed(2);
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();

        var paramUserData = ClassPoolMgr.Instance.Pull<EntityMoveDownBuffParamsUserData>();
        paramUserData.value = 3f;
        var param = ClassPoolMgr.Instance.Pull<EntityMoveDownBuffParams>(paramUserData);
        ClassPoolMgr.Instance.Push(paramUserData);
        Entity3DMgr.Instance.AddEntityBuff(m_Graph, EnBuff.MoveDown, param);
    }
    public override void RemoveCmd()
    {
        Entity3DMgr.Instance.RemoveEntityBuff(m_Graph, EnBuff.MoveDown);
        base.RemoveCmd();
    }
    public override bool NextAnimLevelComdition()
    {
        return GetPlaySchedule01() >= GlobalConfig.Float1;
    }
    public override float GetPlayTime()
    {
        return m_PlayableAdapter.GetPlayTime();
    }
    public override float GetUnitTime()
    {
        return m_PlayableAdapter.GetUnitTime();
    }
}
