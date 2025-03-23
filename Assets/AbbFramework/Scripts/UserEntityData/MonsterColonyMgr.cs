using System.Collections.Generic;
using UnityEngine;

public class MonsterColonyMgr : Singleton<MonsterColonyMgr>
{
    private class MonsterColontInfo : IClassPoolDestroy
    {
        public float lastTime = 0;
        public List<int> monsterIDList = new(10);
        public int Count => monsterIDList.Count;

        public void OnPoolDestroy()
        {
            lastTime
                = 0;
            monsterIDList.Clear();
        }
    }
    public override EnManagerFuncType FuncType => base.FuncType | EnManagerFuncType.Update;
    private Dictionary<int, MonsterColontInfo> _MonsterColonyDic = new();
    private List<int> _MonsterColonyList = new(20);

    private void AddMonsterColny(int entityID)
    {
        var info = ClassPoolMgr.Instance.Pull<MonsterColontInfo>();
        _MonsterColonyDic.Add(entityID, info);
        _MonsterColonyList.Add(entityID);
    }
    private void RemoveMonsterColny(int entityID)
    {
        if (!_MonsterColonyDic.TryGetValue(entityID, out var info))
            return;
        ClassPoolMgr.Instance.Push(info);
        _MonsterColonyDic.Remove(entityID);
        _MonsterColonyList.Remove(entityID);
    }
    public int CreateMonsterColony(int colontID, Vector3 worldPos)
    {
        var userData = ClassPoolMgr.Instance.Pull<GeneralIntUserData>();
        userData.intValue = colontID;
        var entityID = Entity3DMgr.Instance.CreateEntityData<MonsterColonyLowerData>(userData);
        ClassPoolMgr.Instance.Push(userData);
        var colonyEntityData = Entity3DMgr.Instance.GetEntity3DData<MonsterColonyLowerData>(entityID);
        colonyEntityData.SetPosition(worldPos);
        //colonyEntityData.AddEntityCom<EntityAnimComData>();
        AddMonsterColny(entityID);

        Entity3DMgr.Instance.LoadEntity(entityID);
        return entityID;
    }

    public override void Update()
    {
        base.Update();

        for (int i = 0; i < _MonsterColonyList.Count; i++)
        {
            var colonyEntityID = _MonsterColonyList[i];
            var colonyInfo = _MonsterColonyDic[colonyEntityID];
            var colonyEntityData = Entity3DMgr.Instance.GetEntity3DData<MonsterColonyData>(colonyEntityID);
            var colonyCfg = GameSchedule.Instance.GetMonsterColonyCfg0(colonyEntityData.GetMonsterColonyID());

            if (colonyInfo.Count >= colonyCfg.nMaxCount)
                continue;
            if (colonyInfo.lastTime + colonyCfg.fCreateIntervalTime > ABBUtil.GetGameTimeSeconds())
                continue;
            colonyInfo.lastTime = ABBUtil.GetGameTimeSeconds();


            var index = Random.Range(0, colonyCfg.arrMonsterID.Length);
            var monsterID = colonyCfg.arrMonsterID[index];
            var worldPos = Entity3DMgr.Instance.GetEntityWorldPos(colonyEntityID);
            var pos = worldPos + new Vector3(colonyCfg.v3CreateLocalPos[0], colonyCfg.v3CreateLocalPos[1], colonyCfg.v3CreateLocalPos[2]);
            var monsterEntityID = MonsterMgr.Instance.CreateMonster(monsterID, pos);
            colonyInfo.monsterIDList.Add(monsterEntityID);

            var userData = ClassPoolMgr.Instance.Pull<AIRandomMoveModuleUserData>();
            userData.rangeType = EnAIRangeType.Sphere;
            userData.typeParams = new int[] { colonyCfg.nRangeRadius };
            userData.centerPos = worldPos;
            EntityAIMgr.Instance.AddEntityAIModule(monsterEntityID, (int)EntityAIMgr.EnAIModuleType.RangeMove, userData);
            ClassPoolMgr.Instance.Push(userData);

            Entity3DMgr.Instance.SetEntityControllerType(monsterEntityID, EnEntityControllerType.AI);

            EntityAIMgr.Instance.AddEntityAIModule(monsterEntityID, (int)EntityAIMgr.EnAIModuleType.MonsterBattle);

        }
    }
}
