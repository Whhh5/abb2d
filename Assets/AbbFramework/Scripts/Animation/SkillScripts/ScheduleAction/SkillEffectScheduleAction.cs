using System;
using System.Collections.Generic;
using UnityEngine;




public enum EnEffectBindingType
{
    None,
    LocalTransform,
    Bone,
}
public enum EnEffectTraceType
{
    None,
    Once,
    Successive,
}

public interface ISkillEffectBindingData
{
    public void Init(int[] data, int arrCount, ref int startIndex);
    public void Excute(int entityID, int effectID);
}

public class SkillEffectBindingLocalTransformInfo: ISkillEffectBindingData
{
    public float offsetX;
    public float offsetY;
    public float offsetZ;


    public void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = arrCount + startIndex;
        var gCount = startIndex >= endIndex ? default : data[startIndex];
        offsetX = gCount < 1 ? default : data[startIndex] / 100f;
        offsetY = gCount < 2 ? default : data[startIndex] / 100f;
        offsetZ = gCount < 3 ? default : data[startIndex] / 100f;
    }
    public void Excute(int entityID, int effectID)
    {
        var enittyPos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
        var forword = Entity3DMgr.Instance.GetEntityForward(entityID);
        var up = Entity3DMgr.Instance.GetEntityUp(entityID);
        var right = Entity3DMgr.Instance.GetEntityRight(entityID);
        EffectMgr.Instance.PlayEffectOnce(effectID, enittyPos + right * offsetX + up * offsetY + forword * offsetZ);
    }
}

public class SkillEffectScheduleAction : IEffectParams, ISkillScheduleAction
{
    public int effectID;
    public float schedule;
    public EnEffectTraceType traceType;
    public EnEffectBindingType bindingType;

    public float offsetX;
    public float offsetY;
    public float offsetZ;
    public int[] effectParams;



    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;
    public bool m_IsEffect = false;
    public ISkillEffectBindingData _SkillEffectBindingData = null;

    public void OnPoolDestroy()
    {
        effectID = -1;
        schedule = -1;
        effectParams = null;
        m_ScheduleType = EnAtkLinkScheculeType.None;
        m_IsEffect = false;
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
        offsetX = gCount < 3 ? default : data[startIndex++] / 100f;
        offsetY = gCount < 4 ? default : data[startIndex++] / 100f;
        offsetZ = gCount < 5 ? default : data[startIndex++] / 100f;
        traceType = gCount < 6 ? default : (EnEffectTraceType)data[startIndex++];
        bindingType = gCount < 7 ? default : (EnEffectBindingType)data[startIndex++];

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
        var forword = Entity3DMgr.Instance.GetEntityForward(entityID);
        var up = Entity3DMgr.Instance.GetEntityUp(entityID);
        var right = Entity3DMgr.Instance.GetEntityRight(entityID);
        EffectMgr.Instance.PlayEffectOnce(effectID, enittyPos + right * offsetX + up * offsetY + forword * offsetZ);

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