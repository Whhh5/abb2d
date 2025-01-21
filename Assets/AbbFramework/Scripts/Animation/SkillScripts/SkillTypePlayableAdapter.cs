using UnityEngine;
using UnityEngine.Playables;

public enum EnCmdStep
{
    None = -1,
    Step0,
    Step1,
    Step2,
    Step3,
    Step4,
    Step5,
    Step6,
    Step7,
}
public abstract class SkillTypePlayableAdapter : PlayableAdapter
{
    private EnEntityCmd m_EntityCmd = EnEntityCmd.None;
    private bool m_IsValid = false;
    protected override void OnDestroy()
    {
        base.OnDestroy();
        m_EntityCmd = EnEntityCmd.None;
    }
    public override void OnPoolInit(PlayableAdapterUserData userData)
    {
        base.OnPoolInit(userData);
        var data = userData as CmdPlayableAdapterUserData;
    }
    public virtual void ExecuteCmd()
    {
        m_IsValid = true;
    }
    public virtual void RemoveCmd()
    {
        m_IsValid = false;
    }
    public virtual void ReExecuteCmd()
    {

    }
    public virtual void CancelCmd()
    {

    }
    public virtual bool NextAnimLevelComdition()
    {
        return true;
    }
    public EnEntityCmd GetEntityCmd()
    {
        return m_EntityCmd;
    }
    public void SetEntityCmd(EnEntityCmd entityCmd)
    {
        m_EntityCmd = entityCmd;
    }
    public override bool OnPrepareFrame(Playable playable, FrameData info)
    {
        if (!base.OnPrepareFrame(playable, info))
            return false;
        return m_IsValid;
    }
}
