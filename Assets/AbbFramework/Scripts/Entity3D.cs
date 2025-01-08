using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Entity3DData<T> : Entity3DData
    where T : Entity3D
{
    protected T m_Entity3D = null;
    public override void OnGODestroy()
    {
        base.OnGODestroy();
        m_Entity3D = null;
    }
    public override void OnGOCreate()
    {
        base.OnGOCreate();
        m_Entity3D = m_Entity as T;
    }
}
public abstract class Entity3DData : EntityData
{
    private Entity3D m_Entity3D = null;
    public Vector3 LocalRotation { get; private set; }
    public Vector3 LocalScale { get; private set; } = Vector3.one;
    public Vector3 DirType { get; private set; }

    private Dictionary<Type, IEntity3DComData> m_EntityComs = new();
    private Dictionary<Type, IEntityMonitorEntity> m_MonitorDic = new();


    public override void Destroy()
    {
        base.Destroy();
        LocalScale = Vector3.one;
        LocalRotation = Vector3.zero;
        DirType = Vector3.zero;
    }
    public override void Create()
    {
        base.Create();
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
        if (m_EntityComs.ContainsKey(type))
            return false;
        var data = new Entity3DComDataUserData()
        {
            entity3DData = this,
        };
        var entityCom = ClassPoolMgr.Instance.Pull<T, Entity3DComDataUserData>(ref data);
        m_EntityComs.Add(type, entityCom);
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
        entityCom.OnPoolDestroy();
        ClassPoolMgr.Instance.Push(entityCom);
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
        var monitor = ClassPoolMgr.Instance.Pull<T>();
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
        ClassPoolMgr.Instance.Push(monitor);
        return true;
    }
    #endregion


    public bool IsCanMove()
    {
        //var buffCom = GetEntityCom<Entity3DBuffComData>();
        var animCom = GetEntityCom<EntityCCComData>();
        return animCom.GetMoveStatus();
    }
    public bool IsCanJump()
    {
        //var animCom = GetEntityCom<EntityAnimComData>();
        var animCom = GetEntityCom<EntityCCComData>();
        return animCom.GetJumpStatus();
    }

    public void SetDirType(Vector3 dirType)
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
    public void SetLocalScale(Vector3 localScale)
    {
        LocalScale = localScale;
        if (m_IsLoadSuccess)
            m_Entity3D.SetLocalScale();
    }
    public void UpdateRotation(float timeDelta)
    {
        if (m_IsLoadSuccess)
            m_Entity3D.UpdateRotation(timeDelta);
    }
}
public abstract class Entity3D<T>: Entity3D
    where T: Entity3DData
{
    protected T m_Entity3DData = null;
    public override void OnUnload()
    {
        base.OnUnload();
        m_Entity3DData = null;
    }
    public override void LoadCompeletion()
    {
        base.LoadCompeletion();
        m_Entity3DData = m_EntityData as T;
    }
}
public abstract class Entity3D : Entity
{
    public static implicit operator int(Entity3D entity3D)
    {
        return entity3D.m_Entity3DData.EntityID;
    }
    private Entity3DData m_Entity3DData = null;
    [SerializeField]
    protected GameObject m_BodyObj = null;
    private Quaternion m_TargetRotation;
    private Transform m_MainBodyTran = null;

    protected override void Awake()
    {
        base.Awake();
        m_MainBodyTran= m_BodyObj == null ? transform : m_BodyObj.transform;
    }
    public override void LoadCompeletion()
    {
        m_Entity3DData = m_EntityData as Entity3DData;
        base.LoadCompeletion();
        SetDirType();
        SetLocalRotation();
        SetLocalScale();
    }

    public virtual void SetLocalRotation()
    {
        m_MainBodyTran.localEulerAngles = m_Entity3DData.LocalRotation;
    }
    public virtual void SetLocalScale()
    {
        m_MainBodyTran.localScale = m_Entity3DData.LocalScale;
    }
    public void SetDirType()
    {
        var dirType = m_Entity3DData.DirType;
        m_TargetRotation = ABBUtil.Dir2Quaternion(dirType);
    }
    public override Vector3 GetForward()
    {
        return m_MainBodyTran.forward;
    }
    protected override void Update()
    {
        base.Update();

    }
    public override void SetPosition()
    {
        base.SetPosition();

    }
    public void UpdateRotation(float timeDelta)
    {
        var curQua = Quaternion.Euler(m_Entity3DData.LocalRotation);
        var angle = Quaternion.SlerpUnclamped(curQua, m_TargetRotation, 10 * timeDelta);
        m_Entity3DData.SetLocalRotation(angle.eulerAngles);
    }
}
