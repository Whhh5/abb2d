using System;

public interface IAttackLinkScheduleItem: IClassPool
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
public interface IAttackLinkScheduleItem<T> : IAttackLinkScheduleItem, IClassPool<T>
    where T : class, IClassPoolUserData
{

}