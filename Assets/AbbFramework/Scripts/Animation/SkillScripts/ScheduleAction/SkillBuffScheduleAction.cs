using System;
using System.Collections.Generic;
using UnityEngine;


public class SkillBuffScheduleAction : ISkillScheduleAction
{
    public float addSchedule;
    public int buffID;
    public int[] arrBuffParams;
    public IEntityBuffParams _BuffDataParams;

    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;

    private bool m_IsEffect = false;
    private int _AddBuffKey = -1;
    public void OnPoolDestroy()
    {
        if (_AddBuffKey > 0)
            BuffMgr.Instance.RemoveEntityBuff(_AddBuffKey);
        BuffUtil.PushConvertBuffData(_BuffDataParams);
        _AddBuffKey = -1;
        addSchedule = -1;
        buffID = -1;
        arrBuffParams = null;
        _BuffDataParams = null;
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
        var endIndex = arrCount + startIndex;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        addSchedule = gCount-- < 0 ? default : data[startIndex++] / 100f;
        buffID = gCount-- < 0 ? default : data[startIndex++];

        var arrParamsCount = startIndex >= endIndex ? default : data[startIndex++];
        arrBuffParams = data.Copy(startIndex, arrParamsCount);
        startIndex += arrParamsCount;

        _BuffDataParams = BuffUtil.ConvertBuffData((EnBuff)buffID, arrBuffParams);
    }
    public void Enter(int entityID)
    {
        var addKey = BuffMgr.Instance.AddEntityBuff(entityID, entityID, (EnBuff)buffID, _BuffDataParams);
        if (BuffMgr.Instance.GetBuffType(addKey) != EnBuffType.Time)
            _AddBuffKey = addKey;

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
