using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;



public abstract class EntityData : IClassPool<EntityDataUserData>
{
    protected int m_EntityID;
    public int EntityID => m_EntityID;
    protected int m_GOID;
    public int GOID => m_GOID;
    protected bool m_IsLoadSuccess = false;
    public bool IsLoadSuccess => m_IsLoadSuccess;
    public int m_LoadKey = -1;
    public int LoadKey => m_LoadKey;
    public abstract EnLoadTarget LoadTarget { get; }
    private EnLoadStatus m_LoadStatus = EnLoadStatus.None;
    public EnLoadStatus LoadStatus => m_LoadStatus;
    private Vector3 m_WorldPos;
    public Vector3 WorldPos => m_WorldPos;
    public Vector3 Forward => m_Entity.GetForward();
    public Vector3 Up => m_Entity.GetUp();
    public Vector3 Right => m_Entity.GetRight();
    private Transform m_ParentTran = null;
    public Transform ParentTran => m_ParentTran;
    protected Entity m_Entity = null;
    public Entity EntityGO => m_Entity;
    public virtual void OnPoolDestroy()
    {
        m_EntityID
            = m_GOID
            = m_LoadKey
            = -1;
        m_IsLoadSuccess = false;
        m_LoadStatus = EnLoadStatus.None;
        m_WorldPos = Vector3.zero;
        m_ParentTran = null;
    }
    public virtual void OnPoolInit(EntityDataUserData userData)
    {

    }

    public void PoolConstructor()
    {
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }
    public virtual void Create()
    {

    }
    public virtual void Destroy()
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
    public void SetLoadKey(int loadKey)
    {
        m_LoadKey = loadKey;
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
    public T GetEntity<T>()
        where T : Entity
    {
        if (!m_IsLoadSuccess)
            return null;
        var entity = m_Entity.GetEntity<T>();
        return entity;
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
    public T GetEntity<T>()
        where T : Entity
    {
        return this as T;
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
    public virtual Vector3 GetForward()
    {
        return transform.forward;
    }
    public virtual Vector3 GetUp()
    {
        return transform.up;
    }
    public virtual Vector3 GetRight()
    {
        return transform.right;
    }
    protected virtual void Update()
    {

    }

    protected virtual void Awake()
    {
        
    }
}

