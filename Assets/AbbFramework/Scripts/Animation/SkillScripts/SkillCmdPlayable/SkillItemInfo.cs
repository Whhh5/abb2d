using System;
using System.Collections.Generic;

public class SkillItemInfo : IClassPool<PoolNaNUserData>
{
    public int _ClipID;
    public float canNextTime;
    public float atkEndTime;
    public bool _IsAutoRemove = true;
    protected Dictionary<EnBuff, int[]> arrBuff = new();

    protected ISkillScheduleAction [] m_ArrAtkLinkSchedule = null;
    public int ScheduleEventCount => m_ArrAtkLinkSchedule.Length;


    private int m_CurScheduleItemIndex = 0;

    public void OnPoolDestroy()
    {
        ResetAllItemData();
        foreach (var item in m_ArrAtkLinkSchedule)
        {
            ClassPoolMgr.Instance.Push(item);
        }
        arrBuff.Clear();
        m_ArrAtkLinkSchedule = null;
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
        _ClipID = gCount < 1 ? default : data[startIndex++];
        canNextTime = gCount < 2 ? default : data[startIndex++] / 100f;
        atkEndTime = gCount < 3 ? default : data[startIndex++] / 100f;
        _IsAutoRemove = gCount < 4 ? _IsAutoRemove : (data[startIndex++] > 0);

        var scheduleCount = startIndex >= endIndex ? default : data[startIndex++];
        m_ArrAtkLinkSchedule = new ISkillScheduleAction [scheduleCount];
        for (int i = 0; i < scheduleCount; i++)
        {
            var scheduleType = (EnAtkLinkScheculeType)data[startIndex++];
            var scheduleItem = AttackMgr.GetAtkLinkScheduleItem(scheduleType, data, ref startIndex);
            m_ArrAtkLinkSchedule[i] = scheduleItem;
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
    }

    public ISkillScheduleAction  GetCurScheduleItem()
    {
        return m_ArrAtkLinkSchedule[m_CurScheduleItemIndex];
    }
    public bool NextEventAction()
    {
        if (m_CurScheduleItemIndex >= m_ArrAtkLinkSchedule.Length - 1)
            return false;
        GetCurScheduleItem().Reset();
        m_CurScheduleItemIndex++;
        return true;
    }
    public void ResetAllItemData()
    {
        m_CurScheduleItemIndex = 0;
        foreach (var item in m_ArrAtkLinkSchedule)
        {
            item.Reset();
        }
    }

    public void OnEnable(int entityID)
    {
        foreach (var item in arrBuff)
        {
            var buffDataParams = BuffMgr.Instance.GetBuffData(item.Key, item.Value);
            Entity3DMgr.Instance.AddEntityBuff(entityID, item.Key, buffDataParams);
        }
    }
    public void OnDisable(int entityID)
    {
        foreach (var item in arrBuff)
        {
            //var buffDataParams = BuffMgr.Instance.GetBuffData(item.Key, item.Value);
            Entity3DMgr.Instance.RemoveEntityBuff(entityID, item.Key);
        }

        m_CurScheduleItemIndex = 0;
        if (ScheduleEventCount > 0)
            GetCurScheduleItem().Reset();

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
