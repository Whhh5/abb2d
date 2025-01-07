using System;

public interface IAttackLinkScheduleItem: IGamePool
{
    public void SetScheduleType(EnAtkLinkScheculeType scheduleType);
    public EnAtkLinkScheculeType GetScheduleType();
    public void Init(int[] data, int arrCount, ref int startIndex);
    public void Enter(int entityID);
    public void Exit();
    public void Reset();
    public bool GetIsEffect();
    public float GetEnterSchedule();
    public void SetIsEffect(bool isEffect);
}