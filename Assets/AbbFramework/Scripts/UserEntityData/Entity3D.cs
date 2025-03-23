using System;
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
public abstract class Entity3DData : GameEntityData
{
    private Entity3D m_Entity3D = null;
    public Vector3 MoveDirection { get; private set; } = Vector3.zero;

    public override Vector3 LocalRotation => MoveDirection;

    private Dictionary<Type, IEntity3DComData> m_EntityComs = new();
    private Dictionary<Type, IEntityMonitorEntity> m_MonitorDic = new();


    public override Vector3 GetForward()
    {
        //return base.GetForward();
        var result = Quaternion.Euler(MoveDirection) * Vector3.forward;
        return result;
    }
    public override void Destroy()
    {
        base.Destroy();
        MoveDirection = Vector3.forward;
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
            item.Value.OnCreateGO();
    }
    public override void OnGODestroy()
    {
        foreach (var item in m_EntityComs)
        {
            if (item.Value.IsActive())
                item.Value.OnDisable();
            item.Value.OnDestroyGO();
        }
        m_EntityComs.Clear();
        foreach (var item in m_MonitorDic)
            item.Value.StopMonitor();
        m_MonitorDic.Clear();
        m_Entity3D = null;
        base.OnGODestroy();
    }

    public override void SetLocalRotation(Vector3 localRotation)
    {
        //base.SetLocalRotation(localRotation);
        MoveDirection = localRotation;
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
        var data = ClassPoolMgr.Instance.Pull<Entity3DComDataUserData>();
        data.entityID = m_EntityID;
        var entityCom = ClassPoolMgr.Instance.Pull<T>(data);
        ClassPoolMgr.Instance.Push(data);
        m_EntityComs.Add(type, entityCom);
        if (m_IsLoadSuccess)
            entityCom.OnCreateGO();
        if (entityCom.IsCanActive())
            entityCom.OnEnable();
        return true;
    }
    public bool RemoveEntityCom<T>()
        where T : class, IEntity3DComData, new()
    {
        var type = typeof(T);
        if (!m_EntityComs.TryGetValue(type, out var entityCom))
            return false;
        m_EntityComs.Remove(type);
        if (entityCom.IsActive())
            entityCom.OnDisable();
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

    public void SetMoveDirection(Vector3 value)
    {
        MoveDirection += value;
    }
    public void UpdateRotation(float timeDelta)
    {
        if (m_IsLoadSuccess)
            m_Entity3D.UpdateRotation(timeDelta);
    }

    public Vector3 GetTopPoint()
    {
        return WorldPos + new Vector3(0, 2, 0);
    }
}
public abstract class Entity3D<T> : Entity3D
    where T : Entity3DData
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
public abstract class Entity3D : GameEntity, IEntityAnimCom
{
    public static implicit operator int(Entity3D entity3D)
    {
        return entity3D.m_Entity3DData.EntityID;
    }
    private Entity3DData m_Entity3DData = null;

    private Animator m_Anim = null;

    private Vector3 _CurRot = Vector3.zero;
    protected override void Awake()
    {
        base.Awake();
        m_Anim = GetComponent<Animator>();
    }
    public override void LoadCompeletion()
    {
        m_Entity3DData = m_EntityData as Entity3DData;
        base.LoadCompeletion();
        _CurRot = m_Entity3DData.LocalRotation;
    }

    public void UpdateRotation(float timeDelta)
    {
        var curQua = Quaternion.Euler(_CurRot);
        var loogAt = Quaternion.Euler(m_Entity3DData.MoveDirection);
        var angle = Quaternion.SlerpUnclamped(curQua, loogAt, 10 * timeDelta);
        //m_Entity3DData.SetLocalRotation(angle.eulerAngles);
        //base.SetLocalRotation();
        transform.localRotation = angle;
        _CurRot = angle.eulerAngles;
    }
    public Animator GetAnimator()
    {
        return m_Anim;
    }
    protected override void Update()
    {
        base.Update();

        UpdateRotation(ABBUtil.GetTimeDelta());

    }
}
