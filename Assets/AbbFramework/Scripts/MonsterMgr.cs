using System.Collections.Generic;
using UnityEngine;

public enum EnMonsterType
{

}
public class MonsterMgr : Singleton<MonsterMgr>
{
    private Dictionary<int, MonsterEntityData> m_EntityID2MonsterData = new();
    private bool GetMonsterEntityData(int entityID, out MonsterEntityData monsterData)
    {
        if (!m_EntityID2MonsterData.TryGetValue(entityID, out monsterData))
            return false;
        return true;
    }

    public int CreateMonster(int monsterID, Vector3 worldPOs)
    {
        var entityID = Entity3DMgr.Instance.CreateEntityData<MonsterEntityData>();
        var monsterData = Entity3DMgr.Instance.GetEntity3DData<MonsterEntityData>(entityID);
        monsterData.SetPosition(worldPOs);
        monsterData.AddEntityCom<EntityAnimComData>();
        monsterData.AddEntityCom<EntityBuffComData>();
        monsterData.AddEntityCom<EntityCCComData>();
        var animCom = monsterData.GetEntityCom<EntityAnimComData>();
        var ccCom = monsterData.GetEntityCom<EntityCCComData>();
        animCom.AddCmd(EnEntityCmd.Idle);
        m_EntityID2MonsterData.Add(entityID, monsterData);
        Entity3DMgr.Instance.LoadEntity(entityID);
        return entityID;
    }
    public void DestroyMonster(int entityID)
    {
        if (!GetMonsterEntityData(entityID, out var monsterData))
            return;
        m_EntityID2MonsterData.Remove(entityID);
        Entity3DMgr.Instance.UnloadEntity(entityID);
        Entity3DMgr.Instance.RecycleEntityData(entityID);
    }

}
