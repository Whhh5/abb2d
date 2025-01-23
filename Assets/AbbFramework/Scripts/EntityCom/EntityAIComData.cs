using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class EntityAIComDataUserData : IClassPoolUserData
{
    public void OnPoolDestroy()
    {

    }
}
public interface IEntityAICom : IEntity3DCom
{

}
public class EntityAIComData : IEntity3DComData<EntityAIComDataUserData>
{
    private IEntityAICom _AICom = null;
    public void OnCreateGO(Entity3D entity)
    {
        _AICom = entity;

        EntityAIMgr.Instance.Register(this);
    }

    public void OnDestroyGO()
    {
        EntityAIMgr.Instance.Unregister(this);
        _AICom = null;
    }

    public void OnPoolDestroy()
    {
    }

    public void OnPoolEnable()
    {
    }

    public void OnPoolInit(EntityAIComDataUserData userData)
    {
    }

    public void PoolConstructor()
    {
    }

    public void PoolRelease()
    {
    }
}