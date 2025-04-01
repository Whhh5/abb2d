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

public class SkillEffectBindingLocalTransformInfo : ISkillEffectBindingData
{
    public float offsetX;
    public float offsetY;
    public float offsetZ;

    public float offsetRotX;
    public float offsetRotY;
    public float offsetRotZ;


    public void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = arrCount + startIndex;
        var gCount = startIndex >= endIndex ? default : data[startIndex];
        offsetX = gCount-- < 1 ? default : data[startIndex] / 100f;
        offsetY = gCount-- < 1 ? default : data[startIndex] / 100f;
        offsetZ = gCount-- < 1 ? default : data[startIndex] / 100f;

        offsetRotX = gCount-- < 1 ? default : data[startIndex] / 100f;
        offsetRotY = gCount-- < 1 ? default : data[startIndex] / 100f;
        offsetRotZ = gCount-- < 1 ? default : data[startIndex] / 100f;

    }
    public void Excute(int entityID, int effectID)
    {
        var enittyPos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
        var forword = Entity3DMgr.Instance.GetEntityForward(entityID);
        var up = Entity3DMgr.Instance.GetEntityUp(entityID);
        var right = Entity3DMgr.Instance.GetEntityRight(entityID);
        var pos = enittyPos + right * offsetX + up * offsetY + forword * offsetZ;

        var entityRot = Entity3DMgr.Instance.GetEntityRotation(entityID);
        var rot = entityRot + new Vector3(offsetRotX, offsetRotY, offsetRotZ);

        EffectMgr.Instance.PlayEffectOnce(effectID, pos, rot);
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

    public float offsetRotX;
    public float offsetRotY;
    public float offsetRotZ;

    public int[] effectParams;



    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;
    public ISkillEffectBindingData _SkillEffectBindingData = null;
    private int _EffectEntityID = 1;
    public void OnPoolDestroy()
    {
        EffectMgr.Instance.KillEffect(_EffectEntityID);
        schedule
            = effectID
            = _EffectEntityID
            = -1;
        effectParams = null;
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
    public void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = startIndex + arrCount;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        effectID = gCount-- < 1 ? default : data[startIndex++];
        schedule = gCount-- < 1 ? default : data[startIndex++] / 100f;
        offsetX = gCount-- < 1 ? default : data[startIndex++] / 100f;
        offsetY = gCount-- < 1 ? default : data[startIndex++] / 100f;
        offsetZ = gCount-- < 1 ? default : data[startIndex++] / 100f;
        traceType = gCount-- < 1 ? default : (EnEffectTraceType)data[startIndex++];
        bindingType = gCount-- < 1 ? default : (EnEffectBindingType)data[startIndex++];
        offsetRotX = gCount-- < 1 ? default : data[startIndex++] / 100f;
        offsetRotY = gCount-- < 1 ? default : data[startIndex++] / 100f;
        offsetRotZ = gCount-- < 1 ? default : data[startIndex++] / 100f;
        var paramsCount = startIndex >= endIndex ? default : data[startIndex++];
        effectParams = new int[paramsCount];
        data.CopyTo(startIndex, effectParams, paramsCount);
        startIndex += paramsCount;
    }

    public void Reset()
    {

    }

    public int GetEffectID()
    {
        return effectID;
    }

    public int[] GetEffectParams()
    {
        return effectParams;
    }

    public void ScheduleEvent(int entityID, IClassPoolUserData userData)
    {
        var enittyPos = Entity3DMgr.Instance.GetEntityWorldPos(entityID);
        var forword = Entity3DMgr.Instance.GetEntityForward(entityID);
        var up = Entity3DMgr.Instance.GetEntityUp(entityID);
        var right = Entity3DMgr.Instance.GetEntityRight(entityID);
        var pos = enittyPos + right * offsetX + up * offsetY + forword * offsetZ;

        var entityRot = Entity3DMgr.Instance.GetEntityRotation(entityID);
        var rot = entityRot + new Vector3(offsetRotX, offsetRotY, offsetRotZ);
        _EffectEntityID = EffectMgr.Instance.PlayEffect(effectID, pos, rot);

    }
    public void GetEventList(ref List<SkillItemEventInfo> eventList)
    {
        var eventData = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
        eventData.schedule = schedule;
        eventData.onEvent = ScheduleEvent;
        eventList.Add(eventData);
    }

}