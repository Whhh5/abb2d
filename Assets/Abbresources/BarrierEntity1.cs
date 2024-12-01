using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BarrierEntity1Data : BarrierEntityData
{
    private BarrierEntity1 m_BarrierEntity => m_Entity as BarrierEntity1;
    private int m_Num = -1;
    public int Num => m_Num;
    public override void OnPoolRecycle()
    {
        m_Num = -1;
        base.OnPoolRecycle();
    }
    public override void OnGODestroy()
    {
        base.OnGODestroy();
    }
    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
        PlayerMgr.Instance.AddNum(m_Num);
        BarrierEventMgr.Instance.DestroyBarrier(m_BarrierID);
    }
    public void SetNum(int num)
    {
        m_Num = num;
        if (m_IsLoadSuccess)
            m_BarrierEntity.SetNum();
    }

    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}
public class BarrierEntity1 : BarrierEntity
{
    private BarrierEntity1Data m_BarrierEntity1Data => m_EntityData as BarrierEntity1Data;
    [SerializeField]
    private TextMeshPro m_TMP = null;
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        SetNum();
    }

    public void SetNum()
    {
        m_TMP.text = $"{m_BarrierEntity1Data.Num}";
    }

    private void OnWillRenderObject()
    {
        var cameraPos = CameraMgr.Instance.GetCameraWorldPos();
        m_TMP.transform.LookAt(cameraPos);
    }
}
