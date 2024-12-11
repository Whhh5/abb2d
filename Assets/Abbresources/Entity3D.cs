using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class Entity3DData: EntityData
{
    private Entity3D m_Entity3D = null;
    public Vector3 LocalRotation { get; private set; }
    public EnDirectionType DirType { get; private set; }

    private Dictionary<Type, IEntity3DComData> m_EntityComs = new();
    private Dictionary<Type, IEntityMonitorEntity> m_MonitorDic = new();

    public override void OnPoolRecycle()
    {
        LocalRotation = Vector3.zero;
        DirType = EnDirectionType.None;
        base.OnPoolRecycle();
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        m_Entity3D = m_Entity as Entity3D;
        foreach (var item in m_EntityComs)
            item.Value.OnCreateGO(m_Entity3D);
    }
    public override void OnGODestroy()
    {
        foreach (var item in m_EntityComs)
            item.Value.OnDestroyGO();
        m_EntityComs.Clear();
        foreach (var item in m_MonitorDic)
            item.Value.StopMonitor();
        m_MonitorDic.Clear();
        m_Entity3D = null;
        base.OnGODestroy();
    }

    #region coms
    public T GetEntityCom<T>()
        where T : class, IEntity3DComData
    {
        var type = typeof(T);
        if (!m_EntityComs.TryGetValue(type, out var entityCom))
            return default;
        return entityCom as T;
    }
    public bool AddEntityCom<T>()
        where T : class, IEntity3DComData, new()
    {
        var type = typeof(T);
        if (m_EntityComs.TryGetValue(type, out var entityCom))
            return false;
        entityCom = GameClassPoolMgr.Instance.Pull<T>();
        m_EntityComs.Add(type, entityCom);
        entityCom.AddCom(this);
        if (m_IsLoadSuccess)
            entityCom.OnCreateGO(m_Entity3D);
        return true;
    }
    public bool RemoveEntityCom<T>()
        where T : class, IEntity3DComData, new()
    {
        var type = typeof(T);
        if (!m_EntityComs.TryGetValue(type, out var entityCom))
            return false;
        m_EntityComs.Remove(type);
        if (m_IsLoadSuccess)
            entityCom.OnDestroyGO();
        entityCom.RemomveCom();
        GameClassPoolMgr.Instance.Push(entityCom);
        return true;
    }
    public bool ContainsEntityCom<T>()
        where T : class, IEntity3DComData
    {
        var type = typeof(T);
        if (!m_EntityComs.ContainsKey(type))
            return false;
        return true;
    }
    #endregion

    #region monitor
    public bool AddMonition<T>()
        where T : class, IEntityMonitorEntity, new()
    {
        var type = typeof(T);
        if (m_MonitorDic.ContainsKey(type))
            return false;
        var monitor = GameClassPoolMgr.Instance.Pull<T>();
        m_MonitorDic.Add(type, monitor);
        monitor.StartMonitor(this);
        return true;
    }
    public bool RemoveMonitor<T>()
        where T : IEntityMonitorEntity
    {
        var type = typeof(T);
        if (!m_MonitorDic.TryGetValue(type, out var monitor))
            return false;
        m_MonitorDic.Remove(type);
        monitor.StopMonitor();
        GameClassPoolMgr.Instance.Push(monitor);
        return true;
    }
    #endregion


    public void SetDirType(EnDirectionType dirType)
    {
        DirType = dirType;
        if (m_IsLoadSuccess)
            m_Entity3D.SetDirType();
    }
    public void SetLocalRotation(Vector3 localRotation)
    {
        LocalRotation = localRotation;
        if (m_IsLoadSuccess)
            m_Entity3D.SetLocalRotation();
    }
}
public abstract class Entity3D : Entity
{
    private Entity3DData m_Entity3DData = null;
    [SerializeField]
    private GameObject m_BodyObj = null;

    public override void LoadCompeletion()
    {
        m_Entity3DData = m_EntityData as Entity3DData;
        base.LoadCompeletion();
        SetDirType();
    }

    public virtual void SetLocalRotation()
    {
        m_BodyObj.transform.localEulerAngles = m_Entity3DData.LocalRotation;
    }
    public void SetDirType()
    {
        var dirType = m_Entity3DData.DirType;
        switch (dirType)
        {
            case EnDirectionType.Left:
                m_Entity3DData.SetLocalRotation(Vector3.up * -90);
                break;
            case EnDirectionType.Right:
                m_Entity3DData.SetLocalRotation(Vector3.up * 90);
                break;
            case EnDirectionType.Forward:
                m_Entity3DData.SetLocalRotation(Vector3.up * 0);
                break;
            case EnDirectionType.back:
                m_Entity3DData.SetLocalRotation(Vector3.up * 180);
                break;
            default:
                break;
        }
        
    }
}
