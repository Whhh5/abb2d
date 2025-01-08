using System.Collections.Generic;
using UnityEngine;

public class Skill1CmdPlayableAdapter : CmdPlayableAdapter
{
    private PlayableAdapter m_SkillClipAdapteer = null;
    private List<int> m_ClipList = null;
    private int m_Index = -1;
    protected override void OnDestroy()
    {
        PlayableAdapter.Destroy(m_SkillClipAdapteer);
        base.OnDestroy();
        m_SkillClipAdapteer = null;
        m_ClipList = null;
        m_Index = -1;
    }
    public override void OnPoolInit<T>(ref T userData)
    {
        base.OnPoolInit(ref userData);
        var roleID = Entity3DMgr.Instance.EntityID2RoleID(m_Graph.GetEntityID());
        m_ClipList = AnimMgr.Instance.GetSkill1AnimClipList(roleID);
        m_Index = 0;
        m_SkillClipAdapteer = m_Graph.CreateClipPlayableAdapter(m_ClipList[m_Index]);
        AddConnectRootAdapter(m_SkillClipAdapteer);
    }
    public override void RemoveCmd()
    {
        Entity3DMgr.Instance.RemoveEntityBuff(m_Graph.GetEntityID(), EnBuff.NoMove);
        Entity3DMgr.Instance.RemoveEntityBuff(m_Graph.GetEntityID(), EnBuff.NoJump);
        Entity3DMgr.Instance.RemoveEntityBuff(m_Graph.GetEntityID(), EnBuff.NoGravity);
        base.RemoveCmd();
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
        Entity3DMgr.Instance.AddEntityBuff(m_Graph.GetEntityID(), EnBuff.NoMove);
        Entity3DMgr.Instance.AddEntityBuff(m_Graph.GetEntityID(), EnBuff.NoJump);
        Entity3DMgr.Instance.AddEntityBuff(m_Graph.GetEntityID(), EnBuff.NoGravity);
    }
    public override bool NextAnimLevelComdition()
    {
        if (m_SkillClipAdapteer.GetPlaySchedule01() < GlobalConfig.Float1)
            return false;
        return true;
    }
    public override float GetUnitTime()
    {
        return m_SkillClipAdapteer.GetUnitTime();
    }
    public override float GetPlayTime()
    {
        return m_SkillClipAdapteer.GetPlayTime();
    }
    public override void ReExecuteCmd()
    {
        base.ReExecuteCmd();

        if (GetPlaySchedule01() < GlobalConfig.Float02)
            return;
        if (++m_Index >= m_ClipList.Count)
            return;
        if (m_Index == 2)
        {
            Entity3DMgr.Instance.SetEntityHeight(m_Graph.GetEntityID(), GlobalConfig.Int2, GlobalConfig.Int5);
        }
        DisconnectRootAdapter();
        var from = m_SkillClipAdapteer;
        from.Complete();
        var toAdapter = m_Graph.CreateClipPlayableAdapter(m_ClipList[m_Index]);
        m_SkillClipAdapteer = m_Graph.CreateMixerPlayableAdapter(from, toAdapter, GlobalConfig.Float02, MixerComplete);
        ConnectRootAdapter(m_SkillClipAdapteer);
    }
    private void MixerComplete(PlayableMixerAdapter mixer, PlayableAdapter frome, PlayableAdapter to)
    {
        DisconnectRootAdapter();
        mixer.DisconnectAll();
        PlayableAdapter.Destroy(frome);
        ConnectRootAdapter(to);
        PlayableAdapter.Destroy(mixer);
        m_SkillClipAdapteer = to;
    }
}
