using UnityEngine;
using UnityEngine.Playables;
using static UnityEngine.Analytics.IAnalytic;


public class SkillTypeLinkPlayableAdapterCustomData : IPlayableAdapterCustomData
{
    public int[] arrParams;

    public void OnPoolDestroy()
    {
        arrParams = null;
    }
}
public class SkillTypeLinkPlayableAdapter : SkillTypePlayableAdapter
{
    private SkillTypeLinkData m_LinkData = null;

    protected override void OnDestroy()
    {
        ClassPoolMgr.Instance.Push(m_LinkData);
        m_LinkData = null;

        base.OnDestroy();

    }
    public override bool IsPlayEnd()
    {
        if (!m_LinkData.GetIsAutoRemove())
            return false;
        return base.IsPlayEnd();
    }
    public override float GetPlayTime()
    {
        return m_LinkData.GetPlayTime();
    }
    public override float GetUnitTime()
    {
        return m_LinkData.GetUnitTime();
    }
    public override EnAnimLayer GetOutputLayer()
    {
        return m_LinkData.GetOutputLayer();
    }
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);
        var data = userData.customData as SkillTypeLinkPlayableAdapterCustomData;

        var skillData = ClassPoolMgr.Instance.Pull<AttackLinkSkillDataUserData>();
        skillData.arrParams = data.arrParams;
        m_LinkData = ClassPoolMgr.Instance.Pull<SkillTypeLinkData>(skillData);
        ClassPoolMgr.Instance.Push(skillData);

        m_LinkData.InitRuntme(this);
    }
    public override bool NextAnimLevelComdition()
    {
        return GetPlaySchedule01() > m_LinkData.GetCanNextTime();
    }
    public override void ExecuteCmd()
    {
        base.ExecuteCmd();
        m_LinkData.OnEnable(m_Graph);
    }
    public override void RemoveCmd()
    {
        m_LinkData.OnDisable(m_Graph);

        base.RemoveCmd();
    }
    public override void ReExecuteCmd()
    {
        base.ReExecuteCmd();

        m_LinkData.ReExecuteCmd();
    }




    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;

        m_LinkData.Update();
        return true;
    }

}
