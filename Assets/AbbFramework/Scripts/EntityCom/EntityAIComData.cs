using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.EventSystems.EventTrigger;


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
    public void OnCreateGO(int entityID)
    {
        var entityData = Entity3DMgr.Instance.GetEntity3DData(entityID);
        var entityMono = entityData.GetEntity<Entity3D>();
        _AICom = entityMono;

        EntityAIMgr.Instance.Register(entityID);
    }

    public void OnDestroyGO(int entityID)
    {
        EntityAIMgr.Instance.Unregister(entityID);
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