using System;



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

    private bool m_IsEffect = false;
    private EnAtkLinkScheculeType m_ScheduleType = EnAtkLinkScheculeType.None;
    public void OnPoolDestroy()
    {
        SkillFactory.DestroySkillBehaviour(ref _SkillBehaviour);
        //arrParams = null;
        m_ScheduleType = EnAtkLinkScheculeType.None;
        m_IsEffect = false;
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
    public void Enter(int entityID)
    {
        //var height = arrParams[0] / 100f;
        ////var time = arrPArams[1] / 100f;
        //Entity3DMgr.Instance.SetEntityHeight(entityID, height, 1);

        _SkillBehaviour.Execute(entityID);
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
