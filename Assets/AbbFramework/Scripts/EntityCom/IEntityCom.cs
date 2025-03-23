using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Collections;

public interface IEntity3DComData : IClassPool
{
    public void OnCreateGO();
    public void OnDestroyGO();

    public bool IsCanActive();
    public bool IsActive();
    public void OnEnable();
    public void OnDisable();
}
public interface IEntity3DComData<T> : IEntity3DComData, IClassPoolInit<T>
    where T : class, IClassPoolUserData
{

}


public interface IEntity3DCom
{

}

public abstract class Entity3DComDataGO<TGOCom> : Entity3DComData
    where TGOCom : class, IEntity3DCom
{
    protected TGOCom _GoCom { get; private set; }
    public override void OnDestroyGO()
    {
        base.OnDestroyGO();
        _GoCom = null;
    }
    public override void OnCreateGO()
    {
        base.OnCreateGO();
        _GoCom = Entity3DMgr.Instance.GetEntityGOComponent<TGOCom>(_EntityID);
    }
}
public abstract class Entity3DComData : Entity3DComData<Entity3DComDataUserData>
{

}
public abstract class Entity3DComData<T> : IEntity3DComData<T>
    where T : Entity3DComDataUserData
{
    protected bool _IsActive { get; private set; } = false;
    protected int _EntityID { get; private set; } = -1;

    public virtual void OnPoolDestroy()
    {
        _EntityID = -1;
        _IsActive = false;
    }

    public virtual void OnPoolInit(T userData)
    {
        _EntityID = userData.entityID;
    }
    public bool IsActive()
    {
        return _IsActive;
    }

    public virtual void OnCreateGO()
    {

    }

    public virtual void OnDestroyGO()
    {

    }

    public virtual void OnDisable()
    {
        _IsActive = false;
    }

    public virtual void OnEnable()
    {
        _IsActive = true;
    }

    public bool IsCanActive()
    {
        return !_IsActive;
    }
}