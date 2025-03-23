using System.Collections.Generic;
using UnityEngine;

public enum EnMonsterType
{

}
public class MonsterMgr : Singleton<MonsterMgr>
{
    private Dictionary<int, MonsterEntityData> m_EntityID2MonsterData = new();
    private bool TryGetMonsterEntityData(int entityID, out MonsterEntityData monsterData)
    {
        if (!m_EntityID2MonsterData.TryGetValue(entityID, out monsterData))
            return false;
        return true;
    }

    public int CreateMonster(int monsterID, Vector3 worldPOs)
    {
        var entityID = Entity3DMgr.Instance.CreateMonsterEntityData<MonsterEntityData>(monsterID);
        var monsterData = Entity3DMgr.Instance.GetEntity3DData<MonsterEntityData>(entityID);
        monsterData.SetPosition(worldPOs);
        monsterData.AddEntityCom<EntityAnimComData>();
        monsterData.AddEntityCom<EntityCCComData>();
        monsterData.AddEntityCom<EntityBuffComData>();
        monsterData.AddMonition<EntityDirectionMonitorData>();
        var animCom = monsterData.GetEntityCom<EntityAnimComData>();
        animCom.AddCmd(EnEntityCmd.Monster0Idle);
        m_EntityID2MonsterData.Add(entityID, monsterData);
        Entity3DMgr.Instance.LoadEntity(entityID);


        { // 添加默认 ai 行为
            var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterID);
            if (monsterCfg.nAIID > 0)
            {
                var aiCfg = GameSchedule.Instance.GetAICfg0(monsterCfg.nAIID);
                var count = aiCfg.arrAIModuleID.Length;
                for (int i = 0; i < count; i++)
                {
                    var moduleID = aiCfg.arrAIModuleID[i];
                    EntityAIMgr.Instance.AddEntityAIModule(entityID, moduleID);
                }
            }
        }

        return entityID;
    }
    public void DestroyMonster(int entityID)
    {
        if (!TryGetMonsterEntityData(entityID, out var monsterData))
            return;
        EntityAIMgr.Instance.RemoveEntityAI(entityID);
        Entity3DMgr.Instance.UnloadEntity(entityID);
        Entity3DMgr.Instance.RecycleEntityData(entityID);
        m_EntityID2MonsterData.Remove(entityID);
    }

    public void ExecuteCmd(int entityID, EnEntityCmd cmd)
    {
        if (!TryGetMonsterEntityData(entityID, out var monsterData))
            return;
        var animCom = monsterData.GetEntityCom<EntityAnimComData>();
        animCom.AddCmd(cmd);
    }

}
