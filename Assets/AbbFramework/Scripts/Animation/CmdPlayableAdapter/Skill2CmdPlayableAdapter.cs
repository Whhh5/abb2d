using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class Skill2CmdPlayableAdapter : CmdPlayableAdapter
{
    private PlayableAdapter m_PlayableAdapter = null;
    private List<int> m_SkillList = null;
    private EnCmdStep m_StepIndex = EnCmdStep.None;
    private float m_LastExecuteTime = float.NaN;
    private int m_LoopCount = GlobalConfig.IntM1;
    private bool m_IsAddBuff = false;
    private float m_LastAttackTime = -1;
    private float m_AtkInterval = 0.1f;
    private float m_AtkRadius = 2f;
    protected override void OnDestroy()
    {
        PlayableAdapter.Destroy(m_PlayableAdapter);
        base.OnDestroy();
        m_PlayableAdapter = null;
        m_SkillList = null;
        m_StepIndex = EnCmdStep.None;
        m_LastExecuteTime = float.NaN;
        m_LoopCount = GlobalConfig.IntM1;
        m_LastAttackTime = -1;
    }
    protected override void Initialization(PlayableGraphAdapter graph, IPlayableAdapterCustomData customData)
    {
        base.Initialization(graph, customData);
        var roleID = Entity3DMgr.Instance.EntityID2RoleID(graph.GetEntityID());
        m_SkillList = AnimMgr.Instance.GetSkill2AnimClipList(roleID);
        m_StepIndex = EnCmdStep.Step0;
        m_LastExecuteTime = ABBUtil.GetGameTimeSeconds();
        m_PlayableAdapter = graph.CreateClipPlayableAdapter(m_SkillList[(int)m_StepIndex]);
        AddConnectRootAdapter(m_PlayableAdapter);
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
        var param = GameClassPoolMgr.Instance.Pull<EntityMoveDownBuffParams>();
        param.value = -0.5f;
        Entity3DMgr.Instance.AddEntityBuff(m_Graph, EnBuff.MoveDown, param);
        Entity3DMgr.Instance.AddEntityBuff(m_Graph, EnBuff.NoRotation);
    }
    public override void RemoveCmd()
    {
        Entity3DMgr.Instance.RemoveEntityBuff(m_Graph, EnBuff.MoveDown);
        Entity3DMgr.Instance.RemoveEntityBuff(m_Graph, EnBuff.NoRotation);
        if (m_IsAddBuff)
        {
            Entity3DMgr.Instance.RemoveEntityBuff(m_Graph, EnBuff.NoMove);
            Entity3DMgr.Instance.RemoveEntityBuff(m_Graph, EnBuff.NoJump);
            m_IsAddBuff = false;
        }
        base.RemoveCmd();
    }
    public override bool NextAnimLevelComdition()
    {
        return GetPlaySchedule01() >= GlobalConfig.Float1;
    }
    public override float GetPlayTime()
    {
        return m_StepIndex == EnCmdStep.Step1 ? GlobalConfig.Int0 : m_PlayableAdapter.GetPlayTime();
    }
    public override float GetUnitTime()
    {
        return m_PlayableAdapter.GetUnitTime();
    }
    public override int GetPlayCount()
    {
        //return base.GetPlayCount();
        var time = m_PlayableAdapter.GetPlayTime() / m_PlayableAdapter.GetUnitTime();
        return Mathf.FloorToInt(time);
    }
    public override void ReExecuteCmd()
    {
        base.ReExecuteCmd();

        m_LastExecuteTime = ABBUtil.GetGameTimeSeconds();
        if (m_StepIndex + GlobalConfig.Int1 >= (EnCmdStep)m_SkillList.Count)
            return;
        if (m_StepIndex == EnCmdStep.Step1)
            return;
        if (GetPlaySchedule01() < GlobalConfig.Float095)
            return;
        m_StepIndex++;

        DisconnectRootAdapter();
        PlayableAdapter.Destroy(m_PlayableAdapter);
        m_PlayableAdapter = m_Graph.CreateClipPlayableAdapter(m_SkillList[(int)m_StepIndex]);
        ConnectRootAdapter(m_PlayableAdapter);
    }
    public override void CancelCmd()
    {
        base.CancelCmd();
        CmdEnd();
    }
    private void CmdEnd()
    {
        m_StepIndex++;
        var from = m_PlayableAdapter;
        var to = m_Graph.CreateClipPlayableAdapter(m_SkillList[(int)m_StepIndex]);
        from.Complete();
        DisconnectRootAdapter();
        m_PlayableAdapter = m_Graph.CreateMixerPlayableAdapter(from, to, GlobalConfig.Float02, MixerComplete);
        ConnectRootAdapter(m_PlayableAdapter);
        Entity3DMgr.Instance.AddEntityBuff(m_Graph, EnBuff.NoMove);
        Entity3DMgr.Instance.AddEntityBuff(m_Graph, EnBuff.NoJump);
        m_IsAddBuff = true;
    }
    private void MixerComplete(PlayableMixerAdapter mixer, PlayableAdapter from, PlayableAdapter to)
    {
        mixer.DisconnectAll();
        DisconnectRootAdapter();
        PlayableAdapter.Destroy(from);
        PlayableAdapter.Destroy(mixer);
        ConnectRootAdapter(to);
        m_PlayableAdapter = to;
    }
    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;
        if (m_StepIndex == EnCmdStep.Step1
            && ABBUtil.GetGameTimeSeconds() - m_LastExecuteTime > GlobalConfig.Float02)
        {
            if (m_LoopCount != GetPlayCount())
                CmdEnd();
        }
        else
        {
            m_LoopCount = GetPlayCount();
        }
        if (m_StepIndex == EnCmdStep.Step1)
        {
            var curTime = ABBUtil.GetGameTimeSeconds();
            if (curTime - m_LastAttackTime > m_AtkInterval)
            {
                m_LastAttackTime = curTime;
                var pos = Entity3DMgr.Instance.GetEntityWorldPos(m_Graph);
                var arrHit = Physics.OverlapSphere(pos, m_AtkRadius, 1 << (int)EnGameLayer.Monster);
                foreach (var hit in arrHit)
                {
                    var com = hit.GetComponent<Entity3D>();
                    AttackMgr.Instance.AttackEntity(m_Graph, com, 100);
                    Debug.DrawLine(pos, hit.ClosestPoint(pos), Color.red, 1);
                }
                DebugDrawMgr.Instance.DrawSphere(pos, m_AtkRadius, 0.1f);
            }
        }
        return true;
    }
}
