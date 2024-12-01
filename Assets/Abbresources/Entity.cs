using UnityEngine;
using System.Collections;
using System.Xml;

public abstract class Entity : MonoBehaviour
{
	private int m_EntityID;
    protected EntityData m_EntityData = null;
    public void SetEntityID(int entityDataID)
    {
        var entityData = EntityMgr.Instance.GetEntityData(entityDataID);
        m_EntityData = entityData;
    }
    public virtual void LoadCompeletion()
    {
        SetPosition();
        SetParentTran();
    }
    public virtual void OnUnload()
    {

    }


    public void SetPosition()
    {
        transform.position = m_EntityData.WorldPos;
    }
    public void SetParentTran()
    {
        transform.SetParent(m_EntityData.ParentTran);
    }

}

