using System;
using System.Collections.Generic;

public interface ISkillScheduleAction : IClassPoolDestroy
{
    public void SetScheduleType(EnAtkLinkScheculeType scheduleType);
    public EnAtkLinkScheculeType GetScheduleType();
    public void Init(int[] data, int arrCount, ref int startIndex);
    //public void Enter(int entityID);
    //public void Exit();
    public void Reset();
    //public bool GetIsEffect();
    //public float GetEnterSchedule();
    //public void SetIsEffect(bool isEffect);

    public void GetEventList(ref List<SkillItemEventInfo> eventList);
}