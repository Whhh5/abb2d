using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class EntityMgr : Singleton<EntityMgr>
{
    private Dictionary<int, GameEntityData> m_EntityDataMap = new();

    public GameEntityData GetEntityData(int entityID)
    {
        if (!m_EntityDataMap.TryGetValue(entityID, out var entityData))
            return null;
        return entityData;
    }
    public T GetEntityData<T>(int entityID)
        where T : GameEntityData
    {
        var entityData = GetEntityData(entityID);
        if (entityData is not T tData)
            return null;
        return tData;
    }
    public bool IsValid(int entityID)
    {
        var result = m_EntityDataMap.ContainsKey(entityID);
        return result;
    }
    public int CreateEntityData<T>(IClassPoolUserData userData)
        where T : GameEntityData, new()
    {
        var entityID = ABBUtil.GetTempKey();
        var entityData = ClassPoolMgr.Instance.Pull<T>(userData);
        entityData.SetEntityID(entityID);
        entityData.SetLoadStatus(EnLoadStatus.Start);
        entityData.Create();
        entityData.OnEnable();
        m_EntityDataMap.Add(entityID, entityData);
        return entityID;
    }

    public int CreateEntityData<T>()
        where T : GameEntityData, new()
    {
        var entityID = CreateEntityData<T>(null);
        return entityID;
    }
    public void RecycleEntityData(int entityID)
    {
        if (!m_EntityDataMap.TryGetValue(entityID, out var entityData))
            return;
        if (entityData.IsLoadSuccess)
            UnloadEntity(entityID);
        m_EntityDataMap.Remove(entityID);
        if (entityData.GetActive())
            entityData.OnDisable();
        entityData.Destroy();
        ClassPoolMgr.Instance.Push(entityData);
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
        var goEntity = go.GetComponent<GameEntity>();
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
    public void OnDisableEntity(int entityID)
    {
        var entityData = GetEntityData(entityID);
        if (!entityData.GetActive())
            return;
        entityData.OnDisable();
    }
    public void OnEnableEntity(int entityID)
    {
        var entityData = GetEntityData(entityID);
        if (entityData.GetActive())
            return;
        entityData.OnEnable();
    }
}

