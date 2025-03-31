using System;
using System.Collections.Generic;

public enum EnSkillBehaviourType
{
    None,
    [EditorFieldName("高度")]
    Height,
}

public class SkillBehaviourScheduleAction : ISkillScheduleAction
{
    public float schedule;
    public EnSkillBehaviourType behaviourType;
    //public int[] arrParams;
    protected ISkillBehaviour _SkillBehaviour;

    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;
    public void OnPoolDestroy()
    {
        SkillFactory.DestroySkillBehaviour(ref _SkillBehaviour);
        //arrParams = null;
        m_ScheduleType = EnAtkLinkScheculeType.None;
    }

    public void Init(int[] data, int arrCount, ref int startIndex)
    {
        var endIndex = startIndex + arrCount;
        var gCount = startIndex >= endIndex ? default : data[startIndex++];
        schedule = gCount < 1 ? default : (data[startIndex++] / 100f);
        behaviourType = gCount < 2 ? default : (EnSkillBehaviourType)data[startIndex++];

        var paramCount = startIndex >= endIndex ? default : data[startIndex++];
        //arrParams = data.Copy(startIndex, paramCount);

        var userData = ClassPoolMgr.Instance.Pull<CommonSkillItemParamUserData>();
        userData.startIndex = startIndex;
        userData.arrParams = data;
        userData.paramCount = paramCount;
        _SkillBehaviour = SkillFactory.CreateSkillBehaviour(behaviourType, userData);
        ClassPoolMgr.Instance.Push(userData);
        startIndex += paramCount;
    }

    public void GetEventList(ref List<SkillItemEventInfo> eventList)
    {
        var eventData = ClassPoolMgr.Instance.Pull<SkillItemEventInfo>();
        eventData.schedule = schedule;
        eventData.onEvent = ScheduleEvent;
        eventList.Add(eventData);
    }

    public void ScheduleEvent(int entityID, IClassPoolUserData userData)
    {
        //var height = arrParams[0] / 100f;
        ////var time = arrPArams[1] / 100f;
        //Entity3DMgr.Instance.SetEntityHeight(entityID, height, 1);

        _SkillBehaviour.Execute(entityID);
    }


    public void Reset()
    {
        m_ScheduleType = EnAtkLinkScheculeType.None;
    }

    public EnAtkLinkScheculeType GetScheduleType()
    {
        return m_ScheduleType;
    }

    public void SetScheduleType(EnAtkLinkScheculeType scheduleType)
    {
        m_ScheduleType = scheduleType;
    }

}
