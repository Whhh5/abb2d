using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class BarrierEntity2Data : BarrierEntityData
{
    private BarrierEntity2 m_BarrierEntity2 => m_Entity as BarrierEntity2;
    private int m_Num = -1;
    public int Num => m_Num;

    public void SetNum(int num)
    {
        m_Num = num;
        if (m_IsLoadSuccess)
            m_BarrierEntity2.SetNum();
    }

    public override void OnTriggerEnter(Collider other)
    {
        base.OnTriggerEnter(other);
    }
    public override void OnTriggerExit(Collider other)
    {
        base.OnTriggerExit(other);
    }
}
public class BarrierEntity2 : BarrierEntity
{
    private BarrierEntity2Data m_BarrierEntity2Data => m_EntityData as BarrierEntity2Data;
    [SerializeField]
    private TextMeshPro m_TMP = null;

    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        SetNum();
    }

    public void SetNum()
    {
        m_TMP.text = $"{m_BarrierEntity2Data.Num}";
    }

    private void OnWillRenderObject()
    {
        var cameraPos = CameraMgr.Instance.GetCameraWorldPos();
        m_TMP.transform.LookAt(cameraPos);
    }
}
