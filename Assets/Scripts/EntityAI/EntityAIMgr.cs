
using System.Collections.Generic;
using Unity.Entities.UniversalDelegates;

public class EntityAIMgr : Singleton<EntityAIMgr>
{
    #region Struct
    public enum EnAIModuleType
    {
        None = 0,
        RangeMove = 1,
        Idle = 2,
        MonsterBattle,
        Pastor,
        BlueWarrior,
    }
    public class EntityAIInfo : IClassPoolDestroy
    {
        private Dictionary<int, List<int>> _AIModuleIDDic = new();
        private List<int> _SortModuleCfgIDList = new();
        public int Count => _AIModuleIDDic.Count;
        public void AddAIModuleData(int moduleCfgID, int aiModuleID)
        {
            if (!_AIModuleIDDic.TryGetValue(moduleCfgID, out var moduleList))
            {
                moduleList = new(1);
                _AIModuleIDDic.Add(moduleCfgID, moduleList);

                var level = GameSchedule.Instance.GetAIModuleCfg0(moduleCfgID);
                var index = _SortModuleCfgIDList.FindIndex(0, _SortModuleCfgIDList.Count, value =>
                {
                    var aiModuleCfg = GameSchedule.Instance.GetAIModuleCfg0(value);
                    return level.nLevel < aiModuleCfg.nLevel;
                });
                if (index < 0)
                    _SortModuleCfgIDList.Add(moduleCfgID);
                else
                    _SortModuleCfgIDList.Insert(index, moduleCfgID);
            }
            moduleList.Add(aiModuleID);
        }
        public void RemoveAIModuleData(int moduleCfgID, int aiModuleID)
        {
            if (!_AIModuleIDDic.TryGetValue(moduleCfgID, out var moduleList))
                return;
            if (!moduleList.Remove(aiModuleID))
                return;
            if (moduleList.Count == 0)
            {
                _AIModuleIDDic.Remove(moduleCfgID);
                _SortModuleCfgIDList.Remove(moduleCfgID);
            }
        }
        public bool TryGetModuleList(int moduleCfgID, out List<int> moduleDataIDList)
        {
            if (!_AIModuleIDDic.TryGetValue(moduleCfgID, out moduleDataIDList))
                return false;
            return true;
        }

        public void OnPoolDestroy()
        {
            _AIModuleIDDic.Clear();
            _SortModuleCfgIDList.Clear();
        }
        public ref List<int> GetKeyList()
        {
            return ref _SortModuleCfgIDList;
        }
        public int GetAllAIModuleDataIDs(ref List<int> moduleDataIDs)
        {
            foreach (var item in _AIModuleIDDic)
            {
                moduleDataIDs.AddRange(item.Value);
            }
            return moduleDataIDs.Count;
        }
    }

    public class AIModuleInfo : IClassPoolDestroy
    {
        public IAIModule aiModule = null;
        private List<int> _EntityIDList = new(5);
        public int Count => _EntityIDList.Count;

        public void AddEntityID(int entityID)
        {
            _EntityIDList.Add(entityID);
        }
        public void RemoveEntityID(int entityID)
        {
            var index = _EntityIDList.IndexOf(entityID);
            if (index < 0)
                return;
            _EntityIDList[index] = _EntityIDList[^1];
            _EntityIDList.Remove(Count - 1);
        }
        public int GetEntityIDByIndex(int index)
        {
            return _EntityIDList[index];
        }
        public void OnPoolDestroy()
        {
            aiModule = null;
            _EntityIDList.Clear();
        }
    }
    #endregion
    private Dictionary<int, EntityAIInfo> _EntityAIData = new();
    private Dictionary<int, AIModuleInfo> _AiModuleDic = new();

    private List<int> _TempList = new(new int[100]);
    public bool AIModuleIDIsValid(int moduleID)
    {
        var result = _AiModuleDic.ContainsKey(moduleID);
        return result;
    }
    private IAIModule CreateAIModuleData(int moduleCfgID, AIModuleUserData userData)
    {
        return (EnAIModuleType)moduleCfgID switch
        {
            EnAIModuleType.RangeMove => ClassPoolMgr.Instance.Pull<AIRandomMoveModule>(userData),
            EnAIModuleType.Idle => ClassPoolMgr.Instance.Pull<AIIdleModule>(userData),
            EnAIModuleType.MonsterBattle => ClassPoolMgr.Instance.Pull<AIMonsterBattleModule>(userData),
            EnAIModuleType.Pastor => ClassPoolMgr.Instance.Pull<AIPastorModule>(userData),
            EnAIModuleType.BlueWarrior => ClassPoolMgr.Instance.Pull<AIBlueWarriorBattleModule>(userData),
            _ => null
        };
    }
    public bool TryGetEntityAIInfo(int entityID, out EntityAIInfo info)
    {
        if (!_EntityAIData.TryGetValue(entityID, out info))
            return false;
        return true;
    }
    private EntityAIInfo AddEntityAIInfo(int entityID)
    {
        var entityAIInfo = ClassPoolMgr.Instance.Pull<EntityAIInfo>();
        _EntityAIData.Add(entityID, entityAIInfo);
        return entityAIInfo;
    }
    private void RemoveEntityAIInfo(int entityID)
    {
        if (!TryGetEntityAIInfo(entityID, out var info))
            return;
        ClassPoolMgr.Instance.Push(info);
        _EntityAIData.Remove(entityID);
    }
    public bool TryGetAIModuleData(int moduleID, out IAIModule moduleData)
    {
        if (!TryGetAIModuleData<IAIModule>(moduleID, out moduleData))
            return false;
        return true;
    }
    private bool TryGetAIModuleData<T>(int moduleID, out T moduleData)
        where T : class, IAIModule
    {
        if (!_AiModuleDic.TryGetValue(moduleID, out var moduleInfo))
        {
            moduleData = default;
            return false;
        }
        moduleData = moduleInfo.aiModule as T;
        return moduleData != null;
    }
    private int CreateAIMoudle(int moduleCfgID, AIModuleUserData userData)
    {
        var moduleDataID = ABBUtil.GetTempKey();
        userData.moduleDataID = moduleDataID;
        userData.aiModuleCfgID = moduleCfgID;
        var aiModuleData = CreateAIModuleData(moduleCfgID, userData);

        var moduleInfo = ClassPoolMgr.Instance.Pull<AIModuleInfo>();
        moduleInfo.aiModule = aiModuleData;
        _AiModuleDic.Add(moduleDataID, moduleInfo);
        return moduleDataID;
    }
    public void DestroyAIModule(int moduleDataID)
    {
        if (!_AiModuleDic.TryGetValue(moduleDataID, out var moduleInfo))
            return;
        for (int i = 0; i < moduleInfo.Count; i++)
        {
            var entityID = moduleInfo.GetEntityIDByIndex(i);
            RemoveEntityAIData(entityID, moduleDataID);
        }
        ClassPoolMgr.Instance.Push(moduleInfo);
        _AiModuleDic.Remove(moduleDataID);
    }

    public int AddEntityAIModule(int entityID, int moduleCfgID)
    {
        var userData = ClassPoolMgr.Instance.Pull<AIModuleUserData>();
        userData.entityID = entityID;
        var aiModuleID = AddEntityAIModule(entityID, moduleCfgID, userData);
        ClassPoolMgr.Instance.Push(userData);
        return aiModuleID;
    }
    public int AddEntityAIModule(int entityID, int moduleCfgID, AIModuleUserData userData)
    {
        userData.entityID = entityID;
        var moduleDataID = CreateAIMoudle(moduleCfgID, userData);
        if (!_AiModuleDic.TryGetValue(moduleDataID, out var moduleInfo))
            return -1;
        moduleInfo.AddEntityID(entityID);

        if (!TryGetEntityAIInfo(entityID, out var entityAIInfo))
            entityAIInfo = AddEntityAIInfo(entityID);

        entityAIInfo.AddAIModuleData(moduleCfgID, moduleDataID);
        return moduleDataID;
    }
    public void RemoveEntityAIModule(int entityID, int moduleDataID)
    {
        if (!_AiModuleDic.TryGetValue(moduleDataID, out var moduleInfo))
            return;
        moduleInfo.RemoveEntityID(entityID);

        RemoveEntityAIData(entityID, moduleDataID);
    }
    public void RemoveEntityAI(int entityID)
    {
        if (!_EntityAIData.TryGetValue(entityID, out var entityAIInfo))
            return;
        var count = entityAIInfo.GetAllAIModuleDataIDs(ref _TempList);
        for (int i = 0; i < count; i++)
        {
            var moduleDataID = _TempList[i];
            RemoveEntityAIModule(entityID, moduleDataID);
        }
        ClassPoolMgr.Instance.Push(entityAIInfo);
    }
    private void RemoveEntityAIData(int entityID, int moduleDataID)
    {
        if (!TryGetAIModuleData(moduleDataID, out var moduleData))
            return;
        if (!TryGetEntityAIInfo(entityID, out var entityAIInfo))
            return;
        var aiModuleCfgID = moduleData.GetAIModuleCfgID();
        entityAIInfo.RemoveAIModuleData(aiModuleCfgID, moduleDataID);
        if (entityAIInfo.Count == 0)
        {
            RemoveEntityAIInfo(entityID);
        }
    }

}
