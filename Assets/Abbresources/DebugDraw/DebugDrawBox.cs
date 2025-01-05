using UnityEngine;


public abstract class DebugDrawData : Entity3DData, IUpdate
{
    private float m_Duration = -1;
    private float m_StartTime = -1;

    public override void Destroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.Destroy();
        m_Duration = -1;
        m_StartTime = -1;
    }
    public override void Create()
    {
        base.Create();

        m_StartTime = ABBUtil.GetGameTimeSeconds();

        UpdateMgr.Instance.Registener(this);
    }
    public void SetDurationTime(float durationTime)
    {
        m_Duration = durationTime;
    }

    public void Update()
    {
        if (ABBUtil.GetGameTimeSeconds() - m_StartTime < m_Duration)
            return;
        Entity3DMgr.Instance.RecycleEntityData(m_EntityID);
    }
}
public class DebugDrawBoxData : DebugDrawData, IUpdate
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_DrawBox;
}
public class DebugDrawBox : Entity3D
{

}
