using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BarrierEntityData : EntityData
{
    private EnBarrierType m_BarrierType = EnBarrierType.None;
    public EnBarrierType BarrierType => m_BarrierType;
    protected int m_BarrierID = -1;
    public int BarrierID => m_BarrierID;

    public void SetBarrierID(int barrierID)
    {
        m_BarrierID = barrierID;
    }
    public void SetBarrierType(EnBarrierType barrierType)
    {
        m_BarrierType = barrierType;
    }
    public virtual void OnTriggerEnter(Collider other)
    {
        
    }
    public virtual void OnTriggerExit(Collider other)
    {

    }
}
public abstract class BarrierEntity : Entity
{
    private BarrierEntityData m_BarrierEntityData => m_EntityData as BarrierEntityData;

    public void OnTriggerEnter(Collider other)
    {
        m_BarrierEntityData.OnTriggerEnter(other);
        //ABBEventMgr.Instance.FireExecute(EnABBEvent.ENTITY_ONTRIGGER_ENTER,
            //(int)EnEntityType.Barrier, (int)m_BarrierEntityData.BarrierType, m_BarrierEntityData.BarrierID);
    }
    private void OnTriggerExit(Collider other)
    {
        m_BarrierEntityData.OnTriggerExit(other);
        //ABBEventMgr.Instance.FireExecute(EnABBEvent.ENTITY_ONTRIGGER_EXIT,
        //    (int)EnEntityType.Barrier, (int)m_BarrierEntityData.BarrierType, m_BarrierEntityData.BarrierID);
    }
}
