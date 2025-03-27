using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;


public abstract class GameEntityData<T>: GameEntityData
    where T: GameEntity
{
    protected T _GameEntity = null;
    public override void OnGODestroy()
    {
        _GameEntity = null;
        base.OnGODestroy();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        _GameEntity = m_Entity as T;
    }
}
public abstract class GameEntityData : IClassPool<IClassPoolUserData>
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
    public virtual Vector3 LocalRotation { get; private set; }
    public Vector3 LocalScale { get; private set; } = Vector3.one;
    public Vector3 Up => m_Entity.GetUp();
    public Vector3 Right => m_Entity.GetRight();
    private Transform m_ParentTran = null;
    public Transform ParentTran => m_ParentTran;
    protected GameEntity m_Entity = null;
    public GameEntity EntityGO => m_Entity;
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
    public virtual void OnPoolInit(IClassPoolUserData userData)
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
        LocalScale = Vector3.one;
        LocalRotation = Vector3.zero;
    }

    public virtual void OnGOCreate()
    {
        var go = ABBGOMgr.Instance.GetGo(m_GOID);
        m_Entity = go.GetComponent<GameEntity>();
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
    public Transform GetTransfrom()
    {
        if (!m_IsLoadSuccess)
            return null;
        return m_Entity.transform;
    }
    public T GetEntityComponent<T>()
        where T : class
    {
        if (!m_IsLoadSuccess)
            return null;
        var entity = m_Entity.GetEntity<T>();
        return entity;
    }
    public virtual void SetLocalRotation(Vector3 localRotation)
    {
        LocalRotation = localRotation;
        if (m_IsLoadSuccess)
            m_Entity.SetLocalRotation();
    }
    public void SetLocalScale(Vector3 localScale)
    {
        LocalScale = localScale;
        if (m_IsLoadSuccess)
            m_Entity.SetLocalScale();
    }
    public virtual Vector3 GetForward()
    {
        var forword = Quaternion.Euler(LocalRotation) * Vector3.forward;
        return forword;
    }
}
public abstract class GameEntity<T> : GameEntity
    where T: GameEntityData
{
    protected T _GameEntityData = null;
    public override void OnUnload()
    {
        _GameEntityData = null;
        base.OnUnload();
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        _GameEntityData = m_EntityData as T;
    }
}
public abstract class GameEntity : MonoBehaviour
{
    private int m_EntityID;
    protected GameEntityData m_EntityData = null;

    public void SetEntityID(int entityDataID)
    {
        var entityData = EntityMgr.Instance.GetEntityData(entityDataID);
        m_EntityData = entityData;
        m_EntityID = entityDataID;
    }
    public int GetEntityID()
    {
        return m_EntityID;
    }
    public T GetEntity<T>()
        where T : class
    {
        if (!this.TryGetComponent<T>(out var com))
            return null;
        return com;
    }
    public virtual void LoadCompeletion()
    {
        SetParentTran();
        SetPosition();
        SetLocalRotation();
        SetLocalScale();
    }
    public virtual void SetLocalRotation()
    {
        transform.localEulerAngles = m_EntityData.LocalRotation;
    }
    public virtual void SetLocalScale()
    {
        transform.localScale = m_EntityData.LocalScale;
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

    protected virtual void OnEnable()
    {

    }
    protected virtual void OnDisable()
    {

    }
}

