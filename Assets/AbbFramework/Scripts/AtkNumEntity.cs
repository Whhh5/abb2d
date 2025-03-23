using TMPro;
using UnityEngine;


public class AtkNumEntityData : Entity3DData<AtkNumEntity>, IUpdate
{
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_AtkNumEntity;
    public int NumValue { private set; get; }
    private float m_DurationTime = 1f;
    private float m_StartTime = -1;

    public override void Create()
    {
        base.Create();
        UpdateMgr.Instance.Registener(this);
        m_StartTime = ABBUtil.GetGameTimeSeconds() + m_DurationTime;
    }
    public override void Destroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.Destroy();
        NumValue
            = -1;
        m_StartTime
            = -1;
    }

    public void SetNumValue(int value)
    {
        NumValue = value;
        if (m_IsLoadSuccess)
            m_Entity3D.SetNumValue();
    }

    public void Update()
    {
        if (ABBUtil.GetGameTimeSeconds() - m_StartTime < m_DurationTime)
        {
            SetPosition(WorldPos + Vector3.up * 1 * ABBUtil.GetTimeDelta());
            return;
        }
        Entity3DMgr.Instance.RecycleEntityData(m_EntityID);
    }
}
public class AtkNumEntity : Entity3D<AtkNumEntityData>
{
    [SerializeField]
    private TextMeshPro m_NumTxt = null;

    public override void OnUnload()
    {
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        SetNumValue();
    }
    public void SetNumValue()
    {
        m_NumTxt.text = $"{m_Entity3DData.NumValue}";
    }

    protected override void Update()
    {
        base.Update();
        // m_BodyObj.transform.LookAt(CameraMgr.Instance.GetCameraWorldPos());
    }
}
