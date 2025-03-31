using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;


public class SkillTypeLoopPlayableAdapter : SkillTypePlayableAdapter
{
    private SkillTypeLoopData m_AttackLink = null;

    public override EnAnimLayer GetOutputLayer()
    {
        return m_AttackLink.GetOutputLayer();
    }
    protected override void OnDestroy()
    {
        ClassPoolMgr.Instance.Push(m_AttackLink);
        base.OnDestroy();
        m_AttackLink = null;
    }
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);
        var arrData = userData.customData as SkillTypeLinkPlayableAdapterCustomData;

        var skillData = ClassPoolMgr.Instance.Pull<AttackLinkSkillDataUserData>();
        skillData.arrParams = arrData.arrParams;
        m_AttackLink = ClassPoolMgr.Instance.Pull<SkillTypeLoopData>(skillData);
        ClassPoolMgr.Instance.Push(skillData);
        m_AttackLink.InitRuntime(this);
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
        m_AttackLink.OnEnable(m_Graph);
    }
    public override void RemoveCmd()
    {
        m_AttackLink.OnDisable(m_Graph);
        base.RemoveCmd();
    }
    public override bool NextAnimLevelComdition()
    {
        if (!m_AttackLink.NextAnimLevelComdition())
            return false;
        return true;
    }
    public override float GetPlayTime()
    {
        return m_AttackLink.GetPlayTime();
    }
    public override float GetUnitTime()
    {
        return m_AttackLink.GetUnitTime();
    }

    public override bool IsPlayEnd()
    {
        if (!m_AttackLink.CurIsPlayEnd())
            return false;
        return base.IsPlayEnd();
    }
    public override int GetPlayCount()
    {
        //return base.GetPlayCount();
        return m_AttackLink.GetPlayCount();
    }
    public override void ReExecuteCmd()
    {
        base.ReExecuteCmd();
        m_AttackLink.Reexcute();
    }
    public override void CancelCmd()
    {
        base.CancelCmd();
        m_AttackLink.CmdEnd();
    }

    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;
        m_AttackLink.Update();
        return true;
    }

}
