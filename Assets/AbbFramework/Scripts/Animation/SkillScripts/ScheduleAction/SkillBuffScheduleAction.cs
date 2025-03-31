using System;
using System.Collections.Generic;
using UnityEngine;


public class SkillBuffScheduleAction : ISkillScheduleAction
{
    public float startSchedule;
    public float endSchedule;
    public int buffID;
    public int[] arrBuffParams;
    public IEntityBuffParams _BuffDataParams;

    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;

    private int _AddBuffKey = -1;
    public void OnPoolDestroy()
    {
        RemoveEntityBuff();
        BuffUtil.PushConvertBuffData(_BuffDataParams);
        startSchedule
            = endSchedule
            = _AddBuffKey
            = -1;
        buffID = -1;
        arrBuffParams = null;
        _BuffDataParams = null;
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
        var endIndex = arrCount + startIndex;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        startSchedule = gCount-- < 0 ? default : data[startIndex++] / 100f;
        buffID = gCount-- < 0 ? default : data[startIndex++];
        endSchedule = gCount-- < 0 ? default : data[startIndex++] / 100f;

        var arrParamsCount = startIndex >= endIndex ? default : data[startIndex++];
        arrBuffParams = data.Copy(startIndex, arrParamsCount);
        startIndex += arrParamsCount;

        _BuffDataParams = BuffUtil.ConvertBuffData((EnBuff)buffID, arrBuffParams);
    }

    public void ScheduleEvent(int entityID, IClassPoolUserData userData)
    {
        var addKey = BuffMgr.Instance.AddEntityBuff(entityID, entityID, (EnBuff)buffID, _BuffDataParams);
        if (BuffMgr.Instance.GetBuffType(addKey) != EnBuffType.Time)
            _AddBuffKey = addKey;

    }
    public void ScheduleEvent2(int entityID, IClassPoolUserData userData)
    {
        RemoveEntityBuff();
    }
    public void Reset()
    {
        RemoveEntityBuff();
    }
    private void RemoveEntityBuff()
    {
        if (_AddBuffKey <= 0)
            return;
        BuffMgr.Instance.RemoveEntityBuff(_AddBuffKey);
        _AddBuffKey = -1;
    }

    public void GetEventList(ref List<SkillItemEventInfo> eventList)
    {
        var eventData = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
        eventData.schedule = startSchedule;
        eventData.onEvent = ScheduleEvent;
        eventList.Add(eventData);

        if (endSchedule > 0)
        {
            var eventData2 = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
            eventData2.schedule = endSchedule;
            eventData2.onEvent = ScheduleEvent2;
            eventList.Add(eventData2);
        }
    }
}
