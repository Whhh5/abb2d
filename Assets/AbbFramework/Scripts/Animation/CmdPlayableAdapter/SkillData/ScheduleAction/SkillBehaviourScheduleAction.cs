﻿using System;



public enum EnBehaviourType
{
    None,
    [EditorFieldName("高度")]
    Height,
}

public class SkillBehaviourScheduleAction : ISkillScheduleAction<PoolNaNUserData>
{
    public float schedule;
    public EnBehaviourType behaviourType;
    public int[] arrParams;


    private bool m_IsEffect = false;
    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;
    public void OnPoolDestroy()
    {
        arrParams = null;
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
    public void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = startIndex + arrCount;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        schedule = gCount < 1 ? default : (data[startIndex++] / 100f);
        behaviourType = gCount < 2 ? default : (EnBehaviourType)data[startIndex++];

        var paramCount = startIndex >= endIndex ? default : data[startIndex++];
        arrParams = data.Copy(startIndex, paramCount);
        startIndex += paramCount;
    }
    public void Enter(int entityID)
    {
        var height = arrParams[0] / 100f;
        //var time = arrPArams[1] / 100f;
        Entity3DMgr.Instance.SetEntityHeight(entityID, height, 1);
    }

    public void Exit()
    {
    }

    public float GetEnterSchedule()
    {
        return schedule;
    }

    public bool GetIsEffect()
    {
        return m_IsEffect;
    }


    public void Reset()
    {
        m_IsEffect = false;
        m_ScheduleType = EnAtkLinkScheculeType.None;
    }

    public EnAtkLinkScheculeType GetScheduleType()
    {
        return m_ScheduleType;
    }
    public void SetIsEffect(bool isEffect)
    {
        m_IsEffect = isEffect;
    }

    public void SetScheduleType(EnAtkLinkScheculeType scheduleType)
    {
        m_ScheduleType = scheduleType;
    }

}
