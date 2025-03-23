
using System.Collections.Generic;
using UnityEngine;


public sealed class EntityAIComData : Entity3DComData, IUpdate
{
    private int _CurAIModuleID = -1;
    //private Dictionary<int, List<int>> _Level2ModuleIDs = new();
    //private List<int> _Levels = new();

    public override void OnPoolDestroy()
    {
        base.OnPoolDestroy();
        _CurAIModuleID
            = -1;
        //_Levels.Clear();
        //_Level2ModuleIDs.Clear();
    }

    public override void OnDisable()
    {
        StopCurModuleData();
        UpdateMgr.Instance.Unregistener(this);
        base.OnDisable();
    }
    public override void OnEnable()
    {
        base.OnEnable();

        UpdateMgr.Instance.Registener(this);
    }

    public override void OnPoolInit(Entity3DComDataUserData userData)
    {
        base.OnPoolInit(userData);
    }
    private void StopCurModuleData()
    {
        if (_CurAIModuleID <= 0)
            return;
        if (!EntityAIMgr.Instance.TryGetAIModuleData(_CurAIModuleID, out var moduleData))
            return;
        moduleData.Finish();
        _CurAIModuleID = -1;
    }
    public void Update()
    {
        if (!EntityUtil.IsValid(_EntityID))
        {
            StopCurModuleData();
            return;
        }
        var nextAIModuleID = GetNextModuleID();
        if (!EntityAIMgr.Instance.TryGetAIModuleData(nextAIModuleID, out var nextModuleData))
            return;
        var moduleCfgID = nextModuleData.GetAIModuleCfgID();
        var level = GameSchedule.Instance.GetAIModuleCfg0(moduleCfgID).nLevel;

        if (nextAIModuleID == _CurAIModuleID)
        {
            if (!EntityAIMgr.Instance.TryGetAIModuleData(_CurAIModuleID, out var moduleData))
                return;
            if (moduleData.IsNextModule())
            {
                moduleData.Reexecute();
            }
            return;
        }


        if (EntityAIMgr.Instance.AIModuleIDIsValid(_CurAIModuleID))
        {
            if (!EntityAIMgr.Instance.TryGetAIModuleData(_CurAIModuleID, out var moduleData))
                return;
            var curLevel = GameSchedule.Instance.GetAIModuleCfg0(moduleData.GetAIModuleCfgID()).nLevel;

            var isNext = level > curLevel ? moduleData.IsBreak() : moduleData.IsNextModule();
            if (isNext)
            {
                moduleData.Finish();
                nextModuleData.PreExecute();
                nextModuleData.Execute();
                _CurAIModuleID = nextAIModuleID;
            }
        }
        else
        {
            nextModuleData.PreExecute();
            nextModuleData.Execute();
            _CurAIModuleID = nextAIModuleID;
        }
    }
    private int GetNextModuleID()
    {
        if (!EntityAIMgr.Instance.TryGetEntityAIInfo(_EntityID, out var aiInfo))
            return -1;
        ref var keyList = ref aiInfo.GetKeyList();
        for (int j = 0; j < keyList.Count; j++)
        {
            var cfgID = keyList[^(j + 1)];
            if (!aiInfo.TryGetModuleList(cfgID, out var moduleList))
                continue;
            var index = Random.Range(0, moduleList.Count - 1);
            var moduleID = moduleList[index];
            if (!EntityAIMgr.Instance.TryGetAIModuleData(moduleID, out var moduleData))
                continue;
            if (!moduleData.IsExecute())
                continue;
            return moduleID;
        }
        return -1;
    }

}