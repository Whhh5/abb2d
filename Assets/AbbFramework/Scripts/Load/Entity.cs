using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class EntityData : IGamePool
{
    protected int m_EntityID;
    public int EntityID => m_EntityID;
    protected int m_GOID;
    public int GOID => m_GOID;
    protected bool m_IsLoadSuccess = false;
    public bool IsLoadSuccess => m_IsLoadSuccess;
    public abstract EnLoadTarget LoadTarget { get; }
    private EnLoadStatus m_LoadStatus = EnLoadStatus.None;
    public EnLoadStatus LoadStatus => m_LoadStatus;
    private Vector3 m_WorldPos;
    public Vector3 WorldPos => m_WorldPos;
    private Transform m_ParentTran = null;
    public Transform ParentTran => m_ParentTran;
    protected Entity m_Entity = null;
    public Entity EntityGO => m_Entity;
    public virtual void OnPoolRecycle()
    {
        m_EntityID
            = m_GOID
            = -1;
        m_IsLoadSuccess = false;
        m_LoadStatus = EnLoadStatus.None;
        m_WorldPos = Vector3.zero;
        m_ParentTran = null;
    }
    public virtual void OnPoolGet()
    {

    }

    public virtual void OnGOCreate()
    {
        var go = ABBGOMgr.Instance.GetGo(m_GOID);
        m_Entity = go.GetComponent<Entity>();
    }
    public virtual void OnGODestroy()
    {
        m_Entity = null;
    }


    public virtual void SetEntityID(int entityID)
    {
        m_EntityID = entityID;
    }
    public void SetLoadStatus(EnLoadStatus loadStatus)
    {
        m_LoadStatus = loadStatus;
    }
    public virtual void SetGOID(int goID)
    {
        m_GOID = goID;
    }
    public void SetIsLoadSuccess(bool isloadSuccess)
    {
        m_IsLoadSuccess = isloadSuccess;
    }
    public void SetPosition(Vector3 worldPos)
    {
        m_WorldPos = worldPos;
        if (m_IsLoadSuccess)
            m_Entity.SetPosition();
    }
    public void SetParentTran(Transform parentTran)
    {
        m_ParentTran = parentTran;
        if (m_IsLoadSuccess)
            m_Entity.SetParentTran();
    }
}

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
    }
    public virtual void OnUnload()
    {
        m_EntityData = null;
    }


    public virtual void SetPosition()
    {
        transform.position = m_EntityData.WorldPos;
    }
    public void SetParentTran()
    {
        transform.SetParent(m_EntityData.ParentTran);
    }

}

