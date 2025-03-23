using UnityEngine;


public enum EnAtkLinkScheculeType
{
    None,
    [EditorFieldName("物理检测")]
    Physics,
    [EditorFieldName("特效")]
    Effect,
    [EditorFieldName("buff")]
    Buff,
    [EditorFieldName("行为")]
    Behaviour,
    EnumCount,
}
public class EventBattleInfo: IClassPoolNone
{
    public int entityID1;
    public int entityID2;
    public int toValue;
    public int fromValue;
}
public class AttackMgr : Singleton<AttackMgr>
{
    public void AttackEntity(int entityID, int entityID2, int value)
    {
        var curHealthValue = Entity3DMgr.Instance.GetEntityHealthValue(entityID2);
        var health = curHealthValue - value;
        Entity3DMgr.Instance.SetEntityHealthValue(entityID2, curHealthValue - value);
        if (health > 0)
        {
            Entity3DMgr.Instance.AddEntityCmd(entityID2, EnEntityCmd.Injured);
        }
        else
        {
            var monsterID = EntityUtil.EntityID2MonsterID(entityID2);
            var monsterCfg = GameSchedule.Instance.GetMonsterCfg0(monsterID);
            if (monsterCfg.nDieCmdID > 0)
            {
                Entity3DMgr.Instance.AddEntityCmd(entityID2, (EnEntityCmd)monsterCfg.nDieCmdID);
            }
            Entity3DMgr.Instance.DieEntity(entityID2);
        }

        var userData = ClassPoolMgr.Instance.Pull<EventBattleInfo>();
        userData.entityID1 = entityID;
        userData.entityID2 = entityID2;
        userData.fromValue = curHealthValue;
        userData.toValue = curHealthValue - value;
        ABBEventMgr.Instance.FireExecute(EnABBEvent.EVENT_BATTLE_INFO, 0, 0, userData);
        ClassPoolMgr.Instance.Push(userData);
    }

    public static ISkillScheduleAction GetAtkLinkScheduleItem(EnAtkLinkScheculeType scheduleType, int[] data, ref int startIndex)
    {
        var count = data[startIndex++];
        ISkillScheduleAction item = scheduleType switch
        {
            EnAtkLinkScheculeType.Physics => ClassPoolMgr.Instance.Pull<SkillPhysicsScheduleAction>(),
            EnAtkLinkScheculeType.Effect => ClassPoolMgr.Instance.Pull<SkillEffectScheduleAction>(),
            EnAtkLinkScheculeType.Buff => ClassPoolMgr.Instance.Pull<SkillBuffScheduleAction>(),
            EnAtkLinkScheculeType.Behaviour => ClassPoolMgr.Instance.Pull<SkillBehaviourScheduleAction>(),
            _ => null,
        };
        item.SetScheduleType(scheduleType);
        item.Init(data, count, ref startIndex);
        return item;
    }
}
