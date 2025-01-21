using System;
using UnityEngine;

public class SkillEffectScheduleAction : IEffectParams, ISkillScheduleAction<PoolNaNUserData>
{
    public int effectID;
    public float schedule;
    public int[] effectParams;

    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;
    public bool m_IsEffect = false;

    public void OnPoolDestroy()
    {
        effectID = -1;
        schedule = -1;
        effectParams = null;
        m_ScheduleType = EnAtkLinkScheculeType.None;
        m_IsEffect = false;
    }
    public void PoolConstructor()
    {
    }

    public void OnPoolInit(PoolNaNUserData userData)
    {
    }

    public void OnPoolEnable()
    {
    }

    public void PoolRelease()
    {
    }
    public void SetScheduleType(EnAtkLinkScheculeType scheduleType)
    {
        m_ScheduleType = scheduleType;
    }

    public EnAtkLinkScheculeType GetScheduleType()
    {
        return m_ScheduleType;
    }
    public void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = startIndex + arrCount;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        effectID = gCount < 1 ? default : data[startIndex++];
        schedule = gCount < 2 ? default : data[startIndex++] / 100f;

        var paramsCount = startIndex >= endIndex ? default : data[startIndex++];
        effectParams = new int[paramsCount];
        data.CopyTo(startIndex, effectParams, paramsCount);
        startIndex += paramsCount;
    }

    public void Reset()
    {
        m_IsEffect
            = false;
    }

    public int GetEffectID()
    {
        return effectID;
    }

    public int[] GetEffectParams()
    {
        return effectParams;
    }

    public void Enter(int entityID)
    {
        var enittyPos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
        var entityForward = Entity3DMgr.Instance.GetEntityForward(entityID);
        var enittyRot = Entity3DMgr.Instance.GetEntityRotation(entityID);
        var effectEntityID = Entity3DMgr.Instance.CreateEntityData<EffectAttack5Data>();
        var entityData = Entity3DMgr.Instance.GetEntity3DData<EffectAttack5Data>(effectEntityID);
        entityData.SetParams(effectParams);
        entityData.SetPosition(enittyPos + Vector3.up * 1);
        entityData.SetLocalRotation(enittyRot);
        Entity3DMgr.Instance.LoadEntity(effectEntityID);
    }

    public void Exit()
    {

    }

    public bool GetIsEffect()
    {
        return m_IsEffect;
    }

    public void SetIsEffect(bool isEffect)
    {
        m_IsEffect = isEffect;
    }

    public float GetEnterSchedule()
    {
        return schedule;
    }

}