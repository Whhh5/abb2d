using System;
using System.Collections.Generic;
using UnityEngine;
using static SkillTypeLinkPlayableAdapter;




public class PhysicsOverlapCallbackCustomData : IPhysicsColliderCallbackCustomData
{
    public int atkValue;
    public int entityID;
}
public class SkillPhysicsScheduleAction : IPhysicsParams, ISkillScheduleAction
{

    public float atkSchedule;
    public int atkValue;
    public EnPhysicsType physicsType = EnPhysicsType.Sphere;
    public int effectID; // 击中特效
    public EnBuff buff;
    public int[] physicsParams;
    public int[] arrBuffParams; // 攻击特效，附带 buff

    private IEntityBuffParams buffParams = null;
    public IPhysicsResolve physicsResolve;
    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;


    public void OnPoolDestroy()
    {
        BuffUtil.PushConvertBuffData(buffParams);
        buffParams = null;
        atkSchedule = -1;
        atkValue = -1;
        effectID = -1;
        buff = EnBuff.None;
        physicsType = EnPhysicsType.Sphere;
        physicsParams = null;
        physicsResolve = null;
        m_ScheduleType = EnAtkLinkScheculeType.None;
    }

    public void SetScheduleType(EnAtkLinkScheculeType scheduleType)
    {
        m_ScheduleType = scheduleType;
    }

    public EnAtkLinkScheculeType GetScheduleType()
    {
        return m_ScheduleType;
    }
    public virtual void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = startIndex + arrCount;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        atkSchedule = gCount < 1 ? default : data[startIndex++] / 100f;
        atkValue = gCount < 2 ? default : data[startIndex++];
        physicsType = gCount < 3 ? default : (EnPhysicsType)data[startIndex++];
        effectID = gCount < 4 ? 7 : data[startIndex++];
        buff = gCount < 5 ? EnBuff.None : (EnBuff)data[startIndex++];

        var paramCount = startIndex >= endIndex ? default : data[startIndex++];
        physicsParams = new int[paramCount];
        data.CopyTo(startIndex, physicsParams, paramCount);
        startIndex += paramCount;

        var buffParamsCount = startIndex >= endIndex ? default : data[startIndex++];
        arrBuffParams = data.Copy(startIndex, buffParamsCount);
        startIndex += buffParamsCount;

        if (physicsType > 0)
            physicsResolve = PhysicsUtil.CreatePhysicsResolve(this);
        if (buff > EnBuff.None)
            buffParams = BuffUtil.ConvertBuffData(buff, arrBuffParams);
    }

    public void Reset()
    {

    }

    EnPhysicsType IPhysicsParams.GetPhysicsType()
    {
        return physicsType;
    }

    public int[] GetPhysicsParams()
    {
        return physicsParams;
    }

    public void GetEventList(ref List<SkillItemEventInfo> eventList)
    {
        var item1 = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
        item1.schedule = atkSchedule;
        item1.onEvent = ScheduleEvent;
        eventList.Add(item1);
    }

    public void ScheduleEvent(int entityID, IClassPoolUserData userData)
    {
        var data = new PhysicsOverlapCallbackCustomData()
        {
            atkValue = atkValue,
            entityID = entityID,
        };
        var monsterData = Entity3DMgr.Instance.GetMonsterEntityData(entityID);
        var layer = monsterData.GetEnemyLayer();
        PhysicsMgr.Instance.PhysicsOverlap(physicsResolve, entityID, layer, PhysicsOverlapCallback, data);
    }


    private void PhysicsOverlapCallback(ref EntityPhysicsInfo[] entityIDs, ref int count, IPhysicsColliderCallbackCustomData customData)
    {
        var data = customData as PhysicsOverlapCallbackCustomData;
        for (int i = 0; i < count; i++)
        {
            ref var entityInfo = ref entityIDs[i];
            AttackMgr.Instance.AttackEntity(data.entityID, entityInfo.entityID, data.atkValue);

            if (effectID > 0)
                EffectMgr.Instance.PlayEffectOnce(effectID, entityInfo.closestPoint);

            if (buff > EnBuff.None)
                BuffMgr.Instance.AddEntityBuff(data.entityID, entityInfo.entityID, buff, buffParams);
        }
    }
}