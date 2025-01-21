using System;


public interface ISkillTypeData : IClassPool
{
    
}
public interface ISkillTypeData<T> : ISkillTypeData, IClassPool<T>
    where T: class, IClassPoolUserData
{

}