using System;
using System.Collections.Generic;



public delegate void SkillItemEventInfoEvent(int entityID, IClassPoolUserData userData);
public class SkillItemEventInfo : IClassPoolUserData
{
    public float schedule;
    public IClassPoolUserData userData;
    public SkillItemEventInfoEvent onEvent;

    public void OnPoolDestroy()
    {
        schedule = -1;
        userData = null;
        onEvent = null;
    }
}
public class SkillItemInfo : IClassPoolInit<PoolNaNUserData>
{
    public int _ClipID;
    public float canNextTime;
    public float atkEndTime;
    public bool _IsAutoRemove = true;
    protected Dictionary<EnBuff, int[]> arrBuff = new();

    private List<int> _BuffAddKeyList = new();

    protected ISkillScheduleAction[] m_ArrAtkLinkSchedule = null;

    private List<SkillItemEventInfo> _EventList = new(10);
    private int _EventListIndex = 0;

    public void OnPoolDestroy()
    {
        for (int i = 0; i < _EventList.Count; i++)
            ClassPoolMgr.Instance.Push(_EventList[i]);
        ResetAllItemData();
        foreach (var item in m_ArrAtkLinkSchedule)
        {
            ClassPoolMgr.Instance.Push(item);
        }
        arrBuff.Clear();
        _BuffAddKeyList.Clear();
        _EventList.Clear();
        m_ArrAtkLinkSchedule = null;
        _EventListIndex = 0;
    }
    public void PoolConstructor()
    {
    }

    public void OnPoolInit(PoolNaNUserData userData)
    {
    }
    public void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = startIndex + arrCount;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        _ClipID = gCount < 1 ? default : data[startIndex++];
        canNextTime = gCount < 2 ? default : data[startIndex++] / 100f;
        atkEndTime = gCount < 3 ? default : data[startIndex++] / 100f;
        _IsAutoRemove = gCount < 4 ? _IsAutoRemove : (data[startIndex++] > 0);

        var scheduleCount = startIndex >= endIndex ? default : data[startIndex++];
        m_ArrAtkLinkSchedule = new ISkillScheduleAction[scheduleCount];
        for (int i = 0; i < scheduleCount; i++)
        {
            var scheduleType = (EnAtkLinkScheculeType)data[startIndex++];
            var scheduleItem = AttackMgr.GetAtkLinkScheduleItem(scheduleType, data, ref startIndex);
            m_ArrAtkLinkSchedule[i] = scheduleItem;
            scheduleItem.GetEventList(ref _EventList);
        }

        var buffCount = startIndex >= endIndex ? default : data[startIndex++];
        for (int i = 0; i < buffCount; i++)
        {
            var buff = (EnBuff)data[startIndex++];
            var paramCount = data[startIndex++];
            var arrParams = data.Copy(startIndex, paramCount);
            startIndex += paramCount;
            arrBuff.Add(buff, arrParams);
        }

        _EventList.Sort((item, item2) => item.schedule < item2.schedule ? -1 : 1);
    }

    public bool ScheduleEventIsValid()
    {
        return _EventList.Count > 0 && _EventListIndex < _EventList.Count;
    }
    public SkillItemEventInfo GetCurScheduleItem()
    {
        return _EventList[_EventListIndex];
    }
    public bool NextEventAction()
    {
        _EventListIndex++;
        if (_EventListIndex > _EventList.Count - 1)
            return false;
        return true;
    }
    public void ResetAllItemData()
    {
        _EventListIndex = 0;
        foreach (var item in m_ArrAtkLinkSchedule)
        {
            item.Reset();
        }
    }

    public void OnEnable(int entityID)
    {
        foreach (var item in arrBuff)
        {
            var buffDataParams = BuffUtil.ConvertBuffData(item.Key, item.Value);
            var addKey = BuffMgr.Instance.AddEntityBuff(entityID, entityID, item.Key, buffDataParams);
            BuffUtil.PushConvertBuffData(buffDataParams);

            if (BuffMgr.Instance.GetBuffType(addKey) != EnBuffType.Time)
                _BuffAddKeyList.Add(addKey);
        }
    }
    public void OnDisable(int entityID)
    {
        foreach (var addKey in _BuffAddKeyList)
        {
            BuffMgr.Instance.RemoveEntityBuff(addKey);
        }
        ResetAllItemData();
        _EventListIndex = 0;
        _BuffAddKeyList.Clear();
    }

    public int GetClipID()
    {
        return _ClipID;
    }
    public bool IsCanNextAction(float schedule)
    {
        var result = schedule >= canNextTime;
        return result;
    }

}
