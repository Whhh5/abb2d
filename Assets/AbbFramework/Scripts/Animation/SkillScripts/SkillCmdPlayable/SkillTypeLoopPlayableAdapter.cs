using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class SkillTypeLoopPlayableAdapter : SkillTypePlayableAdapter
{
    private PlayableAdapter m_PlayableAdapter = null;
    //private int[] m_SkillList = null;
    private EnCmdStep m_StepIndex = EnCmdStep.None;
    private float m_LastExecuteTime = float.NaN;
    private int m_LoopCount = GlobalConfig.IntM1;

    private SkillTypeLoopData m_AttackLink = null;
    private SkillItemInfo CurAtkLinkItemData => m_AttackLink.GetData((int)m_StepIndex);

    public override EnAnimLayer GetOutputLayer()
    {
        return m_PlayableAdapter.GetOutputLayer();
    }
    protected override void OnDestroy()
    {
        PlayableAdapter.Destroy(m_PlayableAdapter);
        ClassPoolMgr.Instance.Push(m_AttackLink);
        base.OnDestroy();
        m_PlayableAdapter = null;
        //m_SkillList = null;
        m_StepIndex = EnCmdStep.None;
        m_LastExecuteTime = float.NaN;
        m_LoopCount = GlobalConfig.IntM1;
        m_AttackLink = null;
    }
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);
        var arrData = userData.customData as SkillTypeLinkPlayableAdapterCustomData;

        m_StepIndex = EnCmdStep.Step0;
        //m_SkillList = arrData.arrParams.Copy(0, 3);

        var skillData = ClassPoolMgr.Instance.Pull<AttackLinkSkillDataUserData>();
        skillData.arrParams = arrData.arrParams;
        m_AttackLink = ClassPoolMgr.Instance.Pull<SkillTypeLoopData>(skillData);
        ClassPoolMgr.Instance.Push(skillData);

        m_LastExecuteTime = ABBUtil.GetGameTimeSeconds();

        m_PlayableAdapter = m_Graph.CreateClipPlayableAdapter(CurAtkLinkItemData._ClipID);
        AddConnectRootAdapter(m_PlayableAdapter);
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
        m_AttackLink.OnEnable(m_Graph);
        CurAtkLinkItemData.OnEnable(m_Graph);
    }
    public override void RemoveCmd()
    {
        CurAtkLinkItemData.OnDisable(m_Graph);
        m_AttackLink.OnDisable(m_Graph);
        base.RemoveCmd();
    }
    public override bool NextAnimLevelComdition()
    {
        return m_StepIndex == EnCmdStep.Step1 ? false : GetPlaySchedule01() >= CurAtkLinkItemData.canNextTime;
    }
    public override float GetPlayTime()
    {
        return m_PlayableAdapter.GetPlayTime();
    }
    public override float GetUnitTime()
    {
        return m_PlayableAdapter.GetUnitTime();
    }

    public override bool IsPlayEnd()
    {
        if (!CurAtkLinkItemData._IsAutoRemove)
            return false;
        if (m_StepIndex == EnCmdStep.Step1)
            return false;
        return base.IsPlayEnd();
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
        if (m_StepIndex + GlobalConfig.Int1 >= (EnCmdStep)m_AttackLink.GetCount())
            return;
        if (m_StepIndex == EnCmdStep.Step1)
            return;
        if (GetPlaySchedule01() < GlobalConfig.Float095)
            return;

        CurAtkLinkItemData.OnDisable(m_Graph);
        m_StepIndex++;
        CurAtkLinkItemData.OnEnable(m_Graph);
        DisconnectRootAdapter();
        PlayableAdapter.Destroy(m_PlayableAdapter);
        m_PlayableAdapter = m_Graph.CreateClipPlayableAdapter(CurAtkLinkItemData._ClipID);
        ConnectRootAdapter(m_PlayableAdapter);
    }
    public override void CancelCmd()
    {
        base.CancelCmd();
        CmdEnd();
    }
    private void CmdEnd()
    {
        CurAtkLinkItemData.OnDisable(m_Graph);
        m_StepIndex++;
        CurAtkLinkItemData.OnEnable(m_Graph);
        var from = m_PlayableAdapter;
        var to = m_Graph.CreateClipPlayableAdapter(CurAtkLinkItemData._ClipID);
        from.Complete();
        DisconnectRootAdapter();
        m_PlayableAdapter = m_Graph.CreateMixerPlayableAdapter(from, to, GlobalConfig.Float02, MixerComplete);
        ConnectRootAdapter(m_PlayableAdapter);
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
            {
                CmdEnd();
            }
        }
        else
        {
            var curCRound = GetPlayCount();
            if (m_LoopCount != curCRound)
            {
                CurAtkLinkItemData.ResetAllItemData();
                m_LoopCount = curCRound;
            }
        }


        if (CurAtkLinkItemData.ScheduleEventCount > 0)
        {
            var curAttackItem = CurAtkLinkItemData.GetCurScheduleItem();
            var schedule = GetPlaySchedule() % GetUnitTime() / GetUnitTime();
            if (schedule > curAttackItem.GetEnterSchedule())
            {
                if (!curAttackItem.GetIsEffect())
                {
                    curAttackItem.Enter(m_Graph);
                    curAttackItem.SetIsEffect(true);
                    CurAtkLinkItemData.NextEventAction();
                }
            }
        }
        return true;
    }

}
