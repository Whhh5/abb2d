using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityMgr : Singleton<EntityMgr>
{
    private Dictionary<int, EntityData> m_EntityDataMap = new();

    public EntityData GetEntityData(int entityID)
    {
        if (!m_EntityDataMap.TryGetValue(entityID, out var entityData))
            return null;
        return entityData;
    }
    public T GetEntityData<T>(int entityID)
        where T: EntityData
    {
        var entityData = GetEntityData(entityID);
        if (entityData is not T tData)
            return null;
        return tData;
    }

    public int CreateEntityData<T>()
        where T: EntityData, new()
    {
        var entityID = ABBUtil.GetTempKey();
        var entityData = GameClassPoolMgr.Instance.Pull<T>();
        entityData.SetEntityID(entityID);
        entityData.SetLoadStatus(EnLoadStatus.Start);
        entityData.Create();
        m_EntityDataMap.Add(entityID, entityData);
        return entityID;
    }
    public void RecycleEntityData(int entityID)
    {
        if (!m_EntityDataMap.TryGetValue(entityID, out var entityData))
            return;
        if (entityData.IsLoadSuccess)
            UnloadEntity(entityID);
        m_EntityDataMap.Remove(entityID);
        entityData.Destroy();
        GameClassPoolMgr.Instance.Push(entityData);
    }
    public async void LoadEntity(int entityID)
    {
        var entityData = GetEntityData(entityID);
        if (entityData.LoadStatus != EnLoadStatus.Start)
            return;
        entityData.SetLoadStatus(EnLoadStatus.Loading);
        var loadKey = entityID;
        entityData.SetLoadKey(loadKey);
        var goID = await ABBGOMgr.Instance.CreateGOAsync(entityData.LoadTarget, entityData.ParentTran);
        if (goID < 0)
        {
            entityData.SetLoadStatus(EnLoadStatus.Failed);
            return;
        }
        if (loadKey != entityData.LoadKey)
        {
            ABBGOMgr.Instance.DestroyGO(goID);
            return;
        }
        entityData.SetLoadStatus(EnLoadStatus.Success);
        entityData.SetGOID(goID);
        entityData.SetIsLoadSuccess(true);
        entityData.OnGOCreate();
        var go = ABBGOMgr.Instance.GetGo(goID);
        var goEntity = go.GetComponent<Entity>();
        goEntity.SetEntityID(entityID);
        goEntity.LoadCompeletion();
    }
    public void UnloadEntity(int entityID)
    {
        var entityData = GetEntityData(entityID);
        if (entityData.IsLoadSuccess)
        {
            entityData.EntityGO.OnUnload();
            entityData.OnGODestroy();
            ABBGOMgr.Instance.DestroyGO(entityData.GOID);
            entityData.SetGOID(-1);
            entityData.SetIsLoadSuccess(false);
        }
        entityData.SetLoadStatus(EnLoadStatus.Start);
        entityData.SetLoadKey(-1);
    }
    public int EntityID2RoleID(int m_EntityID)
    {
        return 1;
    }
}

