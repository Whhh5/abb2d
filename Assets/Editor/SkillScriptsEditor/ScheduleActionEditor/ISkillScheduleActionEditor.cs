

using System.Collections.Generic;
using UnityEngine;

public interface ISkillScheduleActionEditor : ISkillScheduleAction, ISkillTypeEditor
{
    void ISkillScheduleAction.GetEventList(ref List<SkillItemEventInfo> eventList) { }
    public float GetEnterSchedule();
    public void Sumilation(Rect rect, float itemStartTime, float itemEndTime);
}