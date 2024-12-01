using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameClassPoolData<T> : ClassData
    where T: class, IGamePool, new()
{
    //public static EnClassType ClassType => EnClassType.GameClassPoolData;
    private int m_UseCount = 0;
    private List<T> m_ListClass = new(GlobalConfig.Int5);

    public void Push(T classType)
    {
        m_UseCount--;
        m_ListClass.Add(classType);
    }
    public T Pull()
    {
        m_UseCount++;
        T result = null;
        if (m_ListClass.Count > 0)
        {
            result = m_ListClass[^1];
            m_ListClass.RemoveAt(m_ListClass.Count - 1);
        }
        else
        {
            result = new();
            result.PoolConstructor();
        }
        result.OnPoolGet();
        return result;
    }
}

