using System;
using System.Collections.Generic;

public class AttackLinkItemData : IGamePool
{
    public int clipID;
    public float canNextTime;
    public float atkEndTime;
    public Dictionary<EnBuff, int[]> arrBuff = new();

    protected IAttackLinkScheduleItem[] m_ArrAtkLinkSchedule = null;
    public int ScheduleEventCount => m_ArrAtkLinkSchedule.Length;


    public void Destroy()
    {

    }
    public void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = startIndex + arrCount;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        clipID = gCount < 1 ? default : data[startIndex++];
        canNextTime = gCount < 2 ? default : data[startIndex++] / 100f;
        atkEndTime = gCount < 3 ? default : data[startIndex++] / 100f;

        var scheduleCount = startIndex >= endIndex ? default : data[startIndex++];
        m_ArrAtkLinkSchedule = new IAttackLinkScheduleItem[scheduleCount];
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
            arrBuff.Add(buff, arrParams);
        }
    }

    private int m_CurScheduleItemIndex = 0;

    public IAttackLinkScheduleItem GetCurScheduleItem()
    {
        return m_ArrAtkLinkSchedule[m_CurScheduleItemIndex];
    }

    public bool NextEventAction()
    {
        if (m_CurScheduleItemIndex >= m_ArrAtkLinkSchedule.Length - 1)
            return false;
        m_CurScheduleItemIndex++;
        GetCurScheduleItem().Reset();
        return true;
    }

    public void OnEnable()
    {

    }
    public void OnDisable()
    {
        m_CurScheduleItemIndex = 0;
        if (ScheduleEventCount > 0)
            GetCurScheduleItem().Reset();

    }

}
