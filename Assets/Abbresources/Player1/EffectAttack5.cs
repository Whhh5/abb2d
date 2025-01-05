using UnityEngine;

public class EffectAttack5Data : Entity3DData, IUpdate
{
    private EffectAttack5 m_EffectAttack5 = null;
    public override EnLoadTarget LoadTarget => EnLoadTarget.Pre_EffectAttack5;
    private float m_FlyDistance = -1;
    private float m_FlyTime = -1;
    private float m_StartTime = -1;
    public Vector3 TargetLocalPos = Vector3.zero;
    private float m_LastTime = -1;
    public override void Destroy()
    {
        UpdateMgr.Instance.Unregistener(this);
        base.Destroy();
        m_FlyDistance = -1;
        m_LastTime = -1;
        TargetLocalPos = Vector3.zero;
    }

    public void SetParams(int[] arrParams)
    {
        m_FlyTime = arrParams[0] / 100f;
        m_FlyDistance = arrParams[1] / 100f;

        m_StartTime
            = m_LastTime
            = ABBUtil.GetGameTimeSeconds();
        UpdateMgr.Instance.Registener(this);
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        m_EffectAttack5 = m_Entity as EffectAttack5;
    }

    public void Update()
    {
        if(m_LastTime - m_StartTime > m_FlyTime)
        {
            Entity3DMgr.Instance.RecycleEntityData(m_EntityID);
            return;
        }

        m_LastTime = ABBUtil.GetGameTimeSeconds();
        var time = m_LastTime - m_StartTime;
        var timeDelta = Mathf.Clamp01(time / m_FlyTime);
        var vlaue = Mathf.Lerp(0, m_FlyDistance, timeDelta);
        TargetLocalPos = Vector3.forward * vlaue;
        if (m_IsLoadSuccess)
            m_EffectAttack5.SetTargetLocalPos();
    }
}

public class EffectAttack5 : Entity3D
{
    private EffectAttack5Data m_EffectAttack5Data = null;
    [SerializeField]
    private Transform TargetLocalPos = null;
    public override void OnUnload()
    {
        base.OnUnload();
        m_EffectAttack5Data = null;
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        m_EffectAttack5Data = m_EntityData as EffectAttack5Data;
        SetTargetLocalPos();
    }
    public void SetTargetLocalPos()
    {
        TargetLocalPos.localPosition = m_EffectAttack5Data.TargetLocalPos;
    }
}
