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
public class AttackMgr : Singleton<AttackMgr>
{
    public void AttackEntity(int entityID, int entityID2, int value)
    {
        Entity3DMgr.Instance.AddEntityCmd(entityID2, EnEntityCmd.Injured);

        var entityPos = Entity3DMgr.Instance.GetEntityWorldPos(entityID2);
        var atkPos = entityPos + new Vector3(Random.Range(-0.2f,0.2f), Random.Range(1.8f, 2.2f), Random.Range(-0.2f, 0.2f));
        ShowView(atkPos, value);
    }

    private void ShowView(Vector3 pos, int value)
    {
        var entityID = Entity3DMgr.Instance.CreateEntityData<AtkNumEntityData>();
        var atkNumData = Entity3DMgr.Instance.GetEntity3DData<AtkNumEntityData>(entityID);
        atkNumData.SetPosition(pos);
        atkNumData.SetNumValue(value);
        Entity3DMgr.Instance.LoadEntity(entityID);
    }


    public static ISkillScheduleAction  GetAtkLinkScheduleItem(EnAtkLinkScheculeType scheduleType, int[] data, ref int startIndex)
    {
        var count = data[startIndex++];
        ISkillScheduleAction  item = scheduleType switch
        {
            EnAtkLinkScheculeType.Physics => ClassPoolMgr.Instance.Pull<SkillItemPhysicsData>(),
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
