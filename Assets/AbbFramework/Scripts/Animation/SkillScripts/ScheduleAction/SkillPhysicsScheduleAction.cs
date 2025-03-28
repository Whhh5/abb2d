using System;
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
    public bool m_IsAtked = false;
    private int _EntityID = -1;


    public void OnPoolDestroy()
    {
        buffParams = null;
        m_IsAtked = false;
        atkSchedule = -1;
        atkValue = -1;
        effectID = -1;
        _EntityID = -1;
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
            buffParams = BuffMgr.Instance.ConvertBuffData(buff, arrBuffParams);
    }

    public void Reset()
    {
        m_IsAtked
            = false;
    }

    EnPhysicsType IPhysicsParams.GetPhysicsType()
    {
        return physicsType;
    }

    public int[] GetPhysicsParams()
    {
        return physicsParams;
    }

    public void Enter(int entityID)
    {
        _EntityID = entityID;
        var data = new PhysicsOverlapCallbackCustomData()
        {
            atkValue = atkValue,
            entityID = entityID,
        };
        var monsterData = Entity3DMgr.Instance.GetMonsterEntityData(entityID);
        var layer = monsterData.GetEnemyLayer();
        PhysicsMgr.Instance.PhysicsOverlap(physicsResolve, entityID, layer, PhysicsOverlapCallback, data);
    }

    public void Exit()
    {

    }

    public bool GetIsEffect()
    {
        return m_IsAtked;
    }

    public void SetIsEffect(bool isEffect)
    {
        m_IsAtked = isEffect;
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
                BuffMgr.Instance.AddEntityBuff(_EntityID, entityInfo.entityID, buff, buffParams);
        }
    }

    public float GetEnterSchedule()
    {
        return atkSchedule;
    }
}