using System;


public interface ISkillData : IClassPool
{
    
}
public interface ISkillData<T> : ISkillData, IClassPool<T>
    where T: class, IClassPoolUserData
{

}