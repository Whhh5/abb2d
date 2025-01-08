using System;
using UnityEngine;


public class AttackLinkBuffItem : IAttackLinkScheduleItem
{
    public float addSchedule;
    public int buffID;
    public int[] arrBuffParams;

    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;

    private bool m_IsEffect = false;
    public void OnPoolDestroy()
    {
        addSchedule = -1;
        buffID = -1;
        arrBuffParams = null;
        m_ScheduleType = EnAtkLinkScheculeType.None;
        m_IsEffect = false;
    }

    public void PoolConstructor()
    {
    }

    public void OnPoolInit<T>(ref T userData) where T : struct, IPoolUserData
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
        
    }
    public void Enter(int entityID)
    {

    }
    public void Exit()
    {

    }

    public void Reset()
    {
        m_IsEffect = false;
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
        return addSchedule;
    }

}
